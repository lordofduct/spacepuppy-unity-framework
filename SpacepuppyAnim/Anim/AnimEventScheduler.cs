using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Anim
{
    public class AnimEventScheduler : System.IDisposable
    {

        #region Fields

        private ISPAnim _state;

        private RadicalCoroutine _waitRoutine;

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
            if (_waitRoutine != null && _waitRoutine.Active) _waitRoutine.Stop();
        }
        
        public void Schedule(System.Action<ISPAnim> callback)
        {
            //if (!_state.IsPlaying) throw new System.InvalidOperationException("Can only schedule a callback on a playing animation.");
            if (callback == null) throw new System.ArgumentNullException("callback");

            if (_endOfLineCallback == null) _endOfLineCallback = new CallbackInfo() { timeout = float.PositiveInfinity };
            _endOfLineCallback.callback += callback;

            if (_waitRoutine == null || _waitRoutine.Finished) this.InitWaitRoutine();
            if (!_waitRoutine.Active) _waitRoutine.Start(_state.Controller);
        }

        public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier timeSupplier)
        {
            //if (!_state.IsPlaying) throw new System.InvalidOperationException("Can only schedule a callback on a playing animation.");
            if (callback == null) throw new System.ArgumentNullException("callback");

            if(timeout == float.PositiveInfinity)
            {
                if (_endOfLineCallback == null) _endOfLineCallback = new CallbackInfo() { timeout = float.PositiveInfinity };
                _endOfLineCallback.callback += callback;
            }
            else
            {
                var info = _pool.GetInstance();
                info.callback = callback;
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

            if (_waitRoutine == null || _waitRoutine.Finished) this.InitWaitRoutine();
            if (!_waitRoutine.Active) _waitRoutine.Start(_state.Controller);
        }

        private System.Collections.IEnumerator DoUpdate()
        {
            while(true)
            {
                yield return null;
                this.Update();
            }
        }

        private void Update()
        {
            _inUpdate = true;
            if(!_state.IsPlaying)
            {
                //close them all down
                this.CloseOutAllEventCallbacks();
            }
            else
            {
                if(_timeoutInfos != null && _timeoutInfos.Count > 0)
                {
                    var e = _timeoutInfos.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.timeout -= e.Current.supplier.Delta;
                        if(e.Current.timeout <= 0f)
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
                while(e.MoveNext())
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
            if(!this.ContainsWaitHandles())
            {
                if (_waitRoutine != null && _waitRoutine.Active) _waitRoutine.Stop();
            }
        }

        private bool ContainsWaitHandles()
        {
            if (_endOfLineCallback != null && _endOfLineCallback.callback != null) return true;
            return _timeoutInfos.Count != 0;
        }



        private void InitWaitRoutine()
        {
            _waitRoutine = new RadicalCoroutine(this.DoUpdate()); //RadicalCoroutine.UpdateTicker(this.Update);
            _waitRoutine.OnFinished += (s, e) =>
            {
                if (object.ReferenceEquals(s, _waitRoutine)) _waitRoutine = null;

                _inUpdate = true;
                this.CloseOutAllEventCallbacks();
                _inUpdate = false;
            };
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
                info.timeout = 0f;
                info.supplier = null;
            });

        private class CallbackInfo
        {
            public System.Action<ISPAnim> callback;
            public float timeout;
            public ITimeSupplier supplier;
        }

        #endregion

    }
}
