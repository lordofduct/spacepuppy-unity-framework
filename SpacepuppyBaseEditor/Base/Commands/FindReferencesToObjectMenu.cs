using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using System.Reflection;
using System.Linq;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Commands
{
    public class FindReferencesToObjectMenu
    {

        #region Menu Entries

        private static FindReferencesToObjectMenu _defaultSearchTool;

        //[MenuItem("CONTEXT/ALT/Find References To", validate=true)]
        //private static bool Validate_Search()
        //{
        //    return true;
        //}

        [MenuItem("CONTEXT/ALT/Find References To")]
        private static void Menu_Search(MenuCommand cmnd)
        {
            if (cmnd.context == null) return;
            if (_defaultSearchTool == null) _defaultSearchTool = new FindReferencesToObjectMenu();

            int iid = cmnd.context.GetInstanceID();
            var arr = _defaultSearchTool.Search_Imp(iid);
            Selection.objects = arr;

            //foreach(var obj in arr)
            //{
            //    EditorGUIUtility.PingObject(obj.GetInstanceID());
            //}
        }

        #endregion


        #region Fields

        private Dictionary<System.Type, FieldInfo[]> _fieldTable = new Dictionary<System.Type, FieldInfo[]>();
        private HashSet<GameObject> _hits = new HashSet<GameObject>();
        private HashSet<object> _referenceLoopHits = new HashSet<object>();

        #endregion

        #region Methods

        public GameObject[] Search(GameObject target)
        {
            if (target == null) throw new System.ArgumentNullException("target");

            return this.Search_Imp(target.GetInstanceID());
        }

        public GameObject[] Search(int instanceId)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj == null) return ArrayUtil.Empty<GameObject>();

            return this.Search_Imp(instanceId);
        }

        private GameObject[] Search_Imp(int instanceId)
        {
            _hits.Clear();
            var comps = Object.FindObjectsOfType<Component>();

            foreach (var c in comps)
            {
                if (_hits.Contains(c.gameObject)) continue;

                _referenceLoopHits.Clear();
                var tp = c.GetType();
                var infos = GetRelevantFieldInfos(tp);

                foreach (var field in infos)
                {
                    if (TestField(instanceId, field, c))
                    {
                        _hits.Add(c.gameObject);
                        break;
                    }
                }
            }

            var arr = _hits.ToArray();
            _hits.Clear();
            _referenceLoopHits.Clear();

            return arr;
        }

        private FieldInfo[] GetRelevantFieldInfos(System.Type tp)
        {
            FieldInfo[] infos;
            if (!_fieldTable.TryGetValue(tp, out infos))
            {
                infos = (from f in GetAllFields(tp)
                         where ValidateFieldInfo(f)
                         select f).ToArray();
                _fieldTable[tp] = infos;
            }
            return infos;
        }

        private bool TestField(int instanceIdToFind, FieldInfo field, object obj)
        {
            var ftp = field.FieldType;
            var fieldValue = field.GetValue(obj);

            if (fieldValue == null) return false;
            if (_referenceLoopHits.Contains(fieldValue)) return false;

            _referenceLoopHits.Add(fieldValue);
            if (ftp.IsListType() && fieldValue is System.Collections.IEnumerable)
            {
                ftp = ftp.GetElementTypeOfListType();
                var e = fieldValue as System.Collections.IEnumerable;
                foreach (var v in e)
                {
                    if (_referenceLoopHits.Contains(v)) continue;
                    _referenceLoopHits.Add(v);
                    if (TestFieldValue(instanceIdToFind, ftp, v)) return true;
                }
            }
            else
            {
                if (TestFieldValue(instanceIdToFind, ftp, fieldValue)) return true;
            }

            return false;
        }

        private bool TestFieldValue(int instanceIdToFind, System.Type ftp, object fieldValue)
        {
            try
            {
                GameObject go = GameObjectUtil.GetGameObjectFromSource(fieldValue);
                if (go != null && go.GetInstanceID() == instanceIdToFind)
                {
                    return true;
                }
            }
            catch
            {
            }

            if (fieldValue != null)
            {
                var infos = GetRelevantFieldInfos(ftp);
                foreach (var subfield in infos)
                {
                    if (TestField(instanceIdToFind, subfield, fieldValue)) return true;
                }
            }

            return false;
        }

        #endregion


        #region Static Util Methods

        private static IEnumerable<FieldInfo> GetAllFields(System.Type tp)
        {
            if (tp == null) yield break;

            foreach (var f in tp.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                yield return f;
            }

            System.Type stopType = null;
            if (TypeUtil.IsType(tp, typeof(MonoBehaviour)))
                stopType = typeof(MonoBehaviour);
            else if (TypeUtil.IsType(tp, typeof(UnityEngine.Object)))
                stopType = tp.BaseType;
            else
                stopType = typeof(object);

            while (tp != null && tp != stopType)
            {
                foreach (var f in tp.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    yield return f;
                }
                tp = tp.BaseType;
            }
        }

        private static bool ValidateFieldInfo(FieldInfo field)
        {
            var ftp = field.FieldType;
            if (ftp.IsListType()) ftp = ftp.GetElementTypeOfListType();

            if (TypeUtil.IsType(ftp, typeof(UnityEngine.Object))) return true;
            if (!ftp.IsSerializable) return false;
            if (field.GetCustomAttributes(typeof(SerializeField), false).Length > 0) return true;
            if (field.IsPublic && field.GetCustomAttributes(typeof(System.NonSerializedAttribute), false).Length == 0) return true;

            return false;
        }

        #endregion

    }
}
