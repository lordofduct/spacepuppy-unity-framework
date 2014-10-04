using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{
    public class Trigger : SPComponent
    {

        #region Fields

        [SerializeField()]
        private TriggerTarget[] _targets;

        private TriggerTargetCollection _targetColl;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Properties

        public TriggerTargetCollection Targets
        {
            get
            {
                if (_targetColl == null) _targetColl = new TriggerTargetCollection(this);
                return _targetColl;
            }
        }

        #endregion

        #region Methods

        public void ActivateTrigger()
        {
            if (_targets == null || _targets.Length == 0) return;

            foreach (var targ in _targets)
            {
                var arg = (targ.TriggerableArg != null) ? targ.TriggerableArg.Value : null;
                if (targ != null && targ.Triggerable != null)
                {
                    switch (targ.ActivationType)
                    {
                        case TriggerActivationType.TriggerAllOnTarget:
                            foreach (var t in (from t in targ.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                            {
                                t.Trigger(arg);
                            }
                            break;
                        case TriggerActivationType.TriggerSelectedTarget:
                            if (targ.Triggerable is ITriggerableMechanism)
                            {
                                (targ.Triggerable as ITriggerableMechanism).Trigger(arg);
                            }
                            break;
                        case TriggerActivationType.SendMessage:
                            var go = GameObjectUtil.GetGameObjectFromSource(targ.Triggerable);
                            if (go != null && targ.MethodName != null)
                            {
                                go.SendMessage(targ.MethodName, arg, SendMessageOptions.DontRequireReceiver);
                            }
                            break;
                        case TriggerActivationType.CallMethodOnSelectedTarget:
                            if (targ.Triggerable != null && targ.MethodName != null)
                            {
                                ObjUtil.CallMethod(targ.Triggerable, targ.MethodName, arg);
                            }
                            break;
                    }

                }
            }

        }

        public void ActivateTrigger(object arg)
        {
            if (_targets == null || _targets.Length == 0) return;

            foreach (var targ in _targets)
            {
                if (targ != null && targ.Triggerable != null)
                {
                    switch (targ.ActivationType)
                    {
                        case TriggerActivationType.TriggerAllOnTarget:
                            foreach (var t in (from t in targ.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                            {
                                t.Trigger(arg);
                            }
                            break;
                        case TriggerActivationType.TriggerSelectedTarget:
                            if (targ.Triggerable is ITriggerableMechanism)
                            {
                                (targ.Triggerable as ITriggerableMechanism).Trigger(arg);
                            }
                            break;
                        case TriggerActivationType.SendMessage:
                            var go = GameObjectUtil.GetGameObjectFromSource(targ.Triggerable);
                            if (go != null && targ.MethodName != null)
                            {
                                go.SendMessage(targ.MethodName, arg, SendMessageOptions.DontRequireReceiver);
                            }
                            break;
                        case TriggerActivationType.CallMethodOnSelectedTarget:
                            if (targ.Triggerable != null && targ.MethodName != null)
                            {
                                ObjUtil.CallMethod(targ.Triggerable, targ.MethodName, arg);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Special Types

        public enum TriggerActivationType
        {
            TriggerAllOnTarget = 0,
            TriggerSelectedTarget = 1,
            SendMessage = 2,
            CallMethodOnSelectedTarget = 3
        }

        [System.Serializable()]
        public class TriggerTarget
        {

            [ComponentTypeRestriction(typeof(ITriggerableMechanism), order = 1)]
            public Component Triggerable;
            public VariantReference TriggerableArg;
            public TriggerActivationType ActivationType;
            public string MethodName;

        }

        public class TriggerTargetCollection : ICollection<TriggerTarget>
        {

            private Trigger _owner;

            internal TriggerTargetCollection(Trigger owner)
            {
                _owner = owner;
            }

            #region Methods

            public TriggerTarget AddNew()
            {
                if (_owner._targets != null)
                {
                    System.Array.Resize(ref _owner._targets, _owner._targets.Length + 1);
                }
                else
                {
                    _owner._targets = new TriggerTarget[1];
                }
                var targ = new TriggerTarget();
                _owner._targets[_owner._targets.Length - 1] = targ;
                return targ;
            }

            public TriggerTarget AddNew(ITriggerableMechanism mechanism, object arg = null, bool triggerAllOnTarget = true)
            {
                if (mechanism == null) throw new System.ArgumentNullException("mechanism");

                var targ = this.AddNew();
                targ.Triggerable = mechanism.component;
                targ.TriggerableArg = new VariantReference()
                {
                    Value = arg
                };

                return targ;
            }

            #endregion

            #region ICollection Interface

            public void Add(TriggerTarget item)
            {
                if (_owner._targets == null)
                {
                    _owner._targets = new TriggerTarget[] { item };
                }
                else if (!_owner._targets.Contains(item))
                {
                    System.Array.Resize(ref _owner._targets, _owner._targets.Length);
                    _owner._targets[_owner._targets.Length - 1] = item;
                }
            }

            public void Clear()
            {
                _owner._targets = null;
            }

            public bool Contains(TriggerTarget item)
            {
                if (_owner._targets == null) return false;
                return System.Array.IndexOf(_owner._targets, item) >= 0;
            }

            public void CopyTo(TriggerTarget[] array, int arrayIndex)
            {
                if (_owner._targets != null)
                {
                    _owner._targets.CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get { return (_owner._targets != null) ? _owner._targets.Length : 0; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TriggerTarget item)
            {
                if (_owner._targets == null) return false;

                if (_owner._targets.Contains(item))
                {
                    var lst = new List<TriggerTarget>(_owner._targets);
                    lst.Remove(item);
                    _owner._targets = lst.ToArray();
                    return true;
                }

                return false;
            }

            public IEnumerator<TriggerTarget> GetEnumerator()
            {
                if (_owner._targets == null) return System.Linq.Enumerable.Empty<TriggerTarget>().GetEnumerator();

                return (_owner._targets as IEnumerable<TriggerTarget>).GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

        }

        #endregion

    }
}
