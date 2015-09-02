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
    [CustomPropertyDrawer(typeof(TimeUnitsSelectorAttribute))]
    public class TimeUnitsSelectorPropertyDrawer : PropertyDrawer
    {

        #region Static Interface

        private static Dictionary<int, TimePeriod.Units> _unitsCache = new Dictionary<int, TimePeriod.Units>();
        private static TimePeriod.Units GetUnits(SerializedProperty property, TimeUnitsSelectorAttribute attrib)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            TimePeriod.Units units;
            if (_unitsCache.TryGetValue(hash, out units))
                return units;
            else
            {
                if (attrib == null)
                    return TimePeriod.Units.Seconds;
                else
                    return attrib.DefaultUnits;
            }
        }
        private static void SetUnits(SerializedProperty property, TimePeriod.Units units)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            _unitsCache[hash] = units;
        }

        public static double DAYS_IN_YEAR
        {
            get
            {
                //TODO - allow configuring at runtime
                return 365d;
            }
        }

        #endregion



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            position = EditorGUI.PrefixLabel(position, label);

            var w = position.width / 2f;
            if (w > 75f)
            {
                position = this.DrawDuration(position, property, position.width - 75f);
                position = this.DrawUnits(position, property, 75f);
            }
            else
            {
                position = this.DrawDuration(position, property, w);
                position = this.DrawUnits(position, property, w);
            }
        }

        public Rect DrawDuration(Rect position, SerializedProperty property, float desiredWidth)
        {
            if (position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);

            var units = GetUnits(property, this.attribute as TimeUnitsSelectorAttribute);
            System.TimeSpan span = System.TimeSpan.FromSeconds(property.floatValue);
            double dur = 0.0;
            switch (units)
            {
                case TimePeriod.Units.Seconds:
                    dur = span.TotalSeconds;
                    break;
                case TimePeriod.Units.Minutes:
                    dur = span.TotalMinutes;
                    break;
                case TimePeriod.Units.Hours:
                    dur = span.TotalHours;
                    break;
                case TimePeriod.Units.Days:
                    dur = span.TotalDays;
                    break;
                case TimePeriod.Units.Years:
                    dur = span.Ticks / (System.TimeSpan.TicksPerDay * TimeUnitsSelectorPropertyDrawer.DAYS_IN_YEAR);
                    break;
            }
            EditorGUI.BeginChangeCheck();
            dur = EditorGUI.FloatField(r, (float)dur);
            if (EditorGUI.EndChangeCheck())
            {
                switch (units)
                {
                    case TimePeriod.Units.Seconds:
                        span = System.TimeSpan.FromSeconds(dur);
                        break;
                    case TimePeriod.Units.Minutes:
                        span = System.TimeSpan.FromMinutes(dur);
                        break;
                    case TimePeriod.Units.Hours:
                        span = System.TimeSpan.FromHours(dur);
                        break;
                    case TimePeriod.Units.Days:
                        span = System.TimeSpan.FromDays(dur);
                        break;
                    case TimePeriod.Units.Years:
                        span = System.TimeSpan.FromTicks((long)(dur * System.TimeSpan.TicksPerDay * TimeUnitsSelectorPropertyDrawer.DAYS_IN_YEAR));
                        break;
                }
                property.floatValue = (float)span.TotalSeconds;
            }

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }

        public Rect DrawUnits(Rect position, SerializedProperty property, float desiredWidth)
        {
            if (position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);

            var units = GetUnits(property, this.attribute as TimeUnitsSelectorAttribute);
            EditorGUI.BeginChangeCheck();
            units = (TimePeriod.Units)EditorGUI.EnumPopup(r, units);
            if (EditorGUI.EndChangeCheck())
                SetUnits(property, units);

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }

    }
}
