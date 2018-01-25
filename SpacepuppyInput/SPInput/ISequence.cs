using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Collections;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// Represents a sequence that can be registered with the GameInputManager.
    /// </summary>
    public interface ISequence
    {

        /// <summary>
        /// Called by the GameInputManager when the sequence is first registered.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Sequence polling Update
        /// </summary>
        /// <returns>Returns true when sequence is complete</returns>
        bool Update();

    }


    public class SequenceManager
    {

        #region Fields

        private HashSet<ISequence> _sequences = new HashSet<ISequence>();
        private HashSet<ISequence> _fixedSequences = new HashSet<ISequence>();

        #endregion

        #region Methods

        public void RegisterSequence(ISequence sequence, bool useFixedUpdate = false)
        {
            bool b = (useFixedUpdate) ? _fixedSequences.Add(sequence) : _sequences.Add(sequence);
            if (b) sequence.OnStart();
        }

        public void Update()
        {
            if (_sequences.Count > 0)
            {
                using (var set = TempCollection.GetSet<ISequence>())
                {
                    var e2 = _sequences.GetEnumerator();
                    while (e2.MoveNext())
                    {
                        if (e2.Current.Update()) set.Add(e2.Current);
                    }

                    if (set.Count > 0)
                    {
                        e2 = set.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            _sequences.Remove(e2.Current);
                        }
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (_fixedSequences.Count > 0)
            {
                using (var set = TempCollection.GetSet<ISequence>())
                {
                    var e2 = _fixedSequences.GetEnumerator();
                    while (e2.MoveNext())
                    {
                        if (e2.Current.Update()) set.Add(e2.Current);
                    }

                    if (set.Count > 0)
                    {
                        e2 = set.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            _fixedSequences.Remove(e2.Current);
                        }
                    }
                }
            }
        }

        #endregion

    }

}
