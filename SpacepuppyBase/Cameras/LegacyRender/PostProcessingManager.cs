using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace com.spacepuppy.Cameras.LegacyRender
{

    public interface IPostProcessingManager : IService
    {

        void SetDirty();
        
        /// <summary>
        /// Applies all global post processing effects.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="callback"></param>
        /// <returns>Returns true if effects were processed.</returns>
        bool ApplyGlobalPostProcessing(ICamera camera, RenderTexture source, RenderTexture destination);

    }

    public class PostProcessingManager : ServiceComponent<IPostProcessingManager>, IPostProcessingManager
    {

        #region Fields

        private GlobalEffectsList _globalEffects;

        private bool _dirty;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            base.OnValidAwake();

            _globalEffects = new GlobalEffectsList(this);
            CameraPool.CameraRegistered += OnRegistered;
            CameraPool.CameraUnregistered += OnUnregistered;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CameraPool.CameraRegistered -= OnRegistered;
            CameraPool.CameraUnregistered -= OnUnregistered;
        }

        #endregion

        #region Properties

        public IList<IPostProcessingEffect> GlobalEffects
        {
            get { return _globalEffects; }
        }

        #endregion

        #region Methods

        private void EnableGlobalHooks()
        {
            var e = CameraPool.GetEnumerator();
            while(e.MoveNext())
            {
                GlobalPostProcessorHook.EnableHook(e.Current);
            }
            _dirty = false;
        }

        private void DisableGlobalHooks()
        {
            var e = CameraPool.GetEnumerator();
            while (e.MoveNext())
            {
                GlobalPostProcessorHook.DisableHook(e.Current);
            }
            _dirty = false;
        }

        #endregion

        #region Event Handlers

        private void OnRegistered(object sender, CameraRegistrationEvent e)
        {
            if(_globalEffects.Count > 0)
            {
                GlobalPostProcessorHook.EnableHook(e.Camera);
            }
            else
            {
                _dirty = true;
            }
        }

        private void OnUnregistered(object sender, CameraRegistrationEvent e)
        {
            if (e.Camera != null) GlobalPostProcessorHook.DisableHook(e.Camera);
        }

        #endregion

        #region IPostProcessingManager Interface

        public void SetDirty()
        {
            _dirty = true;
        }
        
        public bool ApplyGlobalPostProcessing(ICamera camera, RenderTexture source, RenderTexture destination)
        {
            if (_globalEffects.Count == 0) return false;

            using (var set = com.spacepuppy.Collections.TempCollection.GetSet<RenderTexture>())
            {
                var src = source;

                for (int i = 0; i < _globalEffects.Count; i++)
                {
                    var effect = _globalEffects[i];
                    if (effect != null)
                    {
                        var dest = RenderTexture.GetTemporary(src.width, src.height);
                        set.Add(dest);

                        effect.RenderImage(src, dest);

                        src = dest;
                    }
                }

                Graphics.Blit(src, destination);

                var e = set.GetEnumerator();
                while(e.MoveNext())
                {
                    RenderTexture.ReleaseTemporary(e.Current);
                }
            }
                
            return true;
        }

        #endregion

        #region Special Types

        private class GlobalEffectsList : IList<IPostProcessingEffect>
        {

            #region Fields

            private PostProcessingManager _owner;
            private List<IPostProcessingEffect> _lst = new List<IPostProcessingEffect>();

            #endregion

            #region CONSTRUCTOR

            public GlobalEffectsList(PostProcessingManager owner)
            {
                _owner = owner;
            }

            #endregion

            #region Properties

            public int Count
            {
                get { return _lst.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IPostProcessingEffect this[int index]
            {
                get { return _lst[index]; }
                set
                {
                    if (value == null) throw new System.ArgumentNullException("value");
                    _lst[index] = value;
                }
            }

            #endregion

            #region Methods

            public void Add(IPostProcessingEffect item)
            {
                if (item == null) throw new System.ArgumentNullException("item");

                _lst.Add(item);
                if (_owner._dirty)
                {
                    _owner.EnableGlobalHooks();
                }
            }

            public void Insert(int index, IPostProcessingEffect item)
            {
                if (item == null) throw new System.ArgumentNullException("item");

                _lst.Insert(index, item);
                if(_owner._dirty)
                {
                    _owner.EnableGlobalHooks();
                }
            }

            public bool Remove(IPostProcessingEffect item)
            {
                if(_lst.Remove(item))
                {
                    if(_lst.Count == 0)
                    {
                        _owner.DisableGlobalHooks();
                    }
                    return true;
                }

                return false;
            }

            public void RemoveAt(int index)
            {
                _lst.RemoveAt(index);
                if(_lst.Count == 0)
                {
                    _owner.DisableGlobalHooks();
                }
            }

            public void Clear()
            {
                if (_lst.Count == 0) return;

                _lst.Clear();
                _owner.DisableGlobalHooks();
            }

            public bool Contains(IPostProcessingEffect item)
            {
                return _lst.Contains(item);
            }

            public void CopyTo(IPostProcessingEffect[] array, int arrayIndex)
            {
                _lst.CopyTo(array, arrayIndex);
            }

            public int IndexOf(IPostProcessingEffect item)
            {
                return _lst.IndexOf(item);
            }

            public IEnumerator<IPostProcessingEffect> GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _lst.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }
}
