using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim
{

    [CustomEditor(typeof(SPAnimationMaskAsset), true)]
    public class SPAnimationMaskAssetInspector : SPEditor
    {

        public const string PROP_AVATAR = "_avatar";
        public const string PROP_MASKS = "_masks";
        public const string PROP_ENTRY_PATH = "Path";
        public const string PROP_ENTRY_RECURSE = "Recurse";

        const float RECURSE_TOGGLE_WIDTH = 64f;
        const float RECURSE_TOGGLE_PAD = 2f;
        
        private ReorderableList _lstDrawer;
        private bool _lstElementReadonly;


        protected override void OnEnable()
        {
            base.OnEnable();

            _lstDrawer = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty(PROP_MASKS))
            {
                draggable = true,
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = _lstDrawer_DrawHeader,
                drawElementCallback = _lstDrawer_DrawElement
            };
        }

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            var avatarProp = this.serializedObject.FindProperty(PROP_AVATAR);
            
            avatarProp.objectReferenceValue = EditorGUILayout.ObjectField("Avatar", avatarProp.objectReferenceValue, typeof(Transform), false);

            EditorGUILayout.HelpBox("Add bones to define what bones are included in the animation.\n\nAn empty mask means all bones play.", MessageType.Info);

            var avatar = avatarProp.objectReferenceValue as Transform;
            var expanded = GetExpandedHierarchy(avatar);
            if (avatar == null)
            {
                _lstDrawer.draggable = true;
                _lstDrawer.displayAdd = true;
                _lstDrawer.displayRemove = true;
                _lstElementReadonly = false;
            }
            else
            {
                if (!expanded.Contains(avatar)) expanded.Add(avatar);

                _lstDrawer.draggable = true;
                _lstDrawer.displayAdd = false;
                _lstDrawer.displayRemove = true;
                _lstElementReadonly = true;
            }

            _lstDrawer.DoLayoutList();

            this.DrawTree(avatar);
            


            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_AVATAR, PROP_MASKS);
            this.serializedObject.ApplyModifiedProperties();
        }
        
        #region List Drawer Methods

        private void _lstDrawer_DrawHeader(Rect area)
        {
            EditorGUI.LabelField(area, "Masks");
        }

        private void _lstDrawer_DrawElement(Rect area, int index, bool isActive, bool isFocused)
        {
            var prop = _lstDrawer.serializedProperty.GetArrayElementAtIndex(index);
            var r0 = new Rect(area.xMin, area.yMin, Mathf.Max(area.width - RECURSE_TOGGLE_WIDTH - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMax + RECURSE_TOGGLE_PAD, area.yMin, Mathf.Max(area.xMax - r0.xMax - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);

            var pathProp = prop.FindPropertyRelative(PROP_ENTRY_PATH);
            var recurseProp = prop.FindPropertyRelative(PROP_ENTRY_RECURSE);

            if (_lstElementReadonly)
                EditorGUI.SelectableLabel(r0, pathProp.stringValue);
            else
                pathProp.stringValue = EditorGUI.TextField(r0, pathProp.stringValue);
            recurseProp.boolValue = EditorGUI.ToggleLeft(r1, "Recurse", recurseProp.boolValue);
        }

        #endregion

        #region Tree Drawer Methods
        
        private Dictionary<Transform, bool> _masks = new Dictionary<Transform, bool>();

        private void DrawTree(Transform avatar)
        {
            EditorGUILayout.LabelField("Mask Tree", EditorStyles.boldLabel);

            _masks.Clear();
            for(int i = 0; i < _lstDrawer.serializedProperty.arraySize; i++)
            {
                var prop = _lstDrawer.serializedProperty.GetArrayElementAtIndex(i);
                var pathProp = prop.FindPropertyRelative(PROP_ENTRY_PATH);
                var recurseProp = prop.FindPropertyRelative(PROP_ENTRY_RECURSE);

                var t = avatar.Find(pathProp.stringValue);
                if (t != null) _masks[t] = recurseProp.boolValue;
            }

            EditorGUI.BeginChangeCheck();
            this.DrawTreeRecurse(avatar, avatar, false);
            
            if (EditorGUI.EndChangeCheck())
            {
                _lstDrawer.serializedProperty.arraySize = _masks.Count;
                int i = 0;
                foreach(var entry in _masks)
                {
                    var prop = _lstDrawer.serializedProperty.GetArrayElementAtIndex(i);
                    var pathProp = prop.FindPropertyRelative(PROP_ENTRY_PATH);
                    var recurseProp = prop.FindPropertyRelative(PROP_ENTRY_RECURSE);

                    pathProp.stringValue = GameObjectUtil.GetPathNameRelativeTo(entry.Key, avatar);
                    recurseProp.boolValue = entry.Value;

                    i++;
                }
            }
        }

        private void DrawTreeRecurse(Transform root, Transform trans, bool parentIsRecursingMask)
        {
            const float MASK_TOGGLE_WIDTH = 18f;

            var rect = EditorGUILayout.GetControlRect();
            var area = EditorGUI.IndentedRect(rect);
            area = new Rect(area.xMin + 2f, area.yMin, area.width - 2f, area.height);
            var frect = new Rect(rect.xMin, rect.yMin, area.xMin - rect.xMin, EditorGUIUtility.singleLineHeight);

            bool expanded = GetExpandedHierarchy(root).Contains(trans);
            if (trans.childCount > 0)
            {
                bool newValue = EditorGUI.Foldout(frect, expanded, GUIContent.none);
                if(expanded != newValue)
                {
                    if(newValue)
                        GetExpandedHierarchy(root).Add(trans);
                    else
                        GetExpandedHierarchy(root).Remove(trans);
                }
            }
            else if (expanded)
            {
                expanded = false;
                GetExpandedHierarchy(root).Remove(trans);
            }


            bool recurse;
            bool isMask = _masks.TryGetValue(trans, out recurse);

            //do draw
            int cache = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (root == trans)
            {
                var r0 = new Rect(area.xMin, area.yMin, Mathf.Max(area.width - RECURSE_TOGGLE_WIDTH - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);
                var r1 = new Rect(r0.xMax + RECURSE_TOGGLE_PAD, area.yMin, Mathf.Max(area.xMax - r0.xMax - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);
                
                EditorGUI.SelectableLabel(r0, trans.name);

                bool playAll = _masks.Count == 0;
                playAll = EditorGUI.ToggleLeft(r1, "Play All", playAll);
                if (playAll && _masks.Count > 0) _masks.Clear();

                _masks.Remove(trans);
            }
            else if(parentIsRecursingMask)
            {
                var r0 = new Rect(area.xMin, area.yMin, MASK_TOGGLE_WIDTH, EditorGUIUtility.singleLineHeight);
                var r1 = new Rect(r0.xMax, area.yMin, area.xMax - r0.xMax, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(r0, "X");
                EditorGUI.SelectableLabel(r1, trans.name, EditorStyles.boldLabel);
                _masks.Remove(trans);
            }
            else
            {
                var r0 = new Rect(area.xMin, area.yMin, MASK_TOGGLE_WIDTH, EditorGUIUtility.singleLineHeight);

                isMask = EditorGUI.Toggle(r0, isMask);

                if(isMask)
                {
                    var r1 = new Rect(r0.xMax, area.yMin, Mathf.Max(area.xMax - r0.xMax - RECURSE_TOGGLE_WIDTH - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);
                    var r2 = new Rect(r1.xMax, area.yMin, Mathf.Max(area.xMax - r1.xMax - RECURSE_TOGGLE_PAD, 0f), EditorGUIUtility.singleLineHeight);
                    
                    EditorGUI.SelectableLabel(r1, trans.name, isMask || parentIsRecursingMask ? EditorStyles.boldLabel : EditorStyles.label);

                    recurse = EditorGUI.ToggleLeft(r2, "Recurse", recurse);
                    _masks[trans] = recurse;
                }
                else
                {
                    var r1 = new Rect(r0.xMax, area.yMin, Mathf.Max(area.xMax - r0.xMax, 0f), EditorGUIUtility.singleLineHeight);
                    EditorGUI.SelectableLabel(r1, trans.name, isMask || parentIsRecursingMask ? EditorStyles.boldLabel : EditorStyles.label);
                    _masks.Remove(trans);
                }
            }

            EditorGUI.indentLevel = cache;


            //recurse
            if (expanded)
            {
                EditorGUI.indentLevel++;
                try
                {
                    foreach (Transform t in trans)
                    {
                        this.DrawTreeRecurse(root, t, (isMask && recurse) || parentIsRecursingMask);
                    }
                }
                finally
                {
                    EditorGUI.indentLevel--;
                }
            }
        }
        
        #endregion



        #region Static Utils

        private static Dictionary<Transform, List<Transform>> _expandedHierarchyListTable = new Dictionary<Transform, List<Transform>>();
        private static List<Transform> GetExpandedHierarchy(Transform t)
        {
            if (t == null) return null;

            List<Transform> result;
            if(!_expandedHierarchyListTable.TryGetValue(t, out result))
            {
                result = new List<Transform>();
                result.Add(t);
                _expandedHierarchyListTable[t] = result;
            }
            return result;
        }

        #endregion

    }

}
