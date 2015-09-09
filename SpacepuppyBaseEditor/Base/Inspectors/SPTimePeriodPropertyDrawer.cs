using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy;
using com.spacepuppy.Collections;
using com.spacepuppy.Project;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Inspectors
{

    [CustomPropertyDrawer(typeof(SPTimePeriod))]
    public class SPTimePeriodPropertyDrawer : PropertyDrawer
    {

        public const string PROP_SECONDS = "_seconds";

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
            var attrib = this.fieldInfo.GetCustomAttributes(typeof(SPTime.Config), false).FirstOrDefault() as SPTime.Config;
            var availNames = (attrib != null) ? attrib.AvailableCustomTimeNames : null;

            if (w > 75f)
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, Mathf.Min(w, 150f));
                position = _timeDrawer.DrawUnits(position, secondsProp, 75f);
                position = SPTimePropertyDrawer.DrawTimeSupplier(position, property, position.width, availNames); //we mirror the SPTime prop drawer, we can do this because the property names are identical
            }
            else
            {
                position = _timeDrawer.DrawDuration(position, secondsProp, w);
                position = _timeDrawer.DrawUnits(position, secondsProp, w);


                position = SPTimePropertyDrawer.DrawTimeSupplier(position, property, position.width, availNames); //we mirror the SPTime prop drawer, we can do this because the property names are identical
            }
        }

    }
}
