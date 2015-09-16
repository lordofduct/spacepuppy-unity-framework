using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    public static class EditorProjectPrefs
    {

        #region Static Interface

        private const string PREFS_DIR = @"Editor";
        private const string PREFS_PATH = PREFS_DIR + @"/Spacepuppy.EditorProjectPrefs.xml";
        private static string _path;
        private static XDocument _xdoc;
        private static string _projectId;

        private static LocalSettings _local;
        private static GroupSettings _group;

        static EditorProjectPrefs()
        {
            _path = System.IO.Path.Combine(Application.dataPath, PREFS_PATH);
            try
            {
                _xdoc = XDocument.Load(_path);
            }
            catch
            {
                _xdoc = null;
            }

            if(_xdoc == null)
            {
                _projectId = "SPProj." + ShortGuid.NewGuid().ToString();
                _xdoc = new XDocument(new XElement("root"));
                _xdoc.Root.Add(new XAttribute("projectId", _projectId));

                var sdir = System.IO.Path.GetDirectoryName(_path);
                if (!System.IO.Directory.Exists(sdir)) System.IO.Directory.CreateDirectory(sdir);
                _xdoc.Save(_path);
            }
            else
            {
                var xattrib = _xdoc.Root.Attribute("projectId");
                if (xattrib == null)
                {
                    xattrib = new XAttribute("projectId", "SPProj." + ShortGuid.NewGuid().ToString());
                    _xdoc.Root.Add(xattrib);
                    _xdoc.Save(_path);
                }
                else if (string.IsNullOrEmpty(xattrib.Value))
                {
                    xattrib.Value = "SPProj." + ShortGuid.NewGuid().ToString();
                    _xdoc.Save(_path);
                }
                _projectId = xattrib.Value;
            }

            _local = new LocalSettings();
            _group = new GroupSettings();
        }

        public static LocalSettings Local { get { return _local; } }

        public static GroupSettings Group { get { return _group; } }

        #endregion

        public class LocalSettings
        {

            public void DeleteAll()
            {
                foreach (var skey in GetAllKeys())
                {
                    EditorPrefs.DeleteKey(skey);
                }
            }

            public void DeleteKey(string key)
            {
                key = GetKey(key);
                EditorPrefs.DeleteKey(key);
            }

            public bool HasKey(string key)
            {
                key = GetKey(key);
                return EditorPrefs.HasKey(key);
            }

            public bool GetBool(string key)
            {
                key = GetKey(key);
                return EditorPrefs.GetBool(key);
            }
            public bool GetBool(string key, bool defaultValue)
            {
                key = GetKey(key);
                return EditorPrefs.GetBool(key, defaultValue);
            }

            public int GetInt(string key)
            {
                key = GetKey(key);
                return EditorPrefs.GetInt(key);
            }
            public int GetInt(string key, int defaultValue)
            {
                key = GetKey(key);
                return EditorPrefs.GetInt(key, defaultValue);
            }

            public float GetFloat(string key)
            {
                key = GetKey(key);
                return EditorPrefs.GetFloat(key);
            }
            public float GetFloat(string key, float defaultValue)
            {
                key = GetKey(key);
                return EditorPrefs.GetFloat(key, defaultValue);
            }

            public string GetString(string key)
            {
                key = GetKey(key);
                return EditorPrefs.GetString(key);
            }
            public string GetString(string key, string defaultValue)
            {
                key = GetKey(key);
                return EditorPrefs.GetString(key, defaultValue);
            }

            public T GetEnum<T>(string key) where T : struct, System.IConvertible
            {
                key = GetKey(key);
                int i = EditorPrefs.GetInt(key);
                return ConvertUtil.ToEnum<T>(i);
            }

            public T GetEnum<T>(string key, T defaultValue) where T : struct, System.IConvertible
            {
                key = GetKey(key);
                int i = EditorPrefs.GetInt(key, System.Convert.ToInt32(defaultValue));
                return ConvertUtil.ToEnum<T>(i, defaultValue);
            }

            public void SetBool(string key, bool value)
            {
                key = GetKey(key);
                EditorPrefs.SetBool(key, value);
            }

            public void SetInt(string key, int value)
            {
                key = GetKey(key);
                EditorPrefs.SetInt(key, value);
            }

            public void SetFloat(string key, float value)
            {
                key = GetKey(key);
                EditorPrefs.SetFloat(key, value);
            }

            public void SetString(string key, string value)
            {
                key = GetKey(key);
                EditorPrefs.SetString(key, value);
            }

            public void SetEnum<T>(string key, T value) where T : struct, System.IConvertible
            {
                key = GetKey(key);
                EditorPrefs.SetInt(key, System.Convert.ToInt32(value));
            }

            #region Utils

            private string GetKey(string id)
            {
                return _projectId + "." + id;
            }

            private IEnumerable<string> GetAllKeys()
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

        public class GroupSettings
        {

            private const string NODE_NAME = "setting";

            public void Save()
            {
                _xdoc.Save(_path);
            }


            public void DeleteAll()
            {
                _xdoc.Root.Elements().Remove();
            }

            public void DeleteKey(string key)
            {
                _xdoc.Root.Elements(key).Remove();
            }

            public bool HasKey(string key)
            {
                return _xdoc.Root.Elements(key).Count() > 0;
            }

            public bool GetBool(string key)
            {
                return this.GetBool(key, false);
            }
            public bool GetBool(string key, bool defaultValue)
            {
                var xel = (from x in _xdoc.Root.Elements(NODE_NAME) where x.Attribute("id").Value == key select x).FirstOrDefault();
                if (xel == null) return defaultValue;
                var xattrib = xel.Attribute("value");
                return (xattrib != null) ? ConvertUtil.ToBool(xel.Attribute("value").Value) : defaultValue;
            }

            public int GetInt(string key)
            {
                return this.GetInt(key, 0);
            }
            public int GetInt(string key, int defaultValue)
            {
                var xel = (from x in _xdoc.Root.Elements(NODE_NAME) where x.Attribute("id").Value == key select x).FirstOrDefault();
                if (xel == null) return defaultValue;
                var xattrib = xel.Attribute("value");
                return (xattrib != null) ? ConvertUtil.ToInt(xel.Attribute("value").Value) : defaultValue;
            }

            public float GetFloat(string key)
            {
                return this.GetFloat(key, 0f);
            }
            public float GetFloat(string key, float defaultValue)
            {
                var xel = (from x in _xdoc.Root.Elements(NODE_NAME) where x.Attribute("id").Value == key select x).FirstOrDefault();
                if (xel == null) return defaultValue;
                var xattrib = xel.Attribute("value");
                return (xattrib != null) ? ConvertUtil.ToSingle(xel.Attribute("value").Value) : defaultValue;
            }

            public string GetString(string key)
            {
                return this.GetString(key, string.Empty);
            }
            public string GetString(string key, string defaultValue)
            {
                var xel = (from x in _xdoc.Root.Elements(NODE_NAME) where x.Attribute("id").Value == key select x).FirstOrDefault();
                if (xel == null) return defaultValue;
                var xattrib = xel.Attribute("value");
                return (xattrib != null) ? xel.Attribute("value").Value : defaultValue;
            }

            public void SetBool(string key, bool value)
            {
                this.SetValue(key, value);
            }

            public void SetInt(string key, int value)
            {
                this.SetValue(key, value);
            }

            public void SetFloat(string key, float value)
            {
                this.SetValue(key, value);
            }

            public void SetString(string key, string value)
            {
                this.SetValue(key, value);
            }



            private void SetValue(string key, object value)
            {
                var sval = StringUtil.ToLower(ConvertUtil.ToString(value));

                var xel = (from x in _xdoc.Root.Elements(NODE_NAME) where x.Attribute("id").Value == key select x).FirstOrDefault();
                if (xel == null)
                {
                    xel = new XElement(NODE_NAME, new XAttribute("id", key), new XAttribute("value", sval));
                    _xdoc.Root.Add(xel);
                }
                else
                {
                    var xattrib = xel.Attribute("value");
                    if (xattrib != null)
                        xattrib.Value = sval;
                    else
                        xel.Add(new XAttribute("value", sval));
                }
            }

        }

    }

}