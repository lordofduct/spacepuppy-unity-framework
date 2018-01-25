using System;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Sequences
{
    public class FirstSuccessfulSequence : ISequence
    {

        #region Fields

        private List<ISequence> _sequences = new List<ISequence>();

        #endregion

        #region CONSTRUCTOR

        public FirstSuccessfulSequence(params ISequence[] sequences)
        {
            _sequences.AddRange(sequences);
        }

        public FirstSuccessfulSequence(IEnumerable<ISequence> sequences)
        {
            _sequences.AddRange(sequences);
        }

        #endregion

        #region Properties

        public List<ISequence> Sequences { get { return _sequences; } }

        #endregion

        #region Methods

        #endregion

        #region ISequence Interface

        public void OnStart()
        {
            for(int i = 0; i < _sequences.Count; i++)
            {
                _sequences[i].OnStart();
            }
        }

        public bool Update()
        {
            if (_sequences.Count == 0) return true;

            for(int i = 0; i < _sequences.Count; i++)
            {
                if (_sequences[i].Update()) return true;
            }

            return false;
        }

        #endregion

    }
}
