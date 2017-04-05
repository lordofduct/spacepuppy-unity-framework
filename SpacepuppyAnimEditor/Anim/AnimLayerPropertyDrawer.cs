using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Anim;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim
{

    [CustomPropertyDrawer(typeof(AnimLayerAttribute))]
    public class AnimLayerPropertyDrawer : PropertyDrawer
    {

        #region Drawer Instance Interface

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(_supplier == null)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            var infos = _supplier.GetLayers();
            if (infos == null || infos.Length == 0)
            {
                SPEditorGUI.DefaultPropertyField(position, property, label);
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            var names = (from info in infos select info.Name).Append("Custom...").ToArray();
            int i = this.GetCurrentIndex(infos, property.intValue);
            if (i < 0) i = infos.Length;

            var r1 = new Rect(position.xMin, position.yMin, position.width * 0.7f, position.height);
            var r2 = new Rect(r1.xMax, r1.yMin, position.width - r1.width, r1.height);

            EditorGUI.BeginChangeCheck();
            i = EditorGUI.Popup(r1, i, names);
            if (EditorGUI.EndChangeCheck())
            {
                if (i == infos.Length)
                {
                    property.intValue = (from info in infos select info.Layer).Max() + 1;
                }
                else
                {
                    property.intValue = infos[i].Layer;
                }
            }

            EditorGUI.BeginChangeCheck();
            int layer = EditorGUI.IntField(r2, property.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = layer;
            }
        }

        private int GetCurrentIndex(AnimLayerInformation[] infos, int currentLayer)
        {
            for(int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Layer == currentLayer) return i;
            }
            return -1;
        }

        #endregion




        #region Static Interface

        private static IAnimLayerInformationSupplier _supplier;

        static AnimLayerPropertyDrawer()
        {
            SyncLayerData();
        }

        public static void SyncLayerData()
        {
            _supplier = null;
            var tp = TypeUtil.GetTypesAssignableFrom(typeof(IAnimLayerInformationSupplier))
                             .Where((t) =>
                             {
                                 return t.IsClass && !t.IsAbstract;
                             })
                             .OrderByDescending((t) =>
                             {
                                 var attrib = t.GetCustomAttributes(typeof(AnimLayerInformationSupplierConfigAttribute), false).FirstOrDefault() as AnimLayerInformationSupplierConfigAttribute;
                                 if (attrib == null) return 0;
                                 else return attrib.Priority;
                             })
                             .FirstOrDefault();

            if(tp != null)
            {
                try
                {
                    _supplier = System.Activator.CreateInstance(tp) as IAnimLayerInformationSupplier;
                }
                catch
                {
                    Debug.LogWarning("Failed to create IAnimLayerInformationSupplier with highest priority. Make sure a constructor with zero parameters exists and doesn't throw an exception.");
                }
            }
        }

        #endregion

    }
}
