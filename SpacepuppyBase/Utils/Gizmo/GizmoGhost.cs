using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Utils.Gizmo
{
    
    [ExecuteInEditMode()]
    public class GizmoGhost : Singleton
    {

        #region Fields

        private const float DUR_LIFE = 5f;

        private static GizmoGhost _instance;
        private static GizmoGhost Instance
        {
            get
            {
                if (_instance == null) _instance = Singleton.CreateSpecialInstance<GizmoGhost>("Spacepuppy.GizmoGhost", SingletonLifeCycleRule.LivesForever);
                return _instance;
            }
        }

        private List<DrawHandle> _handles = new List<DrawHandle>();

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            if (!Application.isEditor)
            {
                if (this.gameObject == Singleton.GameObjectSource)
                    Object.Destroy(this);
                else
                    Object.Destroy(this.gameObject);
            }
        }

        #endregion

        #region Methods

        public static void DrawCube(Color c, Vector3 center, Vector3 size)
        {
            if (!Application.isEditor) return;

            GizmoGhost.Instance._handles.Add(new DrawHandle(() =>
            {
                Gizmos.color = c;
                Gizmos.DrawCube(center, size);
            }, DUR_LIFE));
        }

        public static void DrawSphere(Color c, Vector3 center, float radius)
        {
            if (!Application.isEditor) return;

            GizmoGhost.Instance._handles.Add(new DrawHandle(() =>
            {
                Gizmos.color = c;
                Gizmos.DrawSphere(center, radius);
            }, DUR_LIFE));
        }

        #endregion



        #region Messages

        private void Update()
        {
            if (!Application.isEditor) return;

            for(int i = 0; i < _handles.Count; i++)
            {
                _handles[i].Life -= Time.deltaTime;
                if(_handles[i].Life <= 0f)
                {
                    _handles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isEditor) return;

            for(int i = 0; i <_handles.Count; i++)
            {
                _handles[i].Draw();
            }
        }

        #endregion

        #region Special Types

        private class DrawHandle
        {

            public float Life;
            public System.Action Draw;

            public DrawHandle(System.Action act, float lifeTime)
            {
                this.Draw = act;
                this.Life = lifeTime;
            }

        }

        #endregion

    }

}
