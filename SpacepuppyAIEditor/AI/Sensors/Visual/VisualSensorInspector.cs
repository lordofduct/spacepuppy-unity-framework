using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.Sensors;
using com.spacepuppy.AI.Sensors.Visual;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Sensors.Visual
{

    [CustomEditor(typeof(RightCylindricalVisualSensor), true)]
    public class VisualSensorInspector : SPEditor
    {

        public const string PROP_LOS = "_requiresLineOfSight";
        public const string PROP_LOS_MASK = "_lineOfSightMask";


        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept(PROP_LOS, PROP_LOS_MASK);

            this.DrawPropertyField(PROP_LOS);
            this.DrawPropertyField(PROP_LOS_MASK);

            this.serializedObject.ApplyModifiedProperties();
        }

    }
}
