using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.SPInput
{

    public class CompositeDualAxleInputSignature : IDualAxleInputSignature
    {

        #region Fields

        private string _id;
        private IAxleInputSignature _horizontal;
        private IAxleInputSignature _vertical;

        #endregion

        #region CONSTRUCTOR

        public CompositeDualAxleInputSignature(string id, IAxleInputSignature horizontal, IAxleInputSignature vertical)
        {
            _id = id;
            _horizontal = horizontal;
            _vertical = vertical;
        }

        #endregion

        #region Properties

        public IAxleInputSignature Horizontal
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public IAxleInputSignature Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        #endregion

        #region IDualAxleInputSignature Interface

        public string Id
        {
            get { return _id; }
        }

        public Vector2 CurrentState
        {
            get
            {
                var v = new Vector2((_horizontal != null) ? _horizontal.CurrentState : 0f,
                                   (_vertical != null) ? _vertical.CurrentState : 0f);
                return InputUtil.CutoffDualAxis(v, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }

        public float Precedence
        {
            get;
            set;
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public float RadialDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff RadialCutoff
        {
            get;
            set;
        }

        public void Update()
        {
            if (_horizontal != null) _horizontal.Update();
            if (_vertical != null) _vertical.Update();
        }

        public void FixedUpdate()
        {
            if (_horizontal != null) _horizontal.FixedUpdate();
            if (_vertical != null) _vertical.FixedUpdate();
        }

        public void Reset()
        {
            if (_horizontal != null) _horizontal.Reset();
            if (_vertical != null) _vertical.Reset();
        }

        #endregion

    }

}
