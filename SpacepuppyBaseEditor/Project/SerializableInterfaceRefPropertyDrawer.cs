using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Project
{

    [CustomPropertyDrawer(typeof(BaseSerializableInterfaceRef), true)]
    public class SerializableInterfaceRefPropertyDrawer : PropertyDrawer
    {

        public const string PROP_OBJ = "_obj";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tp = (this.fieldInfo != null) ? this.fieldInfo.FieldType : null;
            var objProp = property.FindPropertyRelative(PROP_OBJ);
            if (tp == null || objProp == null || objProp.propertyType != SerializedPropertyType.ObjectReference)
            {
                this.DrawMalformed(position);
                return;
            }
            
            var valueType = com.spacepuppy.Dynamic.DynamicUtil.GetReturnType(DynamicUtil.GetMemberFromType(tp, "_value", true));
            if(valueType == null || !(valueType.IsClass || valueType.IsInterface))
            {
                this.DrawMalformed(position);
                return;
            }

            //var val = EditorGUI.ObjectField(position, label, objProp.objectReferenceValue, valueType, true);
            var val = EditorGUI.ObjectField(position, label, objProp.objectReferenceValue, typeof(UnityEngine.Object), true);
            if (val != null && !TypeUtil.IsType(val.GetType(), valueType)) val = null;
            objProp.objectReferenceValue = val as UnityEngine.Object;
        }

        private void DrawMalformed(Rect position)
        {
            EditorGUI.LabelField(position, "Malformed SerializedInterfaceRef.");
            Debug.LogError("Malformed SerializedInterfaceRef - make sure you inherit from 'SerializableInterfaceRef'.");
        }

    }

}
