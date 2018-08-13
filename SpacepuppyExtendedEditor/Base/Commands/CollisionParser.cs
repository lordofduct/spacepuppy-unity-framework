using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    public class CollisionParser : SPEditor
    {

        public const string MENU_NAME = SPMenu.MENU_NAME_ROOT + "/Parse Collision";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_GROUP2;

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        private static void DoParseCollision()
        {
            if (Selection.activeGameObject == null) return;

            //get known layers
            string[] layers = new string[32];
            for (int i = 0; i < 32; i++)
            {
                layers[i] = LayerMask.LayerToName(i);
            }

            //do work
            var go = Selection.activeGameObject;
            foreach (var child in go.GetAllChildrenAndSelf())
            {
                var arr = StringUtil.SplitFixedLength(child.name, ".", 3);
                if (string.Equals(arr[0], "collision", System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arr[0], "trigger", System.StringComparison.OrdinalIgnoreCase))
                {
                    var bTrigger = string.Equals(arr[0], "trigger", System.StringComparison.OrdinalIgnoreCase);
                    //remove old collider
                    if (child.HasComponent<Collider>()) child.RemoveComponents<Collider>();

                    //set collider
                    if (string.IsNullOrEmpty(arr[1])) arr[1] = "box"; //default to box
                    else arr[1] = arr[1].ToLower();

                    MeshFilter filter = child.GetComponent<MeshFilter>();
                    switch (arr[1])
                    {
                        case "mesh":
                            var meshCollider = child.AddComponent<MeshCollider>();
                            meshCollider.isTrigger = bTrigger;
                            if (filter != null)
                            {
                                meshCollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                            }
                            break;
                        case "box":
                            var boxCollider = child.AddComponent<BoxCollider>();
                            boxCollider.isTrigger = bTrigger;
                            if (filter != null && filter.sharedMesh != null)
                            {
                                var bounds = filter.sharedMesh.bounds;
                                boxCollider.center = bounds.center;
                                boxCollider.size = bounds.size;
                            }

                            break;
                        case "sphere":
                            var sphereCollider = child.AddComponent<SphereCollider>();
                            sphereCollider.isTrigger = bTrigger;
                            if (filter != null && filter.sharedMesh != null)
                            {
                                var bounds = com.spacepuppy.Geom.Sphere.FromMesh(filter.sharedMesh, spacepuppy.Geom.BoundingSphereAlgorithm.FromBounds);
                                sphereCollider.center = bounds.Center;
                                sphereCollider.radius = bounds.Radius;
                            }
                            break;
                        case "capsule":
                            var capCollider = child.AddComponent<CapsuleCollider>();
                            capCollider.isTrigger = bTrigger;
                            if (filter != null && filter.sharedMesh != null)
                            {
                                var bounds = filter.sharedMesh.bounds;
                                capCollider.center = bounds.center;
                                capCollider.height = bounds.size.y;
                                capCollider.radius = Mathf.Max(bounds.extents.x, bounds.extents.z);
                            }
                            break;
                    }

                    //set layer
                    if (!string.IsNullOrEmpty(arr[2]))
                    {
                        for (int i = 0; i < layers.Length; i++)
                        {
                            if (string.Equals(arr[2], layers[i], System.StringComparison.OrdinalIgnoreCase))
                            {
                                child.gameObject.layer = i;
                                break;
                            }
                        }
                    }

                    if (child.HasComponent<Renderer>()) child.GetComponent<Renderer>().enabled = false;
                }
            }
        }



    }

}
