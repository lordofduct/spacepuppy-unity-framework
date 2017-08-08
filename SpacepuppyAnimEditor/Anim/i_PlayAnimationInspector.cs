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
    
    [CustomEditor(typeof(i_PlayAnimation), true)]
    public class i_PlayAnimationInspector : SPEditor
    {

        public const string PROP_ORDER = "_order";
        public const string PROP_MODE = "_mode";
        public const string PROP_TARGETANIMATOR = "_targetAnimator";
        public const string PROP_ID = "_id";
        public const string PROP_CLIP = "_clip";
        public const string PROP_SETTINGS = "_settings";

        private TriggerableTargetObjectPropertyDrawer _targetDrawer = new TriggerableTargetObjectPropertyDrawer();

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);

            this.DrawTargetAnimatorProperty();

            var propMode = this.serializedObject.FindProperty(PROP_MODE);
            SPEditorGUILayout.PropertyField(propMode);

            switch (propMode.GetEnumValue<i_PlayAnimation.PlayByMode>())
            {
                case i_PlayAnimation.PlayByMode.PlayAnim:
                    {
                        this.serializedObject.FindProperty(PROP_ID).stringValue = string.Empty;

                        var clipProp = this.serializedObject.FindProperty(PROP_CLIP);
                        var obj = EditorGUILayout.ObjectField(EditorHelper.TempContent(clipProp.displayName), clipProp.objectReferenceValue, typeof(UnityEngine.Object), true);
                        if (obj == null || obj is AnimationClip || obj is IScriptableAnimationClip)
                            clipProp.objectReferenceValue = obj;
                        else if (GameObjectUtil.IsGameObjectSource(obj))
                            clipProp.objectReferenceValue = ObjUtil.GetAsFromSource<IScriptableAnimationClip>(obj) as UnityEngine.Object;

                        this.DrawPropertyField(PROP_SETTINGS);
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimByID:
                    {
                        this.serializedObject.FindProperty(PROP_CLIP).objectReferenceValue = null;

                        this.DrawPropertyField(PROP_ID);

                        if(this.serializedObject.FindProperty(PROP_TARGETANIMATOR).FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET).objectReferenceValue is Animation)
                        {
                            this.DrawPropertyField(PROP_SETTINGS);
                        }
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                    {
                        this.serializedObject.FindProperty(PROP_CLIP).objectReferenceValue = null;

                        this.DrawPropertyField(PROP_ID);

                        this.DrawPropertyField(PROP_SETTINGS);
                    }
                    break;
            }
            
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_MODE, PROP_TARGETANIMATOR, PROP_ID, PROP_CLIP, PROP_SETTINGS);

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
            if (obj == null || i_PlayAnimation.IsAcceptibleAnimator(obj))
                return;

            var go = GameObjectUtil.GetGameObjectFromSource(obj);

            ISPAnimationSource src;
            if (go.GetComponent<ISPAnimationSource>(out src))
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

            ISPAnimator animator;
            if (go.GetComponent<ISPAnimator>(out animator))
            {
                targProp.objectReferenceValue = animator as UnityEngine.Object;
                return;
            }
        }

    }

}
