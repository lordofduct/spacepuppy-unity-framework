using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Anim
{

    public interface IAnimControllerMask
    {

        bool CanPlay(IAnimatable anim);
        bool CanPlay(AnimationClip clip, AnimSettings settings);

    }

    [CreateAssetMenu(fileName = "SPAnimControllerLayerMask", menuName = "Spacepuppy/SPAnimControllerLayerMask")]
    public sealed class SPAnimControllerLayerMask : ScriptableObject, IAnimControllerMask
    {

        #region Fields

        [SerializeField]
        [TextArea()]
        [Tooltip("Comma delimited integers or hyphanated ranges: 10,20,100-200,304")]
        [InsertButton("Reload Data", "InitData", PrecedeProperty = false, RuntimeOnly = true)]
        private string _layers;

        [System.NonSerialized]
        private bool _clean = false;
        [System.NonSerialized]
        private HashSet<int> _layerSet = new HashSet<int>();
        [System.NonSerialized]
        private List<Range> _ranges = new List<Range>();

        #endregion
        
        #region Properties

        public string Data
        {
            get { return _layers; }
        }

        #endregion

        #region Methods

        public void SetData(string layers)
        {
            _layers = layers;
            _clean = false;
        }

        private void InitData()
        {
            _layerSet.Clear();
            _ranges.Clear();

            if(!string.IsNullOrEmpty(_layers))
            {
                var arr = _layers.Split(',');
                for(int i = 0; i < arr.Length; i++)
                {
                    if(arr[i].Contains('-'))
                    {
                        var sarr = arr[i].Split('-');
                        int l, h;
                        if(int.TryParse(sarr[0], out l) && int.TryParse(sarr[1], out h))
                        {
                            if(l > h)
                            {
                                int t = l;
                                l = h;
                                h = t;
                            }
                            _ranges.Add(new Range()
                            {
                                Min = l,
                                Max = h
                            });
                        }
                    }
                    else
                    {
                        int l;
                        if(int.TryParse(arr[i].Trim(), out l))
                        {
                            _layerSet.Add(l);
                        }
                    }
                }
            }

            _layerSet.TrimExcess();
            _ranges.TrimExcess();
            _clean = true;
        }

        private bool TestLayer(int layer)
        {
            if (!_clean) this.InitData();

            if (_layerSet.Contains(layer)) return true;
            for (int i = 0; i < _ranges.Count; i++)
            {
                if (layer >= _ranges[i].Min && layer <= _ranges[i].Max) return true;
            }
            return false;
        }

        #endregion

        #region IAnimControllerMask Interface

        public bool CanPlay(IAnimatable anim)
        {
            if (anim == null) return false;
            return TestLayer(anim.Layer);
        }

        public bool CanPlay(AnimationClip clip, AnimSettings settings)
        {
            return TestLayer(settings.layer);
        }

        #endregion
        
        #region Special Types

        private struct Range
        {
            public int Min;
            public int Max;
        }

        #endregion

    }

    [System.Serializable]
    public class AnimControllerMaskSerializedRef : com.spacepuppy.Project.SerializableInterfaceRef<IAnimControllerMask>
    {

    }

}
