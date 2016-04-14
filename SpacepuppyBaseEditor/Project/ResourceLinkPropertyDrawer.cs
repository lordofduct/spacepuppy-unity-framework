using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomPropertyDrawer(typeof(ResourceLink))]
    public class ResourceLinkPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pathProperty = property.FindPropertyRelative("_path");
            var path = pathProperty.stringValue;

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(ResourceLink.ConfigAttribute), false).FirstOrDefault() as ResourceLink.ConfigAttribute;

            var tp = (attrib != null && attrib.resourceType != null) ? attrib.resourceType : typeof(UnityEngine.Object);
            UnityEngine.Object asset = (!StringUtil.IsNullOrWhitespace(path)) ? Resources.Load(path, tp) : null;

            EditorGUI.BeginChangeCheck();
            asset = SPEditorGUI.ObjectFieldX(position, asset, tp, false);
            if(EditorGUI.EndChangeCheck())
            {
                if(asset == null)
                {
                    pathProperty.stringValue = null;
                }
                else
                {
                    path = this.GetPath(asset);
                    if(!string.IsNullOrEmpty(path))
                    {
                        pathProperty.stringValue = path;
                    }
                } 
            }
        }


        private string GetPath(UnityEngine.Object asset)
        {
            if (asset == null) return string.Empty;

            var path = AssetDatabase.GetAssetPath(asset);
            if (!path.Contains("Resources/")) return string.Empty;

            path = path.Substring(path.LastIndexOf("Resources/") + 10);
            path = Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path));
            
            return path;
        }



        private static string Combine(string path1, string path2)
        {
            if (StringUtil.IsNullOrWhitespace(path1)) return path2;
            if (StringUtil.IsNullOrWhitespace(path2)) return path1;

            if (path1.EndsWith("/"))
            {
                if (path2.StartsWith("/"))
                    return path1 + path2.Substring(1);
                else
                    return path1 + path2;
            }
            else if(path2.StartsWith("/"))
            {
                return path1 + path2;
            }
            else
            {
                return path1 + "/" + path2;
            }
        }
        

    }
}
