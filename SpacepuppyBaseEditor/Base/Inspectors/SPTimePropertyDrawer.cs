using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy;
using com.spacepuppy.Project;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(SPTime))]
    public class SPTimePropertyDrawer : PropertyDrawer
    {

        public const string PROP_TIMESUPPLIERTYPE = "_timeSupplierType";
        public const string PROP_TIMESUPPLIERNAME = "_timeSupplierName";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(SPTime.Config), false).FirstOrDefault() as SPTime.Config;
            var availNames = (attrib != null) ? attrib.AvailableCustomTimeNames : null;

            SPTimePropertyDrawer.DrawTimeSupplier(position, property, position.width, availNames); 
        }



        public static Rect DrawTimeSupplier(Rect position, SerializedProperty property, float desiredWidth, string[] availableNames)
        {
            if (position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);

            var tsTypeProp = property.FindPropertyRelative(PROP_TIMESUPPLIERTYPE);
            var tsNameProp = property.FindPropertyRelative(PROP_TIMESUPPLIERNAME);

            int index = -1;
            using (var lst = TempCollection<string>.GetCollection())
            {
                lst.Add("Normal");
                lst.Add("Real");
                lst.Add("Smooth");

                foreach (var nm in CustomTimeLayersData.Layers)
                {
                    if (!lst.Contains(nm)) lst.Add(nm);
                }

                if (availableNames != null)
                {
                    foreach (var nm in availableNames)
                    {
                        if (!lst.Contains(nm)) lst.Add(nm);
                    }
                }

                var e = tsTypeProp.GetEnumValue<DeltaTimeType>();
                if (e == DeltaTimeType.Custom)
                {
                    index = lst.IndexOf(tsNameProp.stringValue);
                    if (index < 0)
                    {
                        tsTypeProp.SetEnumValue(DeltaTimeType.Normal);
                        tsNameProp.stringValue = null;
                        index = 0;
                    }
                }
                else
                    index = (int)e;

                if (Application.isPlaying) GUI.enabled = false;
                EditorGUI.BeginChangeCheck();
                index = Mathf.Max(EditorGUI.Popup(position, index, lst.ToArray()), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    if (index < 3)
                    {
                        tsTypeProp.SetEnumValue((DeltaTimeType)index);
                        tsNameProp.stringValue = null;
                    }
                    else
                    {
                        tsTypeProp.SetEnumValue(DeltaTimeType.Custom);
                        tsNameProp.stringValue = lst[index];
                    }
                }
                if (Application.isPlaying) GUI.enabled = true;
            }

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }


    }

}
