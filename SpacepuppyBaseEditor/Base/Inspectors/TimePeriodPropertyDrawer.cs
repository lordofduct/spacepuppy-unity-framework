using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(TimePeriod))]
    public class TimePeriodPropertyDrawer : PropertyDrawer
    {

        private TimeUnitsSelectorPropertyDrawer _timeDrawer = new TimeUnitsSelectorPropertyDrawer();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            position = EditorGUI.PrefixLabel(position, label);
            
            var secondsProp = property.FindPropertyRelative("_seconds");
            var w = position.width / 3f;
            if(w > 75f)
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, Mathf.Min(w, 150f));
                position = _timeDrawer.DrawUnits(position, secondsProp, 75f);
                position = this.DrawTimeSupplier(position, property, position.width);
            }
            else
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, w);
                position = _timeDrawer.DrawUnits(position, secondsProp, w);
                position = this.DrawTimeSupplier(position, property, position.width);
            }
        }

        private Rect DrawTimeSupplier(Rect position, SerializedProperty property, float desiredWidth)
        {
            if (position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);
            
            var tsTypeProp = property.FindPropertyRelative("_timeSupplierType");
            var tsNameProp = property.FindPropertyRelative("_timeSupplierName");

            int index = -1;
            var lst = TempCollection<string>.GetCollection();
            lst.Add("Normal");
            lst.Add("Real");
            lst.Add("Smooth");

            foreach(var nm in CustomTimeLayersData.Layers)
            {
                if (!lst.Contains(nm)) lst.Add(nm);
            }

            var attrib = this.fieldInfo.GetCustomAttributes(typeof(TimePeriod.Config), false).FirstOrDefault() as TimePeriod.Config;
            if(attrib != null)
            {
                foreach(var nm in attrib.AvailableCustomTimeNames)
                {
                    if (!lst.Contains(nm)) lst.Add(nm);
                }
            }
            else
            {
                var e = tsTypeProp.GetEnumValue<DeltaTimeType>();
                if (e == DeltaTimeType.Custom)
                {
                    index = lst.IndexOf(tsNameProp.stringValue);
                    if(index < 0)
                    {
                        tsTypeProp.SetEnumValue(DeltaTimeType.Normal);
                        tsNameProp.stringValue = null;
                        index = 0;
                    }
                }
                else
                    index = (int)e;
            }

            EditorGUI.BeginChangeCheck();
            index = Mathf.Max(EditorGUI.Popup(position, index, lst.ToArray()), 0);
            if(EditorGUI.EndChangeCheck())
            {
                if(index < 3)
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

            lst.Release();

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }

    }
}
