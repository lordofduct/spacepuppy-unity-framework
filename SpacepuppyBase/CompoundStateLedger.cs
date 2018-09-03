#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Dynamic.Accessors;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Allows stacking modifiers onto a specific stat/property in a tracked manner. For example if you want to increase the max health of a HealthMeter temporarily.
    /// </summary>
    public class CompoundStateLedger : SPComponent
    {

        #region Fields

        [System.NonSerialized]
        private Dictionary<Token, TargetLedger> _table = new Dictionary<Token, TargetLedger>();

        #endregion

        #region CONSTRUCTOR
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach(var e in _table)
            {
                e.Value.Dispose();
            }
            _table.Clear();
        }

        #endregion

        #region Methods

        public TargetLedger TryGetLedger(Component target, string memberName)
        {
            if (target == null || target.gameObject != this.gameObject) return null;

            var token = new Token(target, memberName);
            TargetLedger result;
            if (_table.TryGetValue(token, out result))
                return result;
            else
                return null;
        }

        #endregion

        #region Static Members

        public static TargetLedger GetLedger(Component target, string memberName)
        {
            if (target == null) return null;

            var ledger = target.GetComponent<CompoundStateLedger>();
            if (ledger == null) ledger = target.AddComponent<CompoundStateLedger>();

            var token = new Token(target, memberName);
            TargetLedger result;
            if (ledger._table.TryGetValue(token, out result)) return result;

            result = TargetLedger.Create(ledger, target, memberName);
            ledger._table[new Token(target, memberName)] = result;
            return result;
        }

        #endregion

        #region Special Types

        private struct Token
        {
            public Component Target;
            public string MemberName;

            public Token(Component targ, string member)
            {
                this.Target = targ;
                this.MemberName = member;
            }

        }

        public class TargetLedger : System.IDisposable
        {

            #region Fields

            private CompoundStateLedger _owner;
            private Component _target;
            private string _memberName;
            private IMemberAccessor _accessor;
            
            private Dictionary<object, double> _entries = new Dictionary<object, double>();
            private double _baseValue;
            private double _balance;

            #endregion

            #region CONSTRUCTOR
            
            private TargetLedger()
            {

            }

            #endregion

            #region Properties

            public double BaseValue
            {
                get { return _baseValue; }
            }

            public double Balance
            {
                get { return _balance; }
            }
            
            #endregion

            #region Methods
            
            public void ApplyModifier(object token, double value)
            {
                _entries[token] = value;
                this.TallyBalance();
            }

            public bool RemoveModifier(object token)
            {
                if (_entries.Remove(token))
                {
                    this.TallyBalance();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Clear()
            {
                _entries.Clear();
                _entries[this] = _baseValue;
                this.TallyBalance();
            }

            /// <summary>
            /// Applies balance to target if this is active.
            /// </summary>
            public void ForceApplyBalance()
            {
                if (_accessor != null) _accessor.Set(_target, _balance);
            }

            public void ResetTargetToBaseValue()
            {
                if (_accessor != null) _accessor.Set(_target, _baseValue);
            }

            /// <summary>
            /// Sets the base value to the current state. This should only be called if you've modified what you'd like the base value to be on the target.
            /// </summary>
            public void ReinitBaseValue()
            {
                if (_accessor != null)
                {
                    _baseValue = ConvertUtil.ToDouble(_accessor.Get(_target));
                    this.ApplyModifier(this, _baseValue);
                }
            }

            private void TallyBalance()
            {
                double total = 0d;
                var e = _entries.GetEnumerator();
                while (e.MoveNext())
                {
                    total += e.Current.Value;
                }
                _balance = total;

                if (_accessor != null)
                {
                    _accessor.Set(_target, _balance);
                }
            }

            #endregion

            #region IDisposable Interface

            public void Dispose()
            {
                _entries.Clear();
                if (_accessor != null) _accessor.Set(_target, _baseValue);
                if (_owner != null)
                {
                    var token = new Token(_target, _memberName);
                    TargetLedger obj;
                    if (_owner._table.TryGetValue(token, out obj) && obj == this)
                        _owner._table.Remove(token);
                }

                _owner = null;
                _target = null;
                _memberName = null;
                _accessor = null;
                _baseValue = 0d;
                _baseValue = 0d;
            }

            #endregion

            #region Factory

            internal static TargetLedger Create(CompoundStateLedger owner, Component target, string memberName)
            {
                if (target == null || string.IsNullOrEmpty(memberName)) return null;

                var member = DynamicUtil.GetMember(target, memberName, true);
                if (member == null) return null;
                
                var entry = new TargetLedger();
                entry._owner = owner;
                entry._target = target;
                entry._memberName = memberName;
                entry._accessor = new DynamicMemberAccessor(memberName);
                entry.ReinitBaseValue();
                return entry;
            }

            #endregion

        }

        #endregion

    }

}
