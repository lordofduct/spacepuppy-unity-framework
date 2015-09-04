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

        public const string PROP_SECONDS = "_seconds";
        public const string PROP_TIMESUPPLIERTYPE = "_timeSupplierType";
        public const string PROP_TIMESUPPLIERNAME = "_timeSupplierName";

        private TimeUnitsSelectorPropertyDrawer _timeDrawer = new TimeUnitsSelectorPropertyDrawer();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            this.DrawTimePeriodSansLabel(position, property);
        }

        protected virtual void DrawTimePeriodSansLabel(Rect position, SerializedProperty property)
        {
            var secondsProp = property.FindPropertyRelative(PROP_SECONDS);
            var w = position.width / 3f;
            var attrib = this.fieldInfo.GetCustomAttributes(typeof(TimePeriod.Config), false).FirstOrDefault() as TimePeriod.Config;
            var availNames = (attrib != null) ? attrib.AvailableCustomTimeNames : null;

            if (w > 75f)
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, Mathf.Min(w, 150f));
                position = _timeDrawer.DrawUnits(position, secondsProp, 75f);
                position = TimePeriodPropertyDrawer.DrawTimeSupplier(position, property, position.width, availNames);
            }
            else
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, w);
                position = _timeDrawer.DrawUnits(position, secondsProp, w);


                position = TimePeriodPropertyDrawer.DrawTimeSupplier(position, property, position.width, availNames); //we mirror the SPTime prop drawer, we can do this because the property names are identical
            }
        }

        public static Rect DrawTimeSupplier(Rect position, SerializedProperty property, float desiredWidth, string[] availableNames)
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

            lst.Release();

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }


    }
}
