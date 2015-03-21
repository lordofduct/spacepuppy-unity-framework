using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Tween
{
    public class MeshCurve : Curve
    {

        #region Fields

        private Mesh[] _meshes;
        private Ease _ease;
        private float _dur;

        private int m_SrcMesh = -1;
        private int m_DstMesh = -1;
        private float m_Weight = -1;

        #endregion

        #region CONSTRUCTOR

        public MeshCurve(Mesh[] meshes, Ease ease, float dur)
        {
            _meshes = meshes;
            _ease = ease;
            _dur = dur;

            //  At least two meshes
            if (_meshes.Length < 2)
            {
                throw new System.ArgumentException("MeshCurve requires at least 2 source meshes.", "meshes");
            }

            // Make sure all meshes are assigned and shaped correctly!
            int vertexCount = (_meshes[0] != null) ? _meshes[0].vertexCount : 0;
            for (int i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i] == null)
                {
                    throw new System.ArgumentNullException("Mesh  " + i + " is null.", "meshes");
                }
                if (_meshes[i].vertexCount != vertexCount)
                {
                    throw new System.ArgumentException("Mesh " + i + " doesn't have the same number of vertices as the first mesh", "meshes");
                }
            }

        }

        #endregion

        #region Methods

        /// Set the current morph in    
        public void SetComplexMorph(MeshFilter targ, int srcIndex, int dstIndex, float t)
        {
            if (m_SrcMesh == srcIndex && m_DstMesh == dstIndex && Mathf.Approximately(m_Weight, t))
                return;

            Vector3[] v0 = _meshes[srcIndex].vertices;
            Vector3[] v1 = _meshes[dstIndex].vertices;
            Vector3[] vdst = new Vector3[targ.mesh.vertexCount];
            for (int i = 0; i < vdst.Length; i++)
                vdst[i] = Vector3.Lerp(v0[i], v1[i], t);

            targ.mesh.vertices = vdst;
            targ.mesh.RecalculateBounds();
        }

        /// t is between 0 and m_Meshes.Length - 1.
        /// 0 means the first mesh, m_Meshes.Length - 1 means the last mesh.
        /// 0.5 means half of the first mesh and half of the second mesh.
        public void SetMorph(MeshFilter targ, float t)
        {
            int floor = (int)t;
            floor = Mathf.Clamp(floor, 0, _meshes.Length - 2);
            float fraction = t - floor;
            fraction = Mathf.Clamp(t - floor, 0.0F, 1.0F);
            SetComplexMorph(targ, floor, floor + 1, fraction);
        }

        #endregion

        #region Curve Interface

        public override float TotalTime
        {
            get { return _dur; }
        }

        protected internal override void Update(object targ, float dt, float t)
        {
            if (targ is MeshFilter) this.SetMorph(targ as MeshFilter, _ease(t, 0f, 1f, _dur));
        }

        #endregion

    }
}
