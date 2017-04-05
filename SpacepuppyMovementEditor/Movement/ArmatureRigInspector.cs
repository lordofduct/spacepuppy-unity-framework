using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Movement
{

    [CustomEditor(typeof(ArmatureRig), true)]
    public class ArmatureRigInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            var targ = this.target as ArmatureRig;

            this.DrawDefaultInspectorExcept("Joints");

            var cache = SPGUI.Disable();
            EditorGUILayout.FloatField("Mass", targ.CalculateMass());
            cache.Reset();

            EditorGUI.BeginChangeCheck();
            Rigidbody rootJoint = EditorGUILayout.ObjectField("Root Joint", targ.RootJoint, typeof(Rigidbody), true) as Rigidbody;
            if (EditorGUI.EndChangeCheck())
            {
                if (rootJoint != null)
                {
                    if (targ.JointCount == 0)
                    {
                        targ.SetJoints(new Rigidbody[] { rootJoint });
                    }
                    else
                    {
                        using (var lst = TempCollection.GetList<Rigidbody>(targ.Joints))
                        {
                            lst.Remove(rootJoint);
                            lst.Insert(0, rootJoint);
                            targ.SetJoints(lst);
                        }
                    }

                    this.serializedObject.Update();
                }
            }

            this.DrawPropertyField("Joints", true);

            //TODO - implement a button that allows removing and setting the bones of the rig

            EditorGUILayout.Separator();

            if (GUILayout.Button("Build Ragdoll Rig"))
            {
                //EditorApplication.ExecuteMenuItem("GameObject/Create Other/Ragdoll...");
                com.spacepuppyeditor.Base.SPCreateRagdollWizard.StartWizard();
            }

            if (GUILayout.Button("Generate Joint List"))
            {
                targ.ReloadJointList();
                targ.LoadJointListOnStart = false;
                this.serializedObject.Update();
            }

            bool bIsKinematic = false;
            if (targ.Joints.Count() > 0) bIsKinematic = targ.Joints.First().isKinematic;
            string msg = (bIsKinematic) ? "Set Joints Not Kinematic" : "Set Joints Kinematic";
            if (GUILayout.Button(msg))
            {
                targ.SetJointsKinematic(!bIsKinematic);
            }

            if (GUILayout.Button("Try Clear Ragdoll Rig"))
            {
                if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to clear rig?", "Yes", "No"))
                {
                    //foreach (var t in targ.GetAllChildrenAndSelf())
                    //{
                    //    if (t.HasComponent<CharacterJoint>())
                    //    {
                    //        DestroyImmediate(t.GetComponent<CharacterJoint>());
                    //        DestroyImmediate(t.GetComponent<Collider>());
                    //        DestroyImmediate(t.GetComponent<Rigidbody>());
                    //    }
                    //}
                    targ.ReloadJointList();
                    foreach(var j in targ.Joints)
                    {
                        if (j.HasComponent<CharacterJoint>()) DestroyImmediate(j.GetComponent<CharacterJoint>());
                        DestroyImmediate(j.GetComponent<Collider>());
                        DestroyImmediate(j);
                    }
                    targ.SetJoints(null);
                    this.serializedObject.Update();
                }
            }

            if (GUILayout.Button("Clear Defined Joints"))
            {
                if (targ.Joints.Count() > 0)
                {
                    if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to clear all defined joints?", "Yes", "No"))
                    {
                        foreach (var j in targ.Joints)
                        {
                            var go = j.gameObject;
                            DestroyImmediate(go.GetComponent<CharacterJoint>());
                            DestroyImmediate(go.GetComponent<Collider>());
                            DestroyImmediate(go.GetComponent<Rigidbody>());
                        }
                    }

                    targ.SetJoints(null);
                    this.serializedObject.Update();
                }
            }

            if (Application.isPlaying)
            {
                GUILayout.Space(10);

                if (!targ.Ragdolled)
                {
                    if (GUILayout.Button("Ragdoll..."))
                    {
                        targ.Ragdoll();
                    }
                }
                else
                {
                    if (GUILayout.Button("Undo Ragdoll..."))
                    {
                        targ.UndoRagdoll();
                    }
                }
            }

        }

    }

}