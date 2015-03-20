using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor
{

    public static class EditorProjectPrefs
    {

        #region Static Interface

        private const string PREFS_DIR = @"Editor";
        private const string PREFS_PATH = PREFS_DIR + @"/Spacepuppy.EditorProjectPrefs.xml";
        private static string _projectId;

        static EditorProjectPrefs()
        {
            var spath = System.IO.Path.Combine(Application.dataPath, PREFS_PATH);
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Load(spath);
            }
            catch
            {
                xdoc = null;
            }

            if (xdoc != null)
            {
                var xattrib = xdoc.Root.Attribute("projectId");
                if (xattrib == null)
                {
                    xattrib = new XAttribute("projectId", "SPProj." + ShortGuid.NewGuid().ToString());
                    xdoc.Root.Add(xattrib);
                    xdoc.Save(spath);
                }
                else if (string.IsNullOrEmpty(xattrib.Value))
                {
                    xattrib.Value = "SPProj." + ShortGuid.NewGuid().ToString();
                    xdoc.Save(spath);
                }
                _projectId = xattrib.Value;
            }
            else
            {
                _projectId = "SPProj." + ShortGuid.NewGuid().ToString();
                xdoc = new XDocument(new XElement("root"));
                var xattrib = new XAttribute("projectId", _projectId);
                xdoc.Root.Add(xattrib);

                var sdir = System.IO.Path.GetDirectoryName(spath);
                if (!System.IO.Directory.Exists(sdir)) System.IO.Directory.CreateDirectory(sdir);
                xdoc.Save(spath);
            }
        }


        public static void DeleteAll()
        {
            foreach (var skey in GetAllKeys())
            {
                EditorPrefs.DeleteKey(skey);
            }
        }

        public static void DeleteKey(string key)
        {
            key = GetKey(key);
            EditorPrefs.DeleteKey(key);
        }

        public static bool HasKey(string key)
        {
            key = GetKey(key);
            return EditorPrefs.HasKey(key);
        }

        public static bool GetBool(string key)
        {
            key = GetKey(key);
            return EditorPrefs.GetBool(key);
        }
        public static bool GetBool(string key, bool defaultValue)
        {
            key = GetKey(key);
            return EditorPrefs.GetBool(key, defaultValue);
        }

        public static int GetInt(string key)
        {
            key = GetKey(key);
            return EditorPrefs.GetInt(key);
        }
        public static int GetInt(string key, int defaultValue)
        {
            key = GetKey(key);
            return EditorPrefs.GetInt(key, defaultValue);
        }

        public static float GetFloat(string key)
        {
            key = GetKey(key);
            return EditorPrefs.GetFloat(key);
        }
        public static float GetFloat(string key, float defaultValue)
        {
            key = GetKey(key);
            return EditorPrefs.GetFloat(key, defaultValue);
        }

        public static string GetString(string key)
        {
            key = GetKey(key);
            return EditorPrefs.GetString(key);
        }
        public static string GetString(string key, string defaultValue)
        {
            key = GetKey(key);
            return EditorPrefs.GetString(key, defaultValue);
        }

        public static void SetBool(string key, bool value)
        {
            key = GetKey(key);
            EditorPrefs.SetBool(key, value);
        }

        public static void SetInt(string key, int value)
        {
            key = GetKey(key);
            EditorPrefs.SetInt(key, value);
        }

        public static void SetFloat(string key, float value)
        {
            key = GetKey(key);
            EditorPrefs.SetFloat(key, value);
        }

        public static void SetString(string key, string value)
        {
            key = GetKey(key);
            EditorPrefs.SetString(key, value);
        }

        #endregion

        #region Utils

        private static string GetKey(string id)
        {
            return _projectId + "." + id;
        }

        private static IEnumerable<string> GetAllKeys()
        {
            if (string.IsNullOrEmpty(_projectId)) yield break;

            var prefsKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Unity Technologies\Unity Editor 4.x");
            foreach (var skey in prefsKey.GetValueNames())
            {
                if (skey.StartsWith(_projectId)) yield return skey;
            }
        }

        #endregion

    }

}