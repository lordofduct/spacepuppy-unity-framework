using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Tween;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_TweenState))]
    public class i_TweenStateInspector : SPEditor
    {

        public const string PROP_TARGET = "_target";
        public const string PROP_SOURCE = "_source";
        public const string PROP_SOURCEALT = "_sourceAlt";
        public const string PROP_ANIMMODE = "_mode";
        public const string PROP_EASE = "_ease";
        public const string PROP_DURATION = "_duration";
        public const string PROP_TWEENTOKEN = "_tweenToken";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);

            this.DrawPropertyField(PROP_TARGET);
            
            var propSourceAlt = this.serializedObject.FindProperty(PROP_ANIMMODE);
            switch(propSourceAlt.GetEnumValue<TweenHash.AnimMode>())
            {
                case TweenHash.AnimMode.AnimCurve:
                case TweenHash.AnimMode.Curve:
                    propSourceAlt.SetEnumValue(TweenHash.AnimMode.To);
                    this.DrawPropertyField(PROP_SOURCE, "Values", false);
                    TriggerableTargetObjectPropertyDrawer.ResetTriggerableTargetObjectTarget(this.serializedObject.FindProperty(PROP_SOURCEALT));
                    break;
                case TweenHash.AnimMode.To:
                case TweenHash.AnimMode.From:
                case TweenHash.AnimMode.By:
                    this.DrawPropertyField(PROP_SOURCE, "Values", false);
                    TriggerableTargetObjectPropertyDrawer.ResetTriggerableTargetObjectTarget(this.serializedObject.FindProperty(PROP_SOURCEALT));
                    break;
                case TweenHash.AnimMode.FromTo:
                case TweenHash.AnimMode.RedirectTo:
                    this.DrawPropertyField(PROP_SOURCE, "Start", false);
                    this.DrawPropertyField(PROP_SOURCEALT, "End", false);
                    break;
            }
            
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_TARGET, PROP_SOURCE, PROP_SOURCEALT);

            this.serializedObject.ApplyModifiedProperties();
        }

    }

}
