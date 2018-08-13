using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Utils;
using com.spacepuppyeditor;

namespace com.spacepuppyeditor.Commands
{
    class ScanAndUpdateGuidForComponents : EditorWindow
    {


        #region Menu

        [MenuItem(SPMenu.MENU_NAME_TOOLS + "/Scan and Update Guid for Components", priority = SPMenu.MENU_PRIORITY_TOOLS)]
        static void CreateRigWizard()
        {
            var wizard = EditorWindow.GetWindow<ScanAndUpdateGuidForComponents>(true, "Scane Component Guid Move", true);
        }

        #endregion



        #region Fields

        private System.Type _typeToReplace;
        private System.Type _typeToReplaceWith;

        #endregion

        #region Methods

        private void OnGUI()
        {
            _typeToReplace = SPEditorGUILayout.TypeDropDown(EditorHelper.TempContent("Type to Replace"), typeof(MonoBehaviour), _typeToReplace);
            _typeToReplaceWith = SPEditorGUILayout.TypeDropDown(EditorHelper.TempContent("New Type"), typeof(MonoBehaviour), _typeToReplaceWith);

            if (GUILayout.Button("Scan"))
            {
                this.DoScan();
            }
        }

        private void DoScan()
        {
            if (_typeToReplace == _typeToReplaceWith) return;
            if (_typeToReplace == null) return;
            if (_typeToReplaceWith == null) return;

            //get guids
            MonoScript script1;
            MonoScript script2;
            if (!GetMonoScripts(_typeToReplace, _typeToReplaceWith, out script1, out script2)) return;

            var spath1 = AssetDatabase.GetAssetPath(script1);
            var spath2 = AssetDatabase.GetAssetPath(script2);
            var sMetaPath1 = spath1 + ".meta";
            var sMetaPath2 = spath2 + ".meta";

            string sguid1;
            string sguid2;
            if (!this.GetGUIDs(sMetaPath1, sMetaPath2, out sguid1, out sguid2)) return;

            string fileid1 = (spath1.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase)) ? FileIDUtil.Compute(_typeToReplace).ToString() : "11500000";
            string fileid2 = (spath2.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase)) ? FileIDUtil.Compute(_typeToReplaceWith).ToString() : "11500000";

            const string MATCH_PATTERN = "{{fileID: {0}, guid: {1}, type: 3}}";
            foreach (var sfile in Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories))
            {
                var pattern = string.Format(MATCH_PATTERN, fileid1, sguid1);
                string contents;
                using (var tr = new StreamReader(sfile))
                {
                    contents = tr.ReadToEnd();
                }

                if (contents.Contains(pattern))
                {
                    contents = contents.Replace(pattern, string.Format(MATCH_PATTERN, fileid2, sguid2));
                    using (var tw = new StreamWriter(sfile, false))
                    {
                        tw.Write(contents);
                        Debug.Log(sfile);
                    }
                }
            }
            foreach (var sfile in Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories))
            {
                var pattern = string.Format(MATCH_PATTERN, fileid1, sguid1);
                string contents;
                using (var tr = new StreamReader(sfile))
                {
                    contents = tr.ReadToEnd();
                }

                if (contents.Contains(pattern))
                {
                    contents = contents.Replace(pattern, string.Format(MATCH_PATTERN, fileid2, sguid2));
                    using (var tw = new StreamWriter(sfile, false))
                    {
                        tw.Write(contents);
                        Debug.Log(sfile);
                    }
                }
            }
        }

        private bool GetMonoScripts(System.Type type1, System.Type type2, out MonoScript script1, out MonoScript script2)
        {
            script1 = null;
            script2 = null;

            var go = new GameObject("temp");
            var c1 = go.AddComponent(type1) as MonoBehaviour;
            var c2 = go.AddComponent(type2) as MonoBehaviour;
            if (c1 == null || c2 == null)
            {
                GameObject.DestroyImmediate(go);
                return false;
            }

            script1 = MonoScript.FromMonoBehaviour(c1);
            script2 = MonoScript.FromMonoBehaviour(c2);

            GameObject.DestroyImmediate(go);

            return true;
        }

        private bool GetGUIDs(string sMetaPath1, string sMetaPath2, out string sguid1, out string sguid2)
        {
            sguid1 = null;
            sguid2 = null;

            try
            {
                //var yaml = new YamlStream();
                //var idGUID = new YamlScalarNode("guid");

                //using (var tr = new StreamReader(EditorHelper.GetFullPathForAssetPath(sMetaPath1)))
                //{
                //    yaml.Load(tr);
                //    var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
                //    sguid1 = (mapping.Children[idGUID] as YamlScalarNode).Value;
                //}
                //using (var tr = new StreamReader(EditorHelper.GetFullPathForAssetPath(sMetaPath2)))
                //{
                //    yaml.Load(tr);
                //    var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
                //    sguid2 = (mapping.Children[idGUID] as YamlScalarNode).Value;
                //}

                var rx = new System.Text.RegularExpressions.Regex(@"\s*guid:\s*(?<id>[a-zA-Z0-9]+?)\s*$");
                string line;
                using (var tr = new StreamReader(EditorHelper.GetFullPathForAssetPath(sMetaPath1)))
                {
                    while(!tr.EndOfStream)
                    {
                        line = tr.ReadLine();
                        var match = rx.Match(line);
                        if (match.Success)
                        {
                            sguid1 = match.Groups["id"].Value;
                            break;
                        }
                    }
                }
                using (var tr = new StreamReader(EditorHelper.GetFullPathForAssetPath(sMetaPath2)))
                {
                    while (!tr.EndOfStream)
                    {
                        line = tr.ReadLine();
                        var match = rx.Match(line);
                        if (match.Success)
                        {
                            sguid2 = match.Groups["id"].Value;
                            break;
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


    }
}
