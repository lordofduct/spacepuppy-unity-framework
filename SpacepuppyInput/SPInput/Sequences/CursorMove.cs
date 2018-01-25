using UnityEngine;

namespace com.spacepuppy.SPInput.Sequences
{
    public class CursorMove : ISequence
    {

        #region Fields

        private ICursorInputSignature _cursor;
        private float _dist;
        private System.Action _callback;

        private Vector2 _startPos;

        #endregion

        #region CONSTRUCTOR

        public CursorMove(ICursorInputSignature cursor, float dist, System.Action callback)
        {
            _cursor = cursor;
            _dist = dist;
            _callback = callback;
        }
        
        #endregion

        #region ISequence Interface

        public void OnStart()
        {
            if(_cursor != null)
            {
                _startPos = _cursor.CurrentState;
            }
        }

        public bool Update()
        {
            if (_cursor == null) return true;

            if((_cursor.CurrentState - _startPos).sqrMagnitude >= _dist * _dist)
            {
                if (_callback != null) _callback();
                return true;
            }

            return false;
        }

        #endregion

    }
}
