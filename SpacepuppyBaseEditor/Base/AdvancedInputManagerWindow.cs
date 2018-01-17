using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppyeditor;
using com.spacepuppyeditor.Internal;

namespace com.spacepuppyeditor.Base
{

    public class AdvancedInputManagerWindow : EditorWindow
    {

        #region Consts

        public const string MENU_NAME = SPMenu.MENU_NAME_SETTINGS + "/Advanced Input Manager";
        public const int MENU_PRIORITY = SPMenu.MENU_PRIORITY_SETTINGS;
        
        #endregion

        #region Menu Entries

        [MenuItem(MENU_NAME, priority = MENU_PRIORITY)]
        public static void OpenAdvancedInputManager()
        {
            if (_openWindow == null)
            {
                EditorWindow.GetWindow<AdvancedInputManagerWindow>();
            }
            else
            {
                GUI.BringWindowToFront(_openWindow.GetInstanceID());
            }
        }

        #endregion

        #region Window

        private static AdvancedInputManagerWindow _openWindow;

        private SerializedObject _inputManagerAsset;
        private SPReorderableList _axesList;


        private void OnEnable()
        {
            if (_openWindow == null)
                _openWindow = this;
            else
                Object.DestroyImmediate(this);

            this.titleContent = new GUIContent("Advanced Input Manager");

            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset").FirstOrDefault();
            if (asset != null)
            {
                _inputManagerAsset = new SerializedObject(asset);
                _axesList = new SPReorderableList(_inputManagerAsset, _inputManagerAsset.FindProperty(InputSettings.PROP_AXES));
                _axesList.elementHeight = EditorGUIUtility.singleLineHeight;
                _axesList.drawHeaderCallback = _axesList_DrawHeader;
                _axesList.drawElementCallback = _axesList_DrawElement;
            }
        }

        private void OnDisable()
        {
            if (_openWindow == this) _openWindow = null;
            if (_inputManagerAsset != null)
            {
                _inputManagerAsset.Dispose();
                _inputManagerAsset = null;
            }
        }

        private void OnGUI()
        {
            if (_inputManagerAsset == null)
            {
                EditorGUILayout.LabelField("NO INPUT MANAGER EXISTS.");
                return;
            }

            _inputManagerAsset.Update();
            EditorGUI.BeginChangeCheck();

            _axesList.DoLayoutList();
            this.DrawCurrentElementExtended();

            if (EditorGUI.EndChangeCheck())
                _inputManagerAsset.ApplyModifiedProperties();
        }

        private void DrawCurrentElementExtended()
        {
            if (_axesList.index < 0 || _axesList.index >= _axesList.serializedProperty.arraySize) return;

            var el = _axesList.serializedProperty.GetArrayElementAtIndex(_axesList.index);
            el.isExpanded = true;
            EditorGUILayout.PropertyField(el, true);
        }

        #endregion

        #region List Drawer Methods

        private void _axesList_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Inputs");
        }

        private void _axesList_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var el = _axesList.serializedProperty.GetArrayElementAtIndex(index);
            var nameProp = el.FindPropertyRelative("m_Name");
            EditorGUI.PropertyField(area, nameProp, EditorHelper.TempContent(string.Format("Input {0:00}", index)));
        }

        #endregion
        
    }

}
