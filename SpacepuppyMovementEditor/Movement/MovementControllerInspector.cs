using UnityEngine;
using UnityEditor;

using com.spacepuppy;

namespace com.spacepuppyeditor.Movement
{

    [CustomEditor(typeof(MovementController))]
    public class MovementControllerInspector : SPEditor
    {

        public enum GameObjectMoverType
        {
            Unknown = -1,
            CharacterController = 0,
            SimulatedRigidbody = 1,
            DirectRigidbody = 2,
            DumbMover = 3,
            Ragdoll = 4,
            Emulated = 5
        }

        #region Properties

        public new MovementController target { get { return base.target as MovementController; } }

        #endregion

        #region Inspector

        protected override void OnSPInspectorGUI()
        {
            //deal with regular serialized properties
            this.serializedObject.Update();

            this.DrawPropertyField("_resetVelocityOnNoMove", EditorHelper.TempContent("Reset Velocity On No Move", "By default, if move is not called in an update cycle, the velocity before hand is conserved. If this is true, then if move is not called we reset the velocity to zero. Note - adding force is considered moving, where as teleporting is not."), false);

            this.serializedObject.ApplyModifiedProperties();

            //deal with the special movertype
            this.DrawMoverType();

            if (Application.isPlaying)
            {
                if (this.target.Mover != null)
                {
                    EditorGUILayout.HelpBox("Currently active mover type is '" + this.target.Mover.GetType().Name + "'.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Currently active mover type is null.", MessageType.Info);
                }
            }

        }


        private void DrawMoverType()
        {
            EditorGUI.BeginChangeCheck();

            var targ = this.target as MovementController;
            var oldTp = GetMoverType(targ.Mover);
            var tp = (GameObjectMoverType)EditorGUILayout.EnumPopup(EditorHelper.TempContent("Mover Type"), oldTp);
            if (tp != oldTp)
            {
                GUI.changed = true;
                if (tp == GameObjectMoverType.Unknown)
                {
                    //bad
                }
                else
                {
                    var mover = BuildMover(tp);
                    if(targ.Mover != null)
                    {
                        mover.Mass = targ.Mover.Mass;
                        mover.StepOffset = targ.Mover.StepOffset;
                        mover.SkinWidth = targ.Mover.SkinWidth;
                    }
                    targ.ChangeMover(mover);
                }
            }

            GUIContent label;
            if (targ.Mover != null)
            {
                switch (tp)
                {
                    case GameObjectMoverType.CharacterController:
                        {
                            EditorGUILayout.LabelField(EditorHelper.TempContent("Prefers Fixed Update"), new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));

                            label = new GUIContent("Mass", "Since CharacterControllers don't have mass, this represents mass for when calculating AddForce for the motor.");
                            targ.Mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                            label = new GUIContent("Skin Width", "CharacterController 'skinWidth' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's skinWidth.");
                            targ.Mover.SkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);
                        }
                        break;
                    case GameObjectMoverType.SimulatedRigidbody:
                        {
                            (targ.Mover as MovementController.SimulatedRigidbodyMover).PrefersFixedUpdate = EditorGUILayout.Toggle(EditorHelper.TempContent("Prefers Fixed Update"), targ.Mover.PrefersFixedUpdate);

                            //label = new GUIContent("Mass", "CharacterControllers don't have mass by default, so this acts as the mass.");
                            //targ.Mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                            label = new GUIContent("Skin Width", "Since Rigidbodies don't have skin, this acts as the skinWidth used by various motors and resolvers.");
                            targ.Mover.SkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);

                            label = new GUIContent("Step Offset", "CharacterController 'stepOffset' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's stepOffset.");
                            targ.Mover.StepOffset = EditorGUILayout.FloatField(label, targ.Mover.StepOffset);
                        }
                        break;
                    case GameObjectMoverType.DirectRigidbody:
                        {
                            label = new GUIContent("Prefers Fixed Update");
                            EditorGUILayout.LabelField(label, new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));

                            //label = new GUIContent("Mass", "CharacterControllers don't have mass by default, so this acts as the mass.");
                            //targ.Mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                            label = new GUIContent("Skin Width", "Since Rigidbodies don't have skin, this acts as the skinWidth used by various motors and resolvers.");
                            targ.Mover.SkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);

                            label = new GUIContent("Step Offset", "CharacterController 'stepOffset' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's stepOffset.");
                            targ.Mover.StepOffset = EditorGUILayout.FloatField(label, targ.Mover.StepOffset);

                            label = new GUIContent("Free Movement", "When FreeMovement is true, then Unity Physics takes over if Move isn't called. This is useful for things like debris.");
                            (targ.Mover as MovementController.DirectRigidBodyMover).FreeMovement = EditorGUILayout.Toggle(label, (targ.Mover as MovementController.DirectRigidBodyMover).FreeMovement);
                        }
                        break;
                    case GameObjectMoverType.DumbMover:
                        {
                            label = new GUIContent("Prefers Fixed Update");
                            EditorGUILayout.LabelField(label, new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));
                        }
                        break;

                    case GameObjectMoverType.Ragdoll:
                        //nothing is drawn
                        break;

                    case GameObjectMoverType.Emulated:
                        {
                            var mover = targ.Mover as MovementController.EmulatedCharacterControllerBodyMover;

                            label = new GUIContent("Mass", "CharacterControllers don't have mass by default, so this acts as the mass.");
                            mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                            label = new GUIContent("Skin Width", "Since Rigidbodies don't have skin, this acts as the skinWidth used by various motors and resolvers.");
                            mover.SkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);

                            label = new GUIContent("Step Offset", "CharacterController 'stepOffset' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's stepOffset.");
                            mover.StepOffset = EditorGUILayout.FloatField(label, targ.Mover.StepOffset);
                            
                            var geom = mover.Geom;
                            var c = geom.Center;
                            var r = geom.Radius;
                            var h = geom.Height;

                            c = EditorGUILayout.Vector3Field(EditorHelper.TempContent("Center"), c);
                            r = EditorGUILayout.FloatField(EditorHelper.TempContent("Radius"), r);
                            h = EditorGUILayout.FloatField(EditorHelper.TempContent("Height"), h);
                            mover.Geom = new spacepuppy.Geom.Capsule(c, Vector3.up, h, r);
                        }
                        break;
                }
            }
            else
            {
                EditorGUILayout.LabelField("ERROR: Failed to configure for selected MoverType.");
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(targ);
        }



        private void DrawMoverType_Old()
        {
            var targ = this.target as MovementController;
            var oldTp = GetMoverType(targ.Mover);
            var tp = (GameObjectMoverType)EditorGUILayout.EnumPopup(EditorHelper.TempContent("Mover Type"), oldTp);
            if (tp != oldTp)
            {
                if (tp == GameObjectMoverType.Unknown)
                {
                    //bad
                }
                else
                {
                    targ.ChangeMover(BuildMover(tp));
                }
            }

            EditorGUI.BeginChangeCheck();

            GUIContent label;
            if (targ.Mover != null)
            {
                bool bSetDirty = false;
                switch (tp)
                {
                    case GameObjectMoverType.CharacterController:
                        label = new GUIContent("Prefers Fixed Update");
                        EditorGUILayout.LabelField(EditorHelper.TempContent("Prefers Fixed Update"), new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));

                        label = new GUIContent("Mass", "Since CharacterControllers don't have mass, this represents mass for when calculating AddForce for the motor.");
                        var ccMass = EditorGUILayout.FloatField(label, targ.Mover.Mass);
                        if (ccMass != targ.Mover.Mass)
                        {
                            targ.Mover.Mass = ccMass;
                            bSetDirty = true;
                        }

                        label = new GUIContent("Skin Width", "CharacterController 'skinWidth' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's skinWidth.");
                        var ccSkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);
                        if (ccSkinWidth != targ.Mover.SkinWidth)
                        {
                            targ.Mover.SkinWidth = ccSkinWidth;
                            bSetDirty = true;
                        }

                        break;
                    case GameObjectMoverType.SimulatedRigidbody:
                        label = new GUIContent("Prefers Fixed Update");
                        (targ.Mover as MovementController.SimulatedRigidbodyMover).PrefersFixedUpdate = EditorGUILayout.Toggle(label, targ.Mover.PrefersFixedUpdate);

                        //label = new GUIContent("Mass", "CharacterControllers don't have mass by default, so this acts as the mass.");
                        //targ.Mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                        label = new GUIContent("Skin Width", "Since Rigidbodies don't have skin, this acts as the skinWidth used by various motors and resolvers.");
                        var srSkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);
                        if (srSkinWidth != targ.Mover.SkinWidth)
                        {
                            targ.Mover.SkinWidth = srSkinWidth;
                            bSetDirty = true;
                        }

                        label = new GUIContent("Step Offset", "CharacterController 'stepOffset' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's stepOffset.");
                        var srStepOffset = EditorGUILayout.FloatField(label, targ.Mover.StepOffset);
                        if (srStepOffset != targ.Mover.StepOffset)
                        {
                            targ.Mover.StepOffset = srStepOffset;
                            bSetDirty = true;
                        }

                        break;
                    case GameObjectMoverType.DirectRigidbody:
                        label = new GUIContent("Prefers Fixed Update");
                        EditorGUILayout.LabelField(label, new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));

                        //label = new GUIContent("Mass", "CharacterControllers don't have mass by default, so this acts as the mass.");
                        //targ.Mover.Mass = EditorGUILayout.FloatField(label, targ.Mover.Mass);

                        label = new GUIContent("Skin Width", "Since Rigidbodies don't have skin, this acts as the skinWidth used by various motors and resolvers.");
                        var drSkinWidth = EditorGUILayout.FloatField(label, targ.Mover.SkinWidth);
                        if (drSkinWidth != targ.Mover.SkinWidth)
                        {
                            targ.Mover.SkinWidth = drSkinWidth;
                            bSetDirty = true;
                        }

                        label = new GUIContent("Step Offset", "CharacterController 'stepOffset' property isn't directly accessible in code. This should be set to be at least the same as the CharacterController's stepOffset.");
                        var drStepOffset = EditorGUILayout.FloatField(label, targ.Mover.StepOffset);
                        if (drStepOffset != targ.Mover.StepOffset)
                        {
                            targ.Mover.StepOffset = drStepOffset;
                            bSetDirty = true;
                        }

                        label = new GUIContent("Free Movement", "When FreeMovement is true, then Unity Physics takes over if Move isn't called. This is useful for things like debris.");
                        var drFreeMove = EditorGUILayout.Toggle(label, (targ.Mover as MovementController.DirectRigidBodyMover).FreeMovement);
                        if (drFreeMove != (targ.Mover as MovementController.DirectRigidBodyMover).FreeMovement)
                        {
                            (targ.Mover as MovementController.DirectRigidBodyMover).FreeMovement = drFreeMove;
                            bSetDirty = true;
                        }

                        break;
                    case GameObjectMoverType.DumbMover:
                        label = new GUIContent("Prefers Fixed Update");
                        EditorGUILayout.LabelField(label, new GUIContent(targ.Mover.PrefersFixedUpdate.ToString()));

                        break;

                    case GameObjectMoverType.Ragdoll:
                        //nothing is drawn
                        break;
                }

                if (bSetDirty) EditorUtility.SetDirty(targ);
            }
            else
            {
                EditorGUILayout.LabelField("ERROR: Failed to configure for selected MoverType.");
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(targ);
        }


        #endregion

        #region Utils

        public static GameObjectMoverType GetMoverType(MovementController.IGameObjectMover mover)
        {
            if (mover is MovementController.CharacterControllerMover)
            {
                return GameObjectMoverType.CharacterController;
            }
            else if (mover is MovementController.SimulatedRigidbodyMover)
            {
                return GameObjectMoverType.SimulatedRigidbody;
            }
            else if (mover is MovementController.DirectRigidBodyMover)
            {
                return GameObjectMoverType.DirectRigidbody;
            }
            else if (mover is MovementController.DumbMover)
            {
                return GameObjectMoverType.DumbMover;
            }
            else if (mover is MovementController.RagdollBodyMover)
            {
                return GameObjectMoverType.Ragdoll;
            }
            else if (mover is MovementController.EmulatedCharacterControllerBodyMover)
            {
                return GameObjectMoverType.Emulated;
            }
            else
            {
                return GameObjectMoverType.Unknown;
            }
        }

        public static MovementController.IGameObjectMover BuildMover(GameObjectMoverType tp)
        {
            switch (tp)
            {
                case GameObjectMoverType.CharacterController:
                    return new MovementController.CharacterControllerMover();
                case GameObjectMoverType.SimulatedRigidbody:
                    return new MovementController.SimulatedRigidbodyMover();
                case GameObjectMoverType.DirectRigidbody:
                    return new MovementController.DirectRigidBodyMover();
                case GameObjectMoverType.DumbMover:
                    return new MovementController.DumbMover();
                case GameObjectMoverType.Ragdoll:
                    return new MovementController.RagdollBodyMover();
                case GameObjectMoverType.Emulated:
                    return new MovementController.EmulatedCharacterControllerBodyMover();
                default:
                    throw new System.InvalidOperationException("Unknown GameObjectMoverType is an invalid selection.");
            }
        }

        #endregion

    }

}