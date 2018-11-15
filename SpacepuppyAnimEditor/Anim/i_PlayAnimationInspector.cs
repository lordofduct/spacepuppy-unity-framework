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
        private const string PROP_APPLYSETTINGS = "_applyCustomSettings"; //OBSOLETE
        public const string PROP_SETTINGSMASK = "_settingsMask";
        public const string PROP_SETTINGS = "_settings";
        public const string PROP_QUEUEMODE = "_queueMode";
        public const string PROP_PLAYMODE = "_playMode";
        public const string PROP_CROSSFADEDUR = "_crossFadeDur";
        private static string[] PROPS_ANIMSETTINGS = new string[] { "weight", "speed", "layer", "wrapMode", "blendMode", "timeSupplier" };


        private TriggerableTargetObjectPropertyDrawer _targetDrawer = new TriggerableTargetObjectPropertyDrawer()
        {
            ManuallyConfigured = true,
            SearchChildren = false,
            ChoiceSelector = new com.spacepuppyeditor.Components.MultiTypeComponentChoiceSelector()
            {
                AllowedTypes = new System.Type[] { typeof(Animation), typeof(ISPAnimationSource), typeof(ISPAnimator) }
            }
        };

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(PROP_ORDER);

            this.DrawTargetAnimatorProperty();

            var propTarget = this.serializedObject.FindProperty(PROP_TARGETANIMATOR);
            var controller = propTarget.FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET).objectReferenceValue;
            System.Type tp;
            switch(GetAnimatorType(controller, out tp))
            {
                case AnimatorType.Unknown:
                case AnimatorType.Animation:
                case AnimatorType.SPAnimController:
                    {
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
                                }
                                break;
                            case i_PlayAnimation.PlayByMode.PlayAnimByID:
                                {
                                    this.serializedObject.FindProperty(PROP_CLIP).objectReferenceValue = null;

                                    //this.DrawPropertyField(PROP_ID);
                                    this.DrawAnimIdSelector(controller);
                                }
                                break;
                            case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                                {
                                    this.serializedObject.FindProperty(PROP_CLIP).objectReferenceValue = null;

                                    this.DrawPropertyField(PROP_ID);
                                }
                                break;
                        }

                        this.DrawAnimSettings();
                        this.DrawPropertyField(PROP_QUEUEMODE);
                        this.DrawPropertyField(PROP_PLAYMODE);
                        this.DrawPropertyField(PROP_CROSSFADEDUR);
                    }
                    break;
                case AnimatorType.SPAnimator:
                    {
                        var propId = this.serializedObject.FindProperty(PROP_ID);
                        propId.stringValue = DrawSPAnimatorFunctionPopup(EditorGUILayout.GetControlRect(), tp, propId.stringValue);
                    }
                    break;
                case AnimatorType.SPAnimSource:
                    {
                        this.serializedObject.FindProperty(PROP_MODE).SetEnumValue<i_PlayAnimation.PlayByMode>(i_PlayAnimation.PlayByMode.PlayAnimByID);
                        this.serializedObject.FindProperty(PROP_CLIP).objectReferenceValue = null;

                        this.DrawPropertyField(PROP_ID);
                        this.DrawAnimSettings();
                        this.DrawPropertyField(PROP_QUEUEMODE);
                        this.DrawPropertyField(PROP_PLAYMODE);
                        this.DrawPropertyField(PROP_CROSSFADEDUR);
                    }
                    break;
            }
            
            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, PROP_ORDER, PROP_MODE, PROP_TARGETANIMATOR, PROP_ID, PROP_CLIP, PROP_APPLYSETTINGS, PROP_SETTINGSMASK, PROP_SETTINGS, PROP_QUEUEMODE, PROP_PLAYMODE, PROP_CROSSFADEDUR);

            this.serializedObject.ApplyModifiedProperties();
        }


        private void DrawTargetAnimatorProperty()
        {
            var targWrapperProp = this.serializedObject.FindProperty(PROP_TARGETANIMATOR);
            
            var label = EditorHelper.TempContent(targWrapperProp.displayName);
            var rect = EditorGUILayout.GetControlRect(true, _targetDrawer.GetPropertyHeight(targWrapperProp, label));
            _targetDrawer.OnGUI(rect, targWrapperProp, label);
            

            var targProp = targWrapperProp.FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET);
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

        private void DrawAnimSettings()
        {
            /*
            var propApply = this.serializedObject.FindProperty(PROP_APPLYSETTINGS);
            SPEditorGUILayout.PropertyField(propApply);
            if (propApply.boolValue)
            {
                //this.DrawPropertyField(PROP_SETTINGS);
                EditorGUI.indentLevel++;
                SPEditorGUILayout.FlatChildPropertyField(this.serializedObject.FindProperty(PROP_SETTINGS));
                EditorGUI.indentLevel--;
            }
            */

            var propMask = this.serializedObject.FindProperty(PROP_SETTINGSMASK);
            var propSettings = this.serializedObject.FindProperty(PROP_SETTINGS);

            int mask = propMask.intValue;
            propSettings.isExpanded = EditorGUILayout.Foldout(propSettings.isExpanded, mask != 0 ? "Custom Settings : Active" : "Custom Settings");

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
            {
                int m = 1 << i;
                bool active = (mask & m) != 0;
                if (!propSettings.isExpanded && !active) continue;

                var propSet = propSettings.FindPropertyRelative(PROPS_ANIMSETTINGS[i]);
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.Toggle(active, GUILayout.MaxWidth(20f)))
                {
                    mask |= m;
                    EditorGUILayout.PropertyField(propSet);
                }
                else
                {
                    mask &= ~m;
                    EditorGUILayout.PrefixLabel(EditorHelper.TempContent(propSet.displayName, propSet.tooltip));
                }

                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
                propMask.intValue = mask;
        }

        private void DrawAnimIdSelector(object animator)
        {
            if(animator == null)
            {
                this.DrawPropertyField(PROP_ID);
                return;
            }

            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<string>())
            {
                GetAnimationIds(lst, animator);
                if(lst.Count == 0)
                {
                    this.DrawPropertyField(PROP_ID);
                    return;
                }
                else
                {
                    var propId = this.serializedObject.FindProperty(PROP_ID);
                    propId.stringValue = SPEditorGUILayout.OptionPopupWithCustom(propId.displayName, propId.stringValue, lst.ToArray());
                }
            }

        }

        public static string DrawSPAnimatorFunctionPopup(Rect position, System.Type animatorType, string functionName)
        {
            var methods = (from m in animatorType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                           let attrib = m.GetCustomAttributes(typeof(SPAnimatorMethodAttribute), true).FirstOrDefault()
                           where attrib != null && m.GetParameters().Length == 0
                           select new { attrib, m }).ToArray();
            if (methods.Length > 0)
            {
                var names = (from o in methods select o.m.Name).ToArray();

                int index = System.Array.IndexOf(names, functionName);
                index = EditorGUI.Popup(position, "Function", index, names);
                if (index < 0) index = 0;
                return names[index];
            }
            else
            {
                EditorGUI.LabelField(position, "Function", "No functions found on animator");
                return string.Empty;
            }
        }

        private static AnimatorType GetAnimatorType(object obj, out System.Type tp)
        {
            if(obj is IProxy)
            {
                tp = (obj as IProxy).GetTargetType();
                return AnimUtil.GetAnimatorType(tp);
            }
            else if (obj != null)
            {
                tp = obj.GetType();
                return AnimUtil.GetAnimatorType(tp);
            }
            else
            {
                tp = null;
                return AnimatorType.Unknown;
            }
        }


        private static void GetAnimationIds(List<string> results, object animator)
        {
            if (animator is IProxy) animator = (animator as IProxy).GetTarget();
            if (animator == null) return;

            if(animator is Animation)
            {
                results.AddRange(from AnimationState st in (animator as System.Collections.IEnumerable) select (st as AnimationState).name);
            }
            else if (animator is SPAnimationController)
            {
                results.AddRange((animator as SPAnimationController).States.Keys);
            }
        }


    }

}
