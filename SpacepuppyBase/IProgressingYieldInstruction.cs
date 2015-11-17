

namespace com.spacepuppy
{

    /// <summary>
    /// Implement this if your RadicalYieldInstruction should have a progress property.
    /// </summary>
    public interface IProgressingYieldInstruction : IRadicalYieldInstruction
    {
        float Progress { get; }
    }


    /// <summary>
    /// Base abstract class that implements IProgressingYieldInstruction. Inherit from this instead of directly implementing 
    /// IProgressingYieldInstruction, unless you can't otherwise.
    /// </summary>
    public class ProgressingYieldInstructionQueue : IProgressingYieldInstruction
    {

        #region Fields

        private IProgressingYieldInstruction[] _operations;
        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public ProgressingYieldInstructionQueue(params IProgressingYieldInstruction[] operations)
        {
            if (operations == null) throw new System.ArgumentNullException("operations");
            _operations = operations;
        }

        #endregion

        #region IProgressingAsyncOperation Interface

        public float Progress
        {
            get
            {
                if (_operations.Length == 0) return 1f;
                else if (_operations.Length == 1) return _operations[0].Progress;
                else
                {
                    float p = 0f;
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        p += _operations[i].Progress;
                    }
                    return p / _operations.Length;
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                if (_complete) return true;
                else
                {
                    for (int i = 0; i < _operations.Length; i++)
                    {
                        if (!_operations[i].IsComplete) return false;
                    }
                    _complete = true;
                    return true;
                }
            }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            for (int i = 0; i < _operations.Length; i++)
            {
                if (!_operations[i].IsComplete)
                {
                    return _operations[i].Tick(out yieldObject);
                }
            }
            yieldObject = null;
            return false;
        }

        #endregion

    }

}
