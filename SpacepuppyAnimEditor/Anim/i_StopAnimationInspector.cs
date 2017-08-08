using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppyeditor.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim
{

    [CustomEditor(typeof(i_StopAnimation), true)]
    public class i_StopAnimationInspector : SPEditor
    {

        public const string PROP_ORDER = EditorHelper.PROP_ORDER;
        public const string PROP_TARGETANIMATOR = "_targetAnimator";
        public const string PROP_MODE = "_mode";
        public const string PROP_ID = "_id";
        public const string PROP_LAYER = "_layer";

        private TriggerableTargetObjectPropertyDrawer _targetDrawer = new TriggerableTargetObjectPropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);

            this.DrawTargetAnimatorProperty();

            var propMode = this.serializedObject.FindProperty(PROP_MODE);
            SPEditorGUILayout.PropertyField(propMode);
            
            switch(propMode.GetEnumValue<i_StopAnimation.StopMode>())
            {
                case i_StopAnimation.StopMode.Id:
                    this.DrawPropertyField(PROP_ID);
                    break;
                case i_StopAnimation.StopMode.Layer:
                    this.DrawPropertyField(PROP_LAYER);
                    break;
            }

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_TARGETANIMATOR, PROP_MODE, PROP_ID, PROP_LAYER);
           
            this.serializedObject.ApplyModifiedProperties();
        }



        private void DrawTargetAnimatorProperty()
        {
            var targWrapperProp = this.serializedObject.FindProperty(PROP_TARGETANIMATOR);
            var targProp = targWrapperProp.FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET);

            _targetDrawer.ManuallyConfigured = true;

            var label = EditorHelper.TempContent(targWrapperProp.displayName);
            var rect = EditorGUILayout.GetControlRect(true, _targetDrawer.GetPropertyHeight(targWrapperProp, label));
            _targetDrawer.OnGUI(rect, targWrapperProp, label);


            var obj = targProp.objectReferenceValue;
            if (obj == null || i_StopAnimation.IsAcceptibleAnimator(obj))
                return;

            var go = GameObjectUtil.GetGameObjectFromSource(obj);

            SPAnimationController src;
            if (go.GetComponent<SPAnimationController>(out src))
            {
                targProp.objectReferenceValue = src as UnityEngine.Object;
                return;
            }

            Animation anim;
            if (go.GetComponent<Animation>(out anim))
            {
                targProp.objectReferenceValue = anim;
                return;
            }
        }

    }
}
