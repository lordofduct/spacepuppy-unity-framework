using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Timers;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenario
{

    public abstract class AbstractTriggerPoint : com.spacepuppy.SPComponent
    {

        #region Special Types

        [System.Serializable]
        public class TargetTriggerableProps
        {
            public GameObject Target;
            public string TriggerParam;
            public string ExplicitTargetType;

            public ITriggerableMechanism GetTriggerablTarget()
            {
                ITriggerableMechanism trig = null;

                if (StringUtil.IsNotNullOrWhitespace(ExplicitTargetType))
                {
                    var comp = Target.GetComponent(ExplicitTargetType);
                    if (comp is ITriggerableMechanism)
                    {
                        trig = comp as ITriggerableMechanism;
                    }
                    else if (comp == null)
                    {
                        System.Type tp = System.Reflection.Assembly.GetExecutingAssembly().GetType(ExplicitTargetType);
                        if (tp != null)
                        {
                            var arr = Target.GetComponents(tp);
                            if (arr.Length > 0 && arr[0] is ITriggerableMechanism)
                            {
                                trig = arr[0] as ITriggerableMechanism;
                            }
                        }
                    }
                }
                else
                {
                    trig = Target.GetFirstLikeComponent<ITriggerableMechanism>();
                }

                return trig;
            }

            public bool GetCanTrigger()
            {
                var trig = this.GetTriggerablTarget();
                return (trig != null) ? trig.CanTrigger : false;
            }

            public object Trigger()
            {
                if (Target == null) return null;

                ITriggerableMechanism trig = this.GetTriggerablTarget();
                if (trig != null && trig.CanTrigger)
                {
                    return trig.Trigger(TriggerParam);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Fields

        public TargetTriggerableProps[] Targets = { new TargetTriggerableProps() };

        #endregion

        #region Properties

        public int TargetCount
        {
            get { return (Targets != null) ? Targets.Length : 0; }
        }

        #endregion

        #region Methods

        public object Trigger(int i)
        {
            if (Targets == null || !MathUtil.InBounds(i, Targets.Length)) return null;

            return Targets[i].Trigger();
        }

        public void TriggerAll()
        {
            foreach (var targ in Targets)
            {
                targ.Trigger();
            }
        }

        public void TriggerAll(out object[] outvalues)
        {
            var lst = new List<object>();
            foreach (var targ in Targets)
            {
                lst.Add(targ.Trigger());
            }
            outvalues = lst.ToArray();
        }



        public void TriggerWithDelay(int i, float delay, System.Action<object> onTriggered)
        {
            if (delay > 0)
            {
                this.Invoke(() =>
                    {
                        var obj = this.Trigger(i);
                        if (onTriggered != null) onTriggered(obj);
                    }, delay);
            }
            else
            {
                var obj = this.Trigger(i);
                if (onTriggered != null) onTriggered(obj);
            }
        }

        public void TriggerAllWithDelay(float delay, System.Action onTriggered)
        {
            if (delay > 0)
            {
                this.Invoke(() =>
                    {
                        this.TriggerAll();
                        if (onTriggered != null) onTriggered();
                    }, delay);
            }
            else
            {
                this.TriggerAll();
                if (onTriggered != null) onTriggered();
            }
        }

        public void TriggerAllWithDelay(float delay, System.Action<object[]> onTriggered)
        {
            if (delay > 0)
            {
                this.Invoke(() =>
                    {
                        object[] objs;
                        this.TriggerAll(out objs);
                        if (onTriggered != null) onTriggered(objs);
                    }, delay);
            }
            else
            {
                object[] objs;
                this.TriggerAll(out objs);
                if (onTriggered != null) onTriggered(objs);
            }
        }

        #endregion

    }

    //public class OldAbstractTriggerPoint : MonoBehaviour
    //{

    //    #region Fields

    //    public GameObject Target;
    //    public string TriggerParam;
    //    public string ExplicitTargetType;

    //    public float Delay;

    //    #endregion

    //    #region Methods

    //    public void TriggerWithDelay(Action<object> onTriggered)
    //    {
    //        if (Delay > 0)
    //        {
    //            GameTimer.CreateGypsyTimer(Delay, (t) =>
    //            {
    //                var obj = this.Trigger();
    //                if (onTriggered != null) onTriggered(obj);
    //            });

    //        }
    //        else
    //        {
    //            var obj = this.Trigger();
    //            if (onTriggered != null) onTriggered(obj);
    //        }
    //    }

    //    public object Trigger()
    //    {
    //        if (Target == null) return null;

    //        ITriggerable trig = null;

    //        if (StringUtil.IsNotNullOrWhitespace(ExplicitTargetType))
    //        {
        

    //            var comp = Target.GetComponent(ExplicitTargetType);
    //            if (comp is ITriggerable)
    //            {
    //                trig = comp as ITriggerable;
    //            }
    //            else if(comp == null)
    //            {
    //                System.Type tp = System.Reflection.Assembly.GetExecutingAssembly().GetType(ExplicitTargetType);
    //                if (tp != null)
    //                {
    //                    var arr = Target.GetComponents(tp);
    //                    if (arr.Length > 0 && arr[0] is ITriggerable)
    //                    {
    //                        trig = arr[0] as ITriggerable;
    //                    }
    //                }
    //            }


    //        }
    //        else
    //        {
    //            trig = Target.GetFirstComponent<ITriggerable>();
    //        }

    //        if (trig == null) return null;

    //        return trig.Trigger(TriggerParam);
    //    }

    //    #endregion

    //}

}
