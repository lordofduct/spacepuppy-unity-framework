using UnityEngine;

using com.spacepuppy;
using com.spacepuppy.Collections;

namespace com.spacepuppy.Tween
{
	public class MeshTweener : Tweener
    {

        #region Fields

        private MeshFilter _filter;
        private Mesh[] _meshes;
        private Ease _ease;
        private float _dur;

        private int m_SrcMesh = -1;
        private int m_DstMesh = -1;
        private float m_Weight = -1;

        #endregion

        #region CONSTRUCTOR

        public MeshTweener(MeshFilter filter) 
        {
            _filter = filter;
        }

        private void Init(Mesh[] meshes, Ease ease, float dur, TweenWrapMode mode)
        {
            _meshes = meshes;
            _ease = ease;
            _dur = dur;
            this.WrapMode = mode;

            // Make sure all meshes are assigned!
            for (int i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i] == null)
                {
                    throw new System.Exception("MeshMorpher mesh  " + i + " has not been setup correctly");
                }
            }

            //  At least two meshes
            if (_meshes.Length < 2)
            {
                throw new System.Exception("The mesh morpher needs at least 2 source meshes");
            }

            _filter.sharedMesh = _meshes[0];
            var mesh = _filter.mesh;
            int vertexCount = mesh.vertexCount;
            for (int i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i].vertexCount != vertexCount)
                {
                    throw new System.Exception("Mesh " + i + " doesn't have the same number of vertices as the first mesh");
                }
            }
        }

        #endregion

        #region Properties

        public override object Target
        {
            get { return _filter; }
        }

        public new MeshFilter TargetMeshFilter { get { return _filter; } }

        public override float TotalDuration
        {
            get { return _dur; }
        }

        #endregion

        #region Methods

        protected override void DoUpdate(float dt, float ct)
        {
            if (_ease != null) ct = _ease(ct, 0, _dur, _dur);
            SetMorph(ct);
        }


        /// Set the current morph in    
        public void SetComplexMorph(int srcIndex, int dstIndex, float t)
        {
            if (m_SrcMesh == srcIndex && m_DstMesh == dstIndex && Mathf.Approximately(m_Weight, t))
                return;

            Vector3[] v0 = _meshes[srcIndex].vertices;
            Vector3[] v1 = _meshes[dstIndex].vertices;
            Vector3[] vdst = new Vector3[_filter.mesh.vertexCount];
            for (int i = 0; i < vdst.Length; i++)
                vdst[i] = Vector3.Lerp(v0[i], v1[i], t);

            _filter.mesh.vertices = vdst;
            _filter.mesh.RecalculateBounds();
        }

        /// t is between 0 and m_Meshes.Length - 1.
        /// 0 means the first mesh, m_Meshes.Length - 1 means the last mesh.
        /// 0.5 means half of the first mesh and half of the second mesh.
        public void SetMorph(float t)
        {
            int floor = (int)t;
            floor = Mathf.Clamp(floor, 0, _meshes.Length - 2);
            float fraction = t - floor;
            fraction = Mathf.Clamp(t - floor, 0.0F, 1.0F);
            SetComplexMorph(floor, floor + 1, fraction);
        }

        #endregion

    }
}
