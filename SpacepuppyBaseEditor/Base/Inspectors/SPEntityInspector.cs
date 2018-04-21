using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{

    [InitializeOnLoad()]
    [CustomEditor(typeof(SPEntity), true)]
    public class SPEntityInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.EnsureHasRootTag();

            this.DrawDefaultInspector();
        }

        public void EnsureHasRootTag()
        {
            var ent = this.target as SPEntity;
            if (!ent.HasTag(SPConstants.TAG_ROOT)) ent.AddTag(SPConstants.TAG_ROOT);
        }


        static SPEntityInspector()
        {
            var tp = typeof(SPEntity);
            var field = tp.GetMember("_pool", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).FirstOrDefault() as System.Reflection.FieldInfo;
            if (field != null)
            {
                var pool = new EditorEntityPool();
                var old = field.GetValue(null) as SPEntity.EntityPool;
                field.SetValue(null, pool);
                if(old != null)
                {
                    var e = old.GetEnumerator();
                    while(e.MoveNext())
                    {
                        pool.AddReference(e.Current);
                    }
                }
            }
        }



        private class EditorEntityPool : SPEntity.EntityPool
        {

            public override SPEntity GetFromSource(object obj)
            {
                if (Application.isPlaying)
                    return base.GetFromSource(obj);

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null)
                    return go.GetComponentInParent<SPEntity>();
                else
                    return null;
            }

            public override TSub GetFromSource<TSub>(object obj)
            {
                if (Application.isPlaying)
                    return base.GetFromSource<TSub>(obj);

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null)
                {
                    var e = go.GetComponentInParent<SPEntity>();
                    if (e is TSub) return e as TSub;
                }

                return null;
            }

            public override SPEntity GetFromSource(System.Type tp, object obj)
            {
                if (Application.isPlaying)
                    return base.GetFromSource(tp, obj);

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null)
                {
                    var e = go.GetComponentInParent<SPEntity>();
                    if (TypeUtil.IsType(e.GetType(), tp)) return e;
                }

                return null;
            }

        }

    }
}
