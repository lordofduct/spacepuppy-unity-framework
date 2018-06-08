using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor
{

    [CreateAssetMenu(fileName = "InputSettings", menuName = "Spacepuppy/Input Settings", order = int.MaxValue)]
    public class InputSettings : ScriptableObject
    {
        
        #region Fields

        [SerializeField]
        private List<InputConfig> _entries = new List<InputConfig>();
        [System.NonSerialized]
        private NonNullList _protectedEntries;

        #endregion

        #region Properties

        public IList<InputConfig> Entries
        {
            get
            {
                if (_protectedEntries == null) _protectedEntries = new NonNullList(_entries);
                return _protectedEntries;
            }
        }

        #endregion

        #region Methods

        public bool ApplyToGlobal()
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset").FirstOrDefault();
            if (asset == null) return false;

            var settings = ScriptableObject.CreateInstance<InputSettings>();
            try
            {
                var serializedObject = new SerializedObject(asset);
                var axes = serializedObject.FindProperty(PROP_AXES);
                axes.arraySize = _entries.Count;

                for(int i = 0; i < _entries.Count; i++)
                {
                    var axis = axes.GetArrayElementAtIndex(i);
                    var config = _entries[i];
                    axis.FindPropertyRelative(PROP_TYPE).enumValueIndex = (int)config.Type;
                    axis.FindPropertyRelative(PROP_NAME).stringValue = config.Name;
                    axis.FindPropertyRelative(PROP_DESCRIPTIVENAME).stringValue = config.DescriptiveName;
                    axis.FindPropertyRelative(PROP_DESCRIPTIVENEGATIVENAME).stringValue = config.DescriptiveNameNegative;
                    axis.FindPropertyRelative(PROP_NEGATIVEBUTTON).stringValue = config.NegativeButton;
                    axis.FindPropertyRelative(PROP_POSITIVEBUTTON).stringValue = config.PositiveButton;
                    axis.FindPropertyRelative(PROP_ALTNEGATIVEBUTTON).stringValue = config.AltNegativeButton;
                    axis.FindPropertyRelative(PROP_ALTPOSITIVEBUTTON).stringValue = config.AltPositiveButton;
                    axis.FindPropertyRelative(PROP_GRAVITY).floatValue = config.Gravity;
                    axis.FindPropertyRelative(PROP_DEAD).floatValue = config.Dead;
                    axis.FindPropertyRelative(PROP_SENSITIVITY).floatValue = config.Sensitivity;
                    axis.FindPropertyRelative(PROP_SNAP).boolValue = config.Snap;
                    axis.FindPropertyRelative(PROP_INVERT).boolValue = config.Invert;
                    axis.FindPropertyRelative(PROP_AXIS).enumValueIndex = (int)config.Axis;
                    axis.FindPropertyRelative(PROP_JOYNUM).enumValueIndex = (int)config.JoyNum;
                }

                serializedObject.ApplyModifiedProperties();
                return true;
            }
            finally
            {
                ObjUtil.SmartDestroy(settings);
            }
        }

        public bool CopyFromGlobal(bool cleanInputConfigs = false)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset").FirstOrDefault();
            if (asset == null) return false;
            
            try
            {
                var serializedObject = new SerializedObject(asset);
                var axes = serializedObject.FindProperty(PROP_AXES);

                for (int i = 0; i < axes.arraySize; i++)
                {
                    var axis = axes.GetArrayElementAtIndex(i);
                    var config = new InputConfig()
                    {
                        Type = (InputType)axis.FindPropertyRelative(PROP_TYPE).enumValueIndex,
                        Name = axis.FindPropertyRelative(PROP_NAME).stringValue,
                        DescriptiveName = axis.FindPropertyRelative(PROP_DESCRIPTIVENAME).stringValue,
                        DescriptiveNameNegative = axis.FindPropertyRelative(PROP_DESCRIPTIVENEGATIVENAME).stringValue,
                        NegativeButton = axis.FindPropertyRelative(PROP_NEGATIVEBUTTON).stringValue,
                        PositiveButton = axis.FindPropertyRelative(PROP_POSITIVEBUTTON).stringValue,
                        AltNegativeButton = axis.FindPropertyRelative(PROP_ALTNEGATIVEBUTTON).stringValue,
                        AltPositiveButton = axis.FindPropertyRelative(PROP_ALTPOSITIVEBUTTON).stringValue,
                        Gravity = axis.FindPropertyRelative(PROP_GRAVITY).floatValue,
                        Dead = axis.FindPropertyRelative(PROP_DEAD).floatValue,
                        Sensitivity = axis.FindPropertyRelative(PROP_SENSITIVITY).floatValue,
                        Snap = axis.FindPropertyRelative(PROP_SNAP).boolValue,
                        Invert = axis.FindPropertyRelative(PROP_INVERT).boolValue,
                        Axis = (InputAxis)axis.FindPropertyRelative(PROP_AXIS).enumValueIndex,
                        JoyNum = (JoyNum)axis.FindPropertyRelative(PROP_JOYNUM).enumValueIndex
                    };
                    if (cleanInputConfigs) config.ClearAsType();

                    _entries.Add(config);
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an InputSettings object for the current configuration in the global unity InputSettings.
        /// 
        /// 'cleanInputConfigs' will clean up the input data pulled from the global settings, removing unnecessary 
        /// data for inputs that don't need said data (example Gravity only applies to button/key configs, so is 
        /// removed from non-button/key configs).
        /// </summary>
        /// <param name="cleanInputConfigs">Clean up the input data pulled from the global settings.</param>
        /// <returns></returns>
        public static InputSettings LoadGlobalInputSettings(bool cleanInputConfigs = false)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset").FirstOrDefault();
            if (asset == null) return null;

            var settings = ScriptableObject.CreateInstance<InputSettings>();
            if(!settings.CopyFromGlobal(cleanInputConfigs))
            {
                ObjUtil.SmartDestroy(settings);
                settings = null;
            }
            return settings;
        }

        #endregion

        #region Special Types

        private class NonNullList : IList<InputConfig>
        {
            private List<InputConfig> _lst;

            public NonNullList(List<InputConfig> lst)
            {
                _lst = lst;
            }




            public InputConfig this[int index]
            {
                get { return _lst[index]; }
                set
                {
                    if (value == null) throw new System.ArgumentNullException("value");
                    _lst[index] = value;
                }
            }

            public int Count {  get { return _lst.Count; } }

            public bool IsReadOnly { get { return false; } }

            public void Add(InputConfig item)
            {
                if (item == null) throw new System.ArgumentNullException("item");
                _lst.Add(item);
            }

            public void Clear()
            {
                _lst.Clear();
            }

            public bool Contains(InputConfig item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(InputConfig[] array, int arrayIndex)
            {
                _lst.CopyTo(array, arrayIndex);
            }

            public IEnumerator<InputConfig> GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            public int IndexOf(InputConfig item)
            {
                return _lst.IndexOf(item);
            }

            public void Insert(int index, InputConfig item)
            {
                if (item == null) throw new System.ArgumentNullException("item");
                _lst.Insert(index, item);
            }

            public bool Remove(InputConfig item)
            {
                return _lst.Remove(item);
            }

            public void RemoveAt(int index)
            {
                _lst.RemoveAt(index);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }
        }

        #endregion

        #region Special Serialization Types

        public const string PROP_AXES = "m_Axes";
        public const string PROP_NAME = "m_Name";
        public const string PROP_DESCRIPTIVENAME = "descriptiveName";
        public const string PROP_DESCRIPTIVENEGATIVENAME = "descriptiveNegativeName";
        public const string PROP_NEGATIVEBUTTON = "negativeButton";
        public const string PROP_POSITIVEBUTTON = "positiveButton";
        public const string PROP_ALTNEGATIVEBUTTON = "altNegativeButton";
        public const string PROP_ALTPOSITIVEBUTTON = "altPositiveButton";
        public const string PROP_GRAVITY = "gravity";
        public const string PROP_DEAD = "dead";
        public const string PROP_SENSITIVITY = "sensitivity";
        public const string PROP_SNAP = "snap";
        public const string PROP_INVERT = "invert";
        public const string PROP_TYPE = "type";
        public const string PROP_AXIS = "axis";
        public const string PROP_JOYNUM = "joyNum";

        public enum InputType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        }

        public enum InputAxis
        {
            Xaxis = 0,
            Yaxis = 1,
            _3rd = 2,
            _4th = 3,
            _5th = 4,
            _6th = 5,
            _7th = 6,
            _8th = 7,
            _9th = 8,
            _10th = 9,
            _11th = 10,
            _12th = 11,
            _13th = 12,
            _14th = 13,
            _15th = 14,
            _16th = 15,
            _17th = 16,
            _18th = 17,
            _19th = 18,
            _20th = 19,
            _21st = 20,
            _22nd = 21,
            _23rd = 22,
            _24th = 23,
            _25th = 24,
            _26th = 25,
            _27th = 26,
            _28th = 27
        }

        public enum JoyNum
        {
            All = 0,
            Joystick1,
            Joystick2,
            Joystick3,
            Joystick4,
            Joystick5,
            Joystick6,
            Joystick7,
            Joystick8,
            Joystick9,
            Joystick10,
            Joystick11
        }

        [System.Serializable]
        public class InputConfig
        {


            public InputType Type;
            public string Name;
            public string DescriptiveName;
            public string DescriptiveNameNegative; //button only
            public string NegativeButton; //button only
            public string PositiveButton; //button only
            public string AltNegativeButton; //button only
            public string AltPositiveButton; //button only
            public float Gravity; //button only
            public float Dead;
            public float Sensitivity = 0.1f;
            public bool Snap; //button only
            public bool Invert;
            public InputAxis Axis;
            
            public JoyNum JoyNum;

            #region Methods

            internal void ClearAsType()
            {
                switch (this.Type)
                {
                    case InputType.KeyOrMouseButton:
                        this.ClearAsButton();
                        break;
                    case InputType.MouseMovement:
                        this.ClearAsMouseMovement();
                        break;
                    case InputType.JoystickAxis:
                        this.ClearAsJoyAxis();
                        break;
                }
            }

            internal void ClearAsButton()
            {
                Axis = InputAxis.Xaxis;
            }

            internal void ClearAsMouseMovement()
            {
                DescriptiveNameNegative = null;
                NegativeButton = null;
                PositiveButton = null;
                AltNegativeButton = null;
                AltPositiveButton = null;
                Gravity = 0f;
                Snap = false;
            }

            internal void ClearAsJoyAxis()
            {
                DescriptiveNameNegative = null;
                NegativeButton = null;
                PositiveButton = null;
                AltNegativeButton = null;
                AltPositiveButton = null;
                Gravity = 0f;
                Snap = false;
            }

            #endregion

            #region Serialized Property Fields
            
            public void ApplyToSerializedProperty(SerializedProperty prop)
            {
                SerializedProperty p;
                p = prop.FindPropertyRelative(PROP_NAME);
                if (p != null) p.stringValue = this.Name;
                p = prop.FindPropertyRelative(PROP_DESCRIPTIVENAME);
                if (p != null) p.stringValue = this.DescriptiveName;
                p = prop.FindPropertyRelative(PROP_DESCRIPTIVENEGATIVENAME);
                if (p != null) p.stringValue = this.DescriptiveNameNegative;
                p = prop.FindPropertyRelative(PROP_NEGATIVEBUTTON);
                if (p != null) p.stringValue = this.NegativeButton;
                p = prop.FindPropertyRelative(PROP_POSITIVEBUTTON);
                if (p != null) p.stringValue = this.PositiveButton;
                p = prop.FindPropertyRelative(PROP_ALTNEGATIVEBUTTON);
                if (p != null) p.stringValue = this.AltNegativeButton;
                p = prop.FindPropertyRelative(PROP_ALTPOSITIVEBUTTON);
                if (p != null) p.stringValue = this.AltPositiveButton;
                p = prop.FindPropertyRelative(PROP_GRAVITY);
                if (p != null) p.floatValue = this.Gravity;
                p = prop.FindPropertyRelative(PROP_DEAD);
                if (p != null) p.floatValue = this.Dead;
                p = prop.FindPropertyRelative(PROP_SENSITIVITY);
                if (p != null) p.floatValue = this.Sensitivity;
                p = prop.FindPropertyRelative(PROP_SNAP);
                if (p != null) p.boolValue = this.Snap;
                p = prop.FindPropertyRelative(PROP_INVERT);
                if (p != null) p.boolValue = this.Invert;
                p = prop.FindPropertyRelative(PROP_TYPE);
                if (p != null) p.enumValueIndex = (int)this.Type;
                p = prop.FindPropertyRelative(PROP_AXIS);
                if (p != null) p.enumValueIndex = (int)this.Axis;
                p = prop.FindPropertyRelative(PROP_JOYNUM);
                if (p != null) p.enumValueIndex = (int)this.JoyNum;
            }

            #endregion

        }

        #endregion

        #region Static Utils

        private static System.Func<string[]> _overrideGetGlobalInputIds;
        /// <summary>
        /// Get the input id's that should show up in a drop down list for any InputID property. This includes in the t_OnSimpleButtonPress editor.
        /// 
        /// Set this delegate to override the default entries as defined in InputManager.asset. This can be useful if you're using a custom input system 
        /// that has named inputs unique from what is found in the InputSettings.asset configuration.
        /// </summary>
        public static System.Func<string[]> GetGlobalInputIds
        {
            get
            {
                if (_overrideGetGlobalInputIds == null) _overrideGetGlobalInputIds = GetGlobalInputIdsDefault;
                return _overrideGetGlobalInputIds;
            }
            set
            {
                _overrideGetGlobalInputIds = value;
            }
        }

        public static string[] GetGlobalInputIdsDefault()
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset").FirstOrDefault();
            if (asset != null)
            {
                var obj = new SerializedObject(asset);
                var axes = obj.FindProperty(PROP_AXES);
                string[] arr = new string[axes.arraySize];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = axes.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue;
                }
                obj.Dispose();
                return arr;
            }
            else
            {
                return com.spacepuppy.Utils.ArrayUtil.Empty<string>();
            }
        }

        #endregion

    }

    [CustomEditor(typeof(InputSettings), true)]
    public class InputSettingsEditor : SPEditor
    {

        public const string PROP_ENTRIES = "_entries";
        public const string PROP_TYPE = "Type";
        public const string PROP_NAME = "Name";
        public const string PROP_DESCRIPTIVENAME = "DescriptiveName";
        public const string PROP_DESCRIPTIVENAMENEGATIVE = "DescriptiveNameNegative";
        public const string PROP_NEGATIVEBUTTON = "NegativeButton";
        public const string PROP_POSITIVEBUTTON = "PositiveButton";
        public const string PROP_ALTNEGATIVEBUTTON = "AltNegativeButton";
        public const string PROP_ALTPOSITIVEBUTTON = "AltPositiveButton";
        public const string PROP_GRAVITY = "Gravity";
        public const string PROP_DEAD = "Dead";
        public const string PROP_SENSITIVITY = "Sensitivity";
        public const string PROP_SNAP = "Snap";
        public const string PROP_INVERT = "Invert";
        public const string PROP_AXIS = "Axis";
        public const string PROP_JOYNUM = "JoyNum";

        #region Fields

        private SPReorderableList _entryList;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            _entryList = new SPReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_ENTRIES));
            _entryList.drawHeaderCallback = this._entryList_DrawHeader;
            _entryList.drawElementCallback = this._entryList_DrawElement;
        }

        #endregion

        #region GUI

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            _entryList.DoLayoutList();
            this.DrawDetailArea();

            this.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();


            const float R1_WIDTH = 90f;
            const float R2_WIDTH = 90f;
            const float PADDING = 5f;
            var rect = EditorGUILayout.GetControlRect();
            var r1 = new Rect(Mathf.Max(0f, rect.xMax - R1_WIDTH), rect.yMin, Mathf.Min(rect.width, R1_WIDTH), rect.height);
            var r2 = new Rect(Mathf.Max(0f, r1.xMin - PADDING - R2_WIDTH), rect.yMin, Mathf.Min(Mathf.Max(0f, rect.width - PADDING - R1_WIDTH), R2_WIDTH), rect.height);

            if(GUI.Button(r1, "Apply Global"))
            {
                if(EditorUtility.DisplayDialog("Apply Settings to Global Input Manager?", "Applying these settings will overwrite the global input settings.", "Apply", "Cancel"))
                {
                    var obj = this.serializedObject.targetObject as InputSettings;
                    if(obj != null)
                    {
                        obj.ApplyToGlobal();
                    }
                }
            }

            if(GUI.Button(r2, "Load Global"))
            {
                if (EditorUtility.DisplayDialog("Load Global Input Manager Settings?", "Loading Global Input Manager Settings will overwrite all entries in this config.", "Load", "Cancel"))
                {
                    var obj = this.serializedObject.targetObject as InputSettings;
                    if (obj != null)
                    {
                        Undo.RecordObject(obj, "Load Global InputSettings to InputSettings Asset");
                        obj.CopyFromGlobal(true);
                        EditorUtility.SetDirty(obj);
                        AssetDatabase.SaveAssets();
                        Selection.activeObject = obj;
                        this.serializedObject.Update();
                    }
                }
            }
        }



        private void DrawDetailArea()
        {
            EditorGUILayout.BeginVertical("Box");
            if (_entryList.index >= 0)
            {
                var element = _entryList.serializedProperty.GetArrayElementAtIndex(_entryList.index);

                var propType = element.FindPropertyRelative(PROP_TYPE);
                
                SPEditorGUILayout.PropertyField(propType);
                SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_NAME));
                SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_DESCRIPTIVENAME));

                switch ((InputSettings.InputType)propType.enumValueIndex)
                {
                    case InputSettings.InputType.KeyOrMouseButton:
                        {
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_DESCRIPTIVENAMENEGATIVE));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_NEGATIVEBUTTON));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_POSITIVEBUTTON));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_ALTNEGATIVEBUTTON));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_ALTPOSITIVEBUTTON));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_GRAVITY));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_DEAD));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_SENSITIVITY));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_SNAP));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_INVERT));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_JOYNUM));

                            element.FindPropertyRelative(PROP_AXIS).enumValueIndex = 0;
                        }
                        break;
                    case InputSettings.InputType.MouseMovement:
                    case InputSettings.InputType.JoystickAxis:
                        {
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_AXIS));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_DEAD));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_SENSITIVITY));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_INVERT));
                            SPEditorGUILayout.PropertyField(element.FindPropertyRelative(PROP_JOYNUM));

                            element.FindPropertyRelative(PROP_DESCRIPTIVENAMENEGATIVE).stringValue = string.Empty;
                            element.FindPropertyRelative(PROP_NEGATIVEBUTTON).stringValue = string.Empty;
                            element.FindPropertyRelative(PROP_POSITIVEBUTTON).stringValue = string.Empty;
                            element.FindPropertyRelative(PROP_ALTNEGATIVEBUTTON).stringValue = string.Empty;
                            element.FindPropertyRelative(PROP_ALTPOSITIVEBUTTON).stringValue = string.Empty;
                            element.FindPropertyRelative(PROP_GRAVITY).floatValue = 0f;
                            element.FindPropertyRelative(PROP_SNAP).boolValue = false;
                        }
                        break;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select a target to edit.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void _entryList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Entries");
        }

        private void _entryList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var element = _entryList.serializedProperty.GetArrayElementAtIndex(index);

            var propName = element.FindPropertyRelative(PROP_NAME);
            var propDesc = element.FindPropertyRelative(PROP_DESCRIPTIVENAME);

            EditorGUI.LabelField(area, propName.stringValue, propDesc.stringValue);
        }

        #endregion

    }

}
