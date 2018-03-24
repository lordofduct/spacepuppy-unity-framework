using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Anim.Blend
{
    public class AnimationBlendGraph : ISPAnim
    {

        #region Fields

        private AnimEventScheduler _scheduler;

        #endregion

        #region CONSTRUCTOR

        #endregion


        #region ISPAnim Interface

        public SPAnimationController Controller
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public int Layer
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public float Speed
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public ITimeSupplier TimeSupplier
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public WrapMode WrapMode
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public bool IsPlaying
        {
            get { throw new System.NotImplementedException(); }
        }

        public float Time
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public float Duration
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public float ScaledDuration
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public void Play(QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            throw new System.NotImplementedException();
        }

        public void CrossFade(float fadeLength, QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer)
        {
            throw new System.NotImplementedException();
        }
        
        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Schedule(System.Action<ISPAnim> callback)
        {
            if (_scheduler == null) _scheduler = new AnimEventScheduler(this);
            _scheduler.Schedule(callback);
        }

        public void Schedule(System.Action<ISPAnim> callback, float timeout, ITimeSupplier supplier)
        {
            if (_scheduler == null) _scheduler = new AnimEventScheduler(this);
            _scheduler.Schedule(callback, timeout, supplier);
        }

        #endregion

        #region IRadicalWaitHandle Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get { return !this.IsPlaying; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return this.IsPlaying;
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            if (callback == null) throw new System.ArgumentNullException("callback");
            if (!this.IsPlaying) throw new System.InvalidOperationException("Can not wait for complete on an already completed IRadicalWaitHandle.");

            this.Schedule((a) => callback(this));
        }

        bool IRadicalWaitHandle.Cancelled
        {
            get { return false; }
        }

        #endregion

        #region IDisposable Interface

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (_scheduler != null) _scheduler.Dispose();
            this.IsDisposed = true;
        }

        #endregion

    }
}
