using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Modifiers
{

    [CustomPropertyDrawer(typeof(ForceFromSelfAttribute))]
    public class ForceFromSelfModifier : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property)
        {
            var relativity = (this.attribute as ForceFromSelfAttribute).Relativity;

            if (property.isArray && TypeUtil.IsListType(fieldInfo.FieldType, true))
            {
                var fieldType = TypeUtil.GetElementTypeOfListType(this.fieldInfo.FieldType);
                for(int i = 0; i < property.arraySize; i++)
                {
                    this.ApplyDefaultAsSingle(property.GetArrayElementAtIndex(i), fieldType, relativity);
                }
            }
            else
            {
                this.ApplyDefaultAsSingle(property, this.fieldInfo.FieldType, relativity);
            }
        }
        
        private void ApplyDefaultAsSingle(SerializedProperty property, System.Type fieldType, EntityRelativity relativity)
        {
            if (object.ReferenceEquals(property.objectReferenceValue, null)) return;

            var self = GameObjectUtil.GetGameObjectFromSource(property.serializedObject.targetObject);
            if (object.ReferenceEquals(self, null)) return;

            var go = GameObjectUtil.GetGameObjectFromSource(property.objectReferenceValue);
            if (object.ReferenceEquals(go, null)) return;


            switch(relativity)
            {
                case EntityRelativity.Entity:
                    if(go.FindRoot() != self.FindRoot())
                    {
                        property.objectReferenceValue = null;
                    }
                    break;
                case EntityRelativity.Self:
                    if(go != self)
                    {
                        property.objectReferenceValue = null;
                    }
                    break;
                case EntityRelativity.SelfAndChildren:
                    if(go != self && !self.IsParentOf(go))
                    {
                        property.objectReferenceValue = null;
                    }
                    break;
            }
        }

    }

}
