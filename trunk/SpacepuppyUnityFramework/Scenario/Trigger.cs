using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Serializable()]
    public sealed class Trigger : ICollection<TriggerTarget>
    {

        #region Fields

        [SerializeField()]
        private List<TriggerTarget> _targets = new List<TriggerTarget>();

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Methods

        public TriggerTarget AddNew()
        {
            var targ = new TriggerTarget();
            _targets.Add(targ);
            return targ;
        }

        public TriggerTarget AddNew(ITriggerableMechanism mechanism, object arg = null, bool triggerAllOnTarget = true)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");

            var targ = this.AddNew();
            targ.Triggerable = mechanism.component;
            targ.TriggerableArgs = new VariantReference[1] { new VariantReference() {
                                                                Value = arg
                                                            }
            };

            return targ;
        }

        public void ActivateTrigger()
        {
            if (_targets.Count == 0) return;

            foreach (var targ in _targets)
            {
                if (targ != null && targ.Triggerable != null)
                {
                    var arg0 = (targ.TriggerableArgs != null && targ.TriggerableArgs.Length > 0) ? targ.TriggerableArgs[0].Value : null;
                    switch (targ.ActivationType)
                    {
                        case TriggerActivationType.TriggerAllOnTarget:
                            foreach (var t in (from t in targ.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                            {
                                t.Trigger(arg0);
                            }
                            break;
                        case TriggerActivationType.TriggerSelectedTarget:
                            if (targ.Triggerable is ITriggerableMechanism)
                            {
                                (targ.Triggerable as ITriggerableMechanism).Trigger(arg0);
                            }
                            break;
                        case TriggerActivationType.SendMessage:
                            var go = GameObjectUtil.GetGameObjectFromSource(targ.Triggerable);
                            if (go != null && targ.MethodName != null)
                            {
                                go.SendMessage(targ.MethodName, arg0, SendMessageOptions.DontRequireReceiver);
                            }
                            break;
                        case TriggerActivationType.CallMethodOnSelectedTarget:
                            if (targ.MethodName != null)
                            {
                                //CallMethod does not support using the passed in arg
                                var args = (from a in targ.TriggerableArgs select (a != null) ? a.Value : null).ToArray();
                                ObjUtil.CallMethod(targ.Triggerable, targ.MethodName, args);
                            }
                            break;
                    }

                }
            }

        }

        public void ActivateTrigger(object arg)
        {
            if (_targets.Count == 0) return;

            foreach (var targ in _targets)
            {
                if (targ != null && targ.Triggerable != null)
                {
                    var arg0 = (targ.TriggerableArgs != null && targ.TriggerableArgs.Length > 0) ? targ.TriggerableArgs[0].Value : arg;
                    switch (targ.ActivationType)
                    {
                        case TriggerActivationType.TriggerAllOnTarget:
                            foreach (var t in (from t in targ.Triggerable.GetLikeComponents<ITriggerableMechanism>() orderby t.Order ascending select t))
                            {
                                t.Trigger(arg0);
                            }
                            break;
                        case TriggerActivationType.TriggerSelectedTarget:
                            if (targ.Triggerable is ITriggerableMechanism)
                            {
                                (targ.Triggerable as ITriggerableMechanism).Trigger(arg0);
                            }
                            break;
                        case TriggerActivationType.SendMessage:
                            var go = GameObjectUtil.GetGameObjectFromSource(targ.Triggerable);
                            if (go != null && targ.MethodName != null)
                            {
                                go.SendMessage(targ.MethodName, arg0, SendMessageOptions.DontRequireReceiver);
                            }
                            break;
                        case TriggerActivationType.CallMethodOnSelectedTarget:
                            if (targ.MethodName != null)
                            {
                                //CallMethod does not support using the passed in arg
                                var args = (from a in targ.TriggerableArgs select (a != null) ? a.Value : null).ToArray();
                                ObjUtil.CallMethod(targ.Triggerable, targ.MethodName, args);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region ICollection Interface

        public void Add(TriggerTarget item)
        {
            _targets.Add(item);
        }

        public void Clear()
        {
            _targets.Clear();
        }

        public bool Contains(TriggerTarget item)
        {
            return _targets.Contains(item);
        }

        public void CopyTo(TriggerTarget[] array, int arrayIndex)
        {
            _targets.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _targets.Count; }
        }

        bool ICollection<TriggerTarget>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TriggerTarget item)
        {
            return _targets.Remove(item);
        }

        public IEnumerator<TriggerTarget> GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _targets.GetEnumerator();
        }

        #endregion

    }

}
