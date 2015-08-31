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

        private const double DAYS_IN_YEAR = 365.0;
        public enum TimePeriodUnits
        {
            Seconds = 0,
            Minutes = 1,
            Hours = 2,
            Days = 3,
            Years = 4
        }
        private static Dictionary<int, TimePeriodUnits> _unitsCache = new Dictionary<int, TimePeriodUnits>();
        private static TimePeriodUnits GetUnits(SerializedProperty property)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            TimePeriodUnits units;
            if (_unitsCache.TryGetValue(hash, out units))
                return units;
            else
                return TimePeriodUnits.Seconds;
        }
        private static void SetUnits(SerializedProperty property, TimePeriodUnits units)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            _unitsCache[hash] = units;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            position = EditorGUI.PrefixLabel(position, label);
            
            var w = position.width / 3f;
            if(w > 75f)
            {
                position = this.DrawDuration(position, property, Mathf.Min(w, 150f));
                position = this.DrawUnits(position, property, 75f);
                position = this.DrawTimeSupplier(position, property, position.width);
            }
            else
            {
                position = this.DrawDuration(position, property, w);
                position = this.DrawUnits(position, property, w);
                position = this.DrawTimeSupplier(position, property, position.width);
            }
        }

        private Rect DrawDuration(Rect position, SerializedProperty property, float desiredWidth)
        {
            if(position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);
            
            var units = GetUnits(property);
            var durProp = property.FindPropertyRelative("_seconds");
            System.TimeSpan span = System.TimeSpan.FromSeconds(durProp.floatValue);
            double dur = 0.0;
            switch(units)
            {
                case TimePeriodUnits.Seconds:
                    dur = span.TotalSeconds;
                    break;
                case TimePeriodUnits.Minutes:
                    dur = span.TotalMinutes;
                    break;
                case TimePeriodUnits.Hours:
                    dur = span.TotalHours;
                    break;
                case TimePeriodUnits.Days:
                    dur = span.TotalDays;
                    break;
                case TimePeriodUnits.Years:
                    dur = span.Ticks / (System.TimeSpan.TicksPerDay * DAYS_IN_YEAR);
                    break;
            }
            EditorGUI.BeginChangeCheck();
            dur = EditorGUI.FloatField(r, (float)dur);
            if(EditorGUI.EndChangeCheck())
            {
                switch(units)
                {
                    case TimePeriodUnits.Seconds:
                        span = System.TimeSpan.FromSeconds(dur);
                        break;
                    case TimePeriodUnits.Minutes:
                        span = System.TimeSpan.FromMinutes(dur);
                        break;
                    case TimePeriodUnits.Hours:
                        span = System.TimeSpan.FromHours(dur);
                        break;
                    case TimePeriodUnits.Days:
                        span = System.TimeSpan.FromDays(dur);
                        break;
                    case TimePeriodUnits.Years:
                        span = System.TimeSpan.FromTicks((long)(dur * System.TimeSpan.TicksPerDay * DAYS_IN_YEAR));
                        break;
                }
                durProp.floatValue = (float)span.TotalSeconds;
            }

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }
        
        private Rect DrawUnits(Rect position, SerializedProperty property, float desiredWidth)
        {
            if(position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);
            
            var units = GetUnits(property);
            EditorGUI.BeginChangeCheck();
            units = (TimePeriodUnits)EditorGUI.EnumPopup(r, units);
            if(EditorGUI.EndChangeCheck())
                SetUnits(property, units);

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
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
