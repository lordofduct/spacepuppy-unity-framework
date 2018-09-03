using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Collections
{
    
    /// <summary>
    /// Represents a triggerable state that can be active/inactive. 
    /// If the state can be activated by multiple sources that overlap, this allows tracking how many 
    /// sources have activated the state.
    /// 
    /// For example if a player can be stunned, it might get stunned by multiple mobs at once. If performed as a boolean 
    /// an overlap may occur where it gets stuck in a 'stunned' state due to ordering.
    /// 
    /// This instead allows each mob to register that it is stunning, and the state turns inactive once all mobs have popped themselves from the stack.
    /// </summary>
    public class StateFlagStack : IEnumerable<object>
    {

        public event System.EventHandler ActiveChanged;

        #region Fields

        private HashSet<object> _stack;

        #endregion

        #region CONSTRUCTOR

        public StateFlagStack()
        {
            _stack = new HashSet<object>();
        }

        public StateFlagStack(IEqualityComparer<object> tokenComparer)
        {
            _stack = new HashSet<object>(tokenComparer);
        }

        #endregion

        #region Properties

        public bool Active
        {
            get { return _stack.Count > 0; }
        }

        public int ActiveCount
        {
            get { return _stack.Count; }
        }

        #endregion

        #region Methods

        public void Begin(object token)
        {
            if (token == null) throw new System.ArgumentNullException("token");
            if(_stack.Count == 0)
            {
                _stack.Add(token);
                if(_stack.Count > 0 && this.ActiveChanged != null)
                {
                    var d = this.ActiveChanged;
                    d(this, EventArgs.Empty);
                }
            }
            else
            {
                _stack.Add(token);
            }
        }

        public void End(object token)
        {
            if (token == null) throw new System.ArgumentNullException("token");
            if (_stack.Count == 0) return;

            _stack.Remove(token);
            if(_stack.Count == 0 && this.ActiveChanged != null)
            {
                var d = this.ActiveChanged;
                d(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            if (_stack.Count == 0) return;

            _stack.Clear();
            if (this.ActiveChanged != null)
            {
                var d = this.ActiveChanged;
                d(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IEnumerable Interface
        
        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)_stack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object>)_stack).GetEnumerator();
        }

        #endregion

    }
    

}
