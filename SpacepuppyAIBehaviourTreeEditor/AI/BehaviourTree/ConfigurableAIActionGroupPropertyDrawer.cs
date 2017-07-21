using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.AI.BehaviourTree;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.BehaviourTree
{

    [CustomPropertyDrawer(typeof(ConfigurableAIActionGroup), true)]
    public class ConfigurableAIActionGroupPropertyDrawer : PropertyDrawer
    {
        private const ParallelPassOptions BOTH_ANY = ParallelPassOptions.FailOnAny | ParallelPassOptions.SucceedOnAny;

        private const string PROP_REPEAT = "_repeat";
        private const string PROP_ALWAYSSUCEED = "_alwaysSucceed";
        private const string PROP_MODE = "_loopMode";
        private const string PROP_OPTIONS = "_passOptions";

        public bool DrawFlat;



        protected void Init()
        {
            if (this.fieldInfo != null)
            {
                var attrib = this.fieldInfo.GetCustomAttributes(typeof(ConfigurableAIActionGroup.ConfigAttribute), false).FirstOrDefault() as ConfigurableAIActionGroup.ConfigAttribute;
                if (attrib != null) this.DrawFlat = attrib.DrawFlat;
            }
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.Init();

            if(this.DrawFlat)
            {
                var h = EditorGUIUtility.singleLineHeight * 3f;

                var e = property.FindPropertyRelative(PROP_MODE).GetEnumValue<ActionGroupType>();
                if (e == ActionGroupType.Parrallel)
                {
                    h += EditorGUIUtility.singleLineHeight * 3f;
                }

                return h;
            }
            else
            {
                if (property.isExpanded)
                {
                    var h = EditorGUIUtility.singleLineHeight * 4f;

                    var e = property.FindPropertyRelative(PROP_MODE).GetEnumValue<ActionGroupType>();
                    if (e == ActionGroupType.Parrallel)
                    {
                        h += EditorGUIUtility.singleLineHeight * 3f;
                    }

                    return h;
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init();


            if(!this.DrawFlat)
            {
                var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.xMin, r0.yMax, position.width, position.height - r0.height);

                if (!property.isExpanded) label.text = string.Format("{0} ({1})", label.text, property.FindPropertyRelative(PROP_MODE).GetEnumValue<ActionGroupType>());
                property.isExpanded = EditorGUI.Foldout(r0, property.isExpanded, label);
                if (!property.isExpanded) return;

                EditorGUI.indentLevel++;
            }

            this.DrawPrimaryPortionOfInspector(position, property);

            if(!this.DrawFlat)
            {
                EditorGUI.indentLevel--;
            }
        }
        
        protected Rect DrawPrimaryPortionOfInspector(Rect position, SerializedProperty property)
        {
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(position.xMin, r0.yMax, position.width, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(position.xMin, r1.yMax, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(r0, property.FindPropertyRelative(PROP_REPEAT));
            EditorGUI.PropertyField(r1, property.FindPropertyRelative(PROP_ALWAYSSUCEED));

            var eloop = property.FindPropertyRelative(PROP_MODE).GetEnumValue<ActionGroupType>();
            EditorGUI.PropertyField(r2, property.FindPropertyRelative(PROP_MODE), EditorHelper.TempContent("Loop Mode"));

            if (eloop == ActionGroupType.Parrallel)
            {
                EditorGUI.indentLevel++;

                var propOptions = property.FindPropertyRelative(PROP_OPTIONS);
                var e = (ParallelPassOptions)propOptions.intValue;
                bool both = ((e & BOTH_ANY) == BOTH_ANY);

                bool failAny = e.HasFlag(ParallelPassOptions.FailOnAny);
                bool passAny = e.HasFlag(ParallelPassOptions.SucceedOnAny);
                bool passOnTie = e.HasFlag(ParallelPassOptions.SucceedOnTie);

                var r3 = new Rect(position.xMin, r2.yMax, position.width, EditorGUIUtility.singleLineHeight);
                var r4 = new Rect(position.xMin, r3.yMax, position.width, EditorGUIUtility.singleLineHeight);
                var r5 = new Rect(position.xMin, r4.yMax, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginChangeCheck();

                failAny = EditorGUI.Popup(r3, "Fail", (failAny) ? 1 : 0, new string[] { "All", "Any" }) == 1;
                passAny = EditorGUI.Popup(r4, "Succeed", (passAny) ? 1 : 0, new string[] { "All", "Any" }) == 1;
                
                var cache = SPGUI.DisableIf(both);
                passOnTie = EditorGUI.Popup(r5, "Tie Breaker", (passOnTie) ? 1 : 0, new string[] { "Fail", "Succeed" }) == 1;
                cache.Reset();

                if (EditorGUI.EndChangeCheck())
                {
                    e = 0;
                    if (failAny) e |= ParallelPassOptions.FailOnAny;
                    if (passAny) e |= ParallelPassOptions.SucceedOnAny;
                    if (passOnTie) e |= ParallelPassOptions.SucceedOnTie;
                    propOptions.intValue = (int)e;
                }

                EditorGUI.indentLevel--;

                return new Rect(position.xMin, r5.yMax, position.width, position.yMax - r5.yMax);
            }
            else
            {
                return new Rect(position.xMin, r2.yMax, position.width, position.yMax - r2.yMax);
            }
        }

    }
}
