using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Anim
{
    public class AnimEventScheduler : System.IDisposable, IUpdateable
    {

        #region Fields

        private ISPAnim _state;
        
        private CallbackInfo _endOfLineCallback;
        private HashSet<CallbackInfo> _timeoutInfos = new HashSet<CallbackInfo>();
        private ITempCollection<CallbackInfo> _toAddOrRemove;
        private bool _inUpdate = false;

        #endregion

        #region CONSTRUCTOR

        public AnimEventScheduler(ISPAnim state)
        {
            if (state == null) throw new System.ArgumentNullException("state");
            _state = state;
        }

        #endregion

        #region Properties

        internal ISPAnim Owner { get { return _state; } }

        #endregion

        #region Methods

        public void Clear()
        {
            if (_endOfLineCallback != null) _endOfLineCallback.callback = null;
            if (_timeoutInfos != null && _timeoutInfos.Count > 0)
            {
                if (_inUpdate)
                {
                    if (_toAddOrRemove == null) _toAddOrRemove = TempCollection.GetList<CallbackInfo>();

                    var e = _timeoutInfos.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.callback = null;
                        _toAddOrRemove.Add(e.Current);
                    }
                }
                else
                {
                    var e = _timeoutInfos.GetEnumerator();
                    while (e.MoveNext())
                    {
                        _pool.Release(e.Current);
                    }
                    _timeoutInfos.Clear();
                }
            }
            GameLoopEntry.UpdatePump.Remove(this);
        }
        
        public void Schedule(System.Action<ISPAnim> callback)
        {
            //if (!_state.IsPlaying) throw new System.InvalidOperationException("Can only schedule a callback on a playing animation.");
            if (callback == null) throw new System.ArgumentNullException("callback");

            if (_endOfLineCallback == null) _endOfLineCallback = new CallbackInfo() { timeout = float.NaN };
            _endOfLineCallback.callback += callback;

            GameLoopEntry.UpdatePump.Add(this);
        }

        public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier timeSupplier)
        {
            //if (!_state.IsPlaying) throw new System.InvalidOperationException("Can only schedule a callback on a playing animation.");
            if (callback == null) throw new System.ArgumentNullException("callback");

            if(float.IsPositiveInfinity(timeout) || float.IsNaN(timeout))
            {
                if (_endOfLineCallback == null) _endOfLineCallback = new CallbackInfo() { timeout = float.NaN };
                _endOfLineCallback.callback += callback;
            }
            else
            {
                var info = _pool.GetInstance();
                info.callback = callback;
                info.current = 0f;
                info.timeout = timeout;
                info.supplier = (timeSupplier != null) ? timeSupplier : SPTime.Normal;
                if (_inUpdate)
                {
                    if (_toAddOrRemove == null) _toAddOrRemove = TempCollection.GetList<CallbackInfo>();
                    _toAddOrRemove.Add(info);
                }
                else
                {
                    _timeoutInfos.Add(info);
                }
            }

            GameLoopEntry.UpdatePump.Add(this);
        }
        
        private bool ContainsWaitHandles()
        {
            if (_endOfLineCallback != null && _endOfLineCallback.callback != null) return true;
            return _timeoutInfos.Count != 0;
        }
        
        private void CloseOutAllEventCallbacks()
        {
            if (_endOfLineCallback != null && _endOfLineCallback.callback != null)
            {
                var a = _endOfLineCallback.callback;
                _endOfLineCallback.callback = null;
                try
                {
                    a(_state);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            if (_timeoutInfos != null && _timeoutInfos.Count > 0)
            {
                var e = _timeoutInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.callback(_state);
                    _pool.Release(e.Current);
                }
                _timeoutInfos.Clear();
            }
        }

        #endregion

        #region IUpdateable Interface

        void IUpdateable.Update()
        {
            _inUpdate = true;
            if (TestAnimComplete(_state))
            {
                //close them all down
                this.CloseOutAllEventCallbacks();
            }
            else
            {
                if (_timeoutInfos != null && _timeoutInfos.Count > 0)
                {
                    var e = _timeoutInfos.GetEnumerator();
                    while (e.MoveNext())
                    {
                        e.Current.current += e.Current.supplier.Delta;
                        if (e.Current.current >= e.Current.timeout)
                        {
                            var a = e.Current.callback;
                            e.Current.callback = null;
                            if (_toAddOrRemove == null) _toAddOrRemove = TempCollection.GetList<CallbackInfo>();
                            _toAddOrRemove.Add(e.Current);

                            try
                            {
                                a(_state);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogException(ex);
                            }
                        }
                    }
                }
            }
            _inUpdate = false;

            //check if any timeouts wanted to get registered while calling update
            if (_toAddOrRemove != null)
            {
                var e = _toAddOrRemove.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.callback == null)
                    {
                        //I set it null in the loop above to flag it as 'remove' rather than 'add'
                        _timeoutInfos.Remove(e.Current);
                        _pool.Release(e.Current);
                    }
                    else
                    {
                        _timeoutInfos.Add(e.Current);
                    }
                }
                _toAddOrRemove.Dispose();
                _toAddOrRemove = null;
            }


            //stop us
            if (!this.ContainsWaitHandles())
            {
                GameLoopEntry.UpdatePump.Remove(this);
            }
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.Clear();
        }

        #endregion

        #region Static Interface

        private static ObjectCachePool<CallbackInfo> _pool = new ObjectCachePool<CallbackInfo>(-1, 
            () => new CallbackInfo(),
            (info) =>
            {
                info.callback = null;
                info.current = 0f;
                info.timeout = 0f;
                info.supplier = null;
            });

        private class CallbackInfo
        {
            public System.Action<ISPAnim> callback;
            public float current;
            public float timeout;
            public ITimeSupplier supplier;
        }

        public static bool TestAnimComplete(ISPAnim anim)
        {
            if (anim == null) throw new System.ArgumentNullException("anim");

            if (!anim.IsPlaying) return true;

            var a = anim as SPAnim;
            if(a != null)
            {
                if (a.Controller == null || !a.Controller.isActiveAndEnabled) return true;

                switch(a.WrapMode)
                {
                    case WrapMode.Default:
                    case WrapMode.Once:
                        return Time.time > (a.StartTime + a.ScaledDuration);
                    case WrapMode.Loop:
                    case WrapMode.PingPong:
                    case WrapMode.ClampForever:
                    default:
                        return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
