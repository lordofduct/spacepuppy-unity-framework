using System;

namespace com.spacepuppy.Async
{

    /// <summary>
    /// Facilitates the multi-threading feature of RadicalCoroutine. Access the static interface to easily 
    /// jump between a pooled thread and the main unity thread.
    /// 
    /// yield RadicalTask.JumpToAsync to operate on a pooled thread
    /// yield RadicalTask.JumpToUnityThread (or anything for that matter) to return to the unity thread
    /// 
    /// If you yield a JumpToAsync in an already threaded routine, it'll just remain in the thread, with no wait.
    /// </summary>
    public sealed class RadicalTask : IRadicalYieldInstruction
    {

        public enum OperationState
        {
            Inactive,
            Initializing,
            RunningAsync,
            WaitingOnYield
        }


        #region Fields

        private OperationState _state;
        private RadicalCoroutine _routine;
        private object _yieldObject;

        #endregion

        #region CONSTRUCTOR
        
        public RadicalTask(RadicalCoroutine routine)
        {
            this.Init(routine);
        }

        #endregion

        #region Properties

        public OperationState State
        {
            get
            {
                return _state;
            }
        }
        
        #endregion

        #region Methods

        private void Init(RadicalCoroutine routine)
        {
            if (_state != OperationState.Inactive) throw new InvalidOperationException("RadicalTask is already operating.");

            _routine = routine;
        }

        private void Clear()
        {
            _state = OperationState.Inactive;
            _routine = null;
            _yieldObject = null;
        }


        private void AsyncWorkerCallback(object state)
        {
            _state = OperationState.RunningAsync;

            if (_state == OperationState.Inactive || 
                _routine == null || 
                _routine.OperatingState != RadicalCoroutineOperatingState.Active || 
                _routine.OperationStack.CurrentOperation != this)
            {
                this.Clear();
                return;
            }
            
        WorkerLoopback:
            var op = _routine.OperationStack.PeekSubOperation();
            if (op == null)
            {
                this.Clear();
                return;
            }
            
            object yieldObject;
            if(op.Tick(out yieldObject))
            {
                if(yieldObject == RadicalTask.JumpToAsync && 
                   _state == OperationState.RunningAsync && 
                   _routine != null && 
                   _routine.OperatingState == RadicalCoroutineOperatingState.Active &&
                   _routine.OperationStack.CurrentOperation == this)
                {
                    goto WorkerLoopback;
                }
                else
                {
                    _yieldObject = yieldObject;
                    _state = OperationState.WaitingOnYield;
                }
            }
            else
            {
                if(_routine.OperationStack.PeekSubOperation() == op) _routine.OperationStack.PopSubOperation();
                _yieldObject = null;
                _state = OperationState.Inactive;
            }
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get
            {
                return _state == OperationState.Inactive;
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            switch(_state)
            {
                case OperationState.Inactive:
                    if (_routine != null && SPThreadPool.QueueUserWorkItem(this.AsyncWorkerCallback))
                    {
                        _state = OperationState.Initializing;
                        yieldObject = null;
                        return true;
                    }
                    else
                    {
                        yieldObject = null;
                        return false;
                    }
                case OperationState.Initializing:
                case OperationState.RunningAsync:
                    yieldObject = null;
                    return true;
                case OperationState.WaitingOnYield:
                    yieldObject = _yieldObject;
                    this.Clear();
                    return true;
                default:
                    yieldObject = null;
                    return false;
            }
        }

        #endregion


        #region Static Coroutine Hooks

        private static object _jumpToAsync = new object();
        private static object _jumpToUnityThreadCommand = new object();
        
        public static object JumpToAsync
        {
            get { return _jumpToAsync; }
        }

        public static object JumpToUnityThread
        {
            get { return _jumpToUnityThreadCommand; }
        }



        public static RadicalTask Create(RadicalCoroutine routine)
        {
            //TODO - possibly implment pooling system to reduce gc
            return new RadicalTask(routine);
        }

        #endregion

    }
}
