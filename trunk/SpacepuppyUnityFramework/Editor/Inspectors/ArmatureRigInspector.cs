using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Inspectors
{

    [CustomEditor(typeof(ArmatureRig), true)]
    public class ArmatureRigInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            var targ = this.target as ArmatureRig;

            this.DrawDefaultInspectorExcept("Joints");

            Rigidbody rootJoint = EditorGUILayout.ObjectField("Root Joint", targ.RootJoint, typeof(Rigidbody), true) as Rigidbody;
            if (rootJoint != targ.RootJoint && rootJoint != null)
            {
                if (targ.Joints == null)
                {
                    targ.Joints = new Rigidbody[] { rootJoint };
                }
                else
                {
                    var lst = new List<Rigidbody>(targ.Joints);
                    lst.Remove(rootJoint);
                    lst.Insert(0, rootJoint);
                    targ.Joints = lst.ToArray();
                }
                EditorUtility.SetDirty(targ);
                this.serializedObject.Update();
            }

            this.DrawDefaultInspector("Joints", true);

            //TODO - implement a button that allows removing and setting the bones of the rig

            EditorGUILayout.Separator();

            if (GUILayout.Button("Build Ragdoll Rig"))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Ragdoll...");
            }

            if (GUILayout.Button("Generate Joint List"))
            {
                targ.ReloadJointList();
                targ.LoadJointListOnStart = false;
            }

            bool bIsKinematic = false;
            if (targ.Joints != null && targ.Joints.Length > 0 && targ.Joints[0].rigidbody != null) bIsKinematic = targ.Joints[0].rigidbody.isKinematic;
            string msg = (bIsKinematic) ? "Set Joints Not Kinematic" : "Set Joints Kinematic";
            if (GUILayout.Button(msg))
            {
                targ.SetJointsKinematic(!bIsKinematic);
            }

            if (GUILayout.Button("Try Clear Ragdoll Rig"))
            {
                if (EditorUtility.DisplayDialog("WARNING!", "Are you sure you want to clear rig?", "Yes", "No"))
                {
                    foreach (var t in targ.GetAllChildrenAndSelf())
                    {
                        if (t.HasComponent<CharacterJoint>())
                        {
                            DestroyImmediate(t.GetComponent<CharacterJoint>());
                            DestroyImmediate(t.GetComponent<Collider>());
                            DestroyImmediate(t.GetComponent<Rigidbody>());
                        }
                    }
                }
            }

            if (GUILayout.Button("Clear Defined Joints"))
            {
                if (targ.Joints != null && targ.Joints.Length > 0)
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
                }
            }

        }

    }

}