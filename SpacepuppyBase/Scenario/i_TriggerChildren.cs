using UnityEngine;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    [System.Obsolete("This thing is bonkers weird, and is a relic of early versions of the t&i system. Ignore this thing at all costs.")]
    public class i_TriggerChildren : AutoTriggerableMechanism
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        private bool _passAlongTriggerArg = true;

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _delay;

        [SerializeField()]
        [Tooltip("If you want to target only a specific child by its name, put that here. Otherwise leave blank to trigger all children.")]
        private string _specificTargetName;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        public string SpecificTargetName
        {
            get { return _specificTargetName; }
            set { _specificTargetName = value; }
        }

        #endregion

        #region Methods

        private void DoTriggerAllChildren(Transform trans, object arg)
        {
            if (trans == null) return;

            if (!string.IsNullOrEmpty(_specificTargetName))
            {
                var child = trans.Find(_specificTargetName);
                if (child != null && child.gameObject.activeInHierarchy)
                {
                    //foreach (var t in (from t in child.GetComponents<ITriggerableMechanism>() where t.CanTrigger orderby t.Order ascending select t))
                    //{
                    //    if (_passAlongTriggerArg)
                    //        t.Trigger(this, arg);
                    //    else
                    //        t.Trigger(this, null);
                    //}
                    using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ITriggerableMechanism>())
                    {
                        child.GetChildComponents<ITriggerableMechanism>(lst);
                        lst.Sort(TriggerableMechanismOrderComparer.Default);
                        var e = lst.GetEnumerator();
                        if(e.MoveNext())
                        {
                            if(e.Current.CanTrigger)
                            {
                                if (_passAlongTriggerArg)
                                    e.Current.Trigger(this, arg);
                                else
                                    e.Current.Trigger(this, null);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Transform child in trans)
                {
                    if (child != null && child.gameObject.activeInHierarchy)
                    {
                        //foreach (var t in (from t in child.GetComponents<ITriggerableMechanism>() where t.CanTrigger orderby t.Order ascending select t))
                        //{
                        //    if (_passAlongTriggerArg)
                        //        t.Trigger(this, arg);
                        //    else
                        //        t.Trigger(this, null);
                        //}
                        using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ITriggerableMechanism>())
                        {
                            child.GetChildComponents<ITriggerableMechanism>(lst);
                            lst.Sort(TriggerableMechanismOrderComparer.Default);
                            var e = lst.GetEnumerator();
                            if (e.MoveNext())
                            {
                                if (e.Current.CanTrigger)
                                {
                                    if (_passAlongTriggerArg)
                                        e.Current.Trigger(this, arg);
                                    else
                                        e.Current.Trigger(this, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Triggerable Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var trans = _target.GetTarget<Transform>(arg);
            if (trans == null) return false;

            if (_delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.DoTriggerAllChildren(trans, arg);
                }, _delay);
            }
            else
            {
                this.DoTriggerAllChildren(trans, arg);
            }

            return true;
        }

        #endregion
        
    }

}
