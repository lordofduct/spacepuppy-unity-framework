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

        #region Get/Set TimeUnits

        //private static Dictionary<int, TimeUnits> _unitsCache = new Dictionary<int, TimeUnits>();
        //private static TimeUnits GetUnits(SerializedProperty property, TimeUnitsSelectorAttribute attrib)
        //{
        //    int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
        //    TimeUnits units;
        //    if (_unitsCache.TryGetValue(hash, out units))
        //        return units;
        //    else
        //    {
        //        if (attrib == null)
        //            return TimeUnits.Seconds;
        //        else
        //            return attrib.DefaultUnits;
        //    }
        //}
        //private static void SetUnits(SerializedProperty property, TimeUnits units)
        //{
        //    int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
        //    _unitsCache[hash] = units;
        //}
        private static Dictionary<int, string> _unitsCache = new Dictionary<int, string>();
        private static string GetUnits(SerializedProperty property, TimeUnitsSelectorAttribute attrib, ITimeUnitsCalculator calculator)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            
            string units;
            if(!_unitsCache.TryGetValue(hash,out units))
            {
                if (attrib != null)
                    units = attrib.DefaultUnits; 
            }

            if(!calculator.TimeUnits.Contains(units))
            {
                units = calculator.DefaultUnits;
            }
            return units;
        }
        private static void SetUnits(SerializedProperty property, string units)
        {
            int hash = com.spacepuppyeditor.Internal.PropertyHandlerCache.GetPropertyHash(property);
            _unitsCache[hash] = units;
        }

        #endregion

        #region Fields

        private ITimeUnitsCalculator _calculator;

        public ITimeUnitsCalculator TimeUnitsCalculator
        {
            get { return _calculator ?? _defaultCalculator; }
            set
            {
                _calculator = value;
            }
        }

        #endregion

        #region Methods

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

            var units = GetUnits(property, this.attribute as TimeUnitsSelectorAttribute, this.TimeUnitsCalculator);
            
            double dur = property.doubleValue;
            if (MathUtil.IsReal(dur)) dur = this.TimeUnitsCalculator.SecondsToTimeUnits(units, dur);
            EditorGUI.BeginChangeCheck();
            dur = EditorGUI.DoubleField(r, (float)dur);
            if (EditorGUI.EndChangeCheck())
            {
                if(MathUtil.IsReal(dur)) dur = this.TimeUnitsCalculator.TimeUnitsToSeconds(units, dur);
                property.doubleValue = dur;
            }

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }

        public Rect DrawUnits(Rect position, SerializedProperty property, float desiredWidth)
        {
            if (position.width <= 0f) return position;

            var r = new Rect(position.xMin, position.yMin, Mathf.Min(position.width, desiredWidth), position.height);

            var units = GetUnits(property, this.attribute as TimeUnitsSelectorAttribute, this.TimeUnitsCalculator);

            EditorGUI.BeginChangeCheck();

            var allowedUnits = this.TimeUnitsCalculator.TimeUnits;
            int i = allowedUnits.IndexOf(units);
            i = EditorGUI.Popup(r, i, allowedUnits);

            if (EditorGUI.EndChangeCheck())
                SetUnits(property, (i < 0) ? allowedUnits.FirstOrDefault() : allowedUnits[i]);

            return new Rect(r.xMax, position.yMin, Mathf.Max(position.width - r.width, 0f), position.height);
        }

        #endregion



        #region Special Types

        private static ITimeUnitsCalculator _defaultCalculator;

        static TimeUnitsSelectorPropertyDrawer()
        {
            int order = int.MinValue;
            System.Type selectedType = null;

            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(ITimeUnitsCalculator)))
            {
                var attrib = tp.GetCustomAttributes(typeof(OverrideDefaultTimeUnitsCalculatorAttribute), false).FirstOrDefault() as OverrideDefaultTimeUnitsCalculatorAttribute;
                if(attrib != null)
                {
                    if(attrib.order > order || selectedType == null)
                    {
                        order = attrib.order;
                        selectedType = tp;
                    }
                }
            }

            if (selectedType != null)
            {
                try
                {
                    _defaultCalculator = System.Activator.CreateInstance(selectedType) as ITimeUnitsCalculator;
                }
                catch
                {
                    Debug.LogWarning("Failed to create an override time units calculator of type '" + selectedType.FullName + "'");
                }
            }

            if(_defaultCalculator == null)
            {
                _defaultCalculator = new DefaultTimeUnitsCalculator();
            }

        }

        public class OverrideDefaultTimeUnitsCalculatorAttribute : System.Attribute
        {
            public int order;
        }

        public interface ITimeUnitsCalculator
        {

            string[] TimeUnits { get; }

            string DefaultUnits { get; }

            double SecondsToTimeUnits(string units, double seconds);

            double TimeUnitsToSeconds(string units, double time);

        }

        public class DefaultTimeUnitsCalculator : ITimeUnitsCalculator
        {

            public const double DAYS_IN_YEAR = 365d;

            private string[] _units = new string[]
            {
                "Seconds",
                "Minutes",
                "Hours",
                "Days",
                "Years"
            };

            public virtual string[] TimeUnits
            {
                get { return _units; }
            }

            public virtual string DefaultUnits
            {
                get { return "Seconds"; }
            }

            public virtual double SecondsToTimeUnits(string units, double seconds)
            {
                var span = System.TimeSpan.FromSeconds(seconds);

                switch(units)
                {
                    case "Seconds":
                        return span.TotalSeconds;
                    case "Minutes":
                        return span.TotalMinutes;
                    case "Hours":
                        return span.TotalHours;
                    case "Days":
                        return span.TotalDays;
                    case "Years":
                        return span.Ticks / (System.TimeSpan.TicksPerDay * DAYS_IN_YEAR);
                    default:
                        return seconds;
                }
            }

            public virtual double TimeUnitsToSeconds(string units, double time)
            {
                switch(units)
                {
                    case "Seconds":
                        return time;
                    case "Minutes":
                        return System.TimeSpan.FromMinutes(time).TotalSeconds;
                    case "Hours":
                        return System.TimeSpan.FromHours(time).TotalSeconds;
                    case "Days":
                        return System.TimeSpan.FromDays(time).TotalSeconds;
                    case "Years":
                        return System.TimeSpan.FromTicks((long)(time * System.TimeSpan.TicksPerDay * DAYS_IN_YEAR)).TotalSeconds;
                    default:
                        return time;
                }
            }

        }

        #endregion


    }
}
