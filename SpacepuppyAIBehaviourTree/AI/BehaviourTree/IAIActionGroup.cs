using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.BehaviourTree
{

    public interface IAIActionGroup : IAINode, IEnumerable<IAIAction>
    {

        int ActionCount { get; }
        
    }

    public abstract class AIActionGroup : AIAction, IAIAction, IAIActionGroup
    {

        #region Methods

        public abstract void SetActions(IEnumerable<IAIAction> actions);

        #endregion

        #region IAIActionGroup Interface

        public abstract int ActionCount { get; }

        protected abstract IEnumerable<IAIAction> GetActions();

        public IEnumerator<IAIAction> GetEnumerator()
        {
            var e = this.GetActions();
            if (e == null) return Enumerable.Empty<IAIAction>().GetEnumerator();

            return e.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Static Factory

        private static AIActionGroup _nullGrp;
        public static AIActionGroup Null
        {
            get
            {
                if (_nullGrp == null) _nullGrp = new NullAIActionGroup();
                return _nullGrp;
            }
        }

        public static AIActionGroup CreateGroup(ActionGroupType type, IEnumerable<IAIAction> actions, ParallelPassOptions parPassOptions = 0)
        {
            switch (type)
            {
                case ActionGroupType.Sequential:
                    return new SequenceAction(actions);
                case ActionGroupType.Selector:
                    return new SelectorAction(actions);
                case ActionGroupType.Parrallel:
                    return new ParallelAction(actions) { PassOptions = parPassOptions };
                case ActionGroupType.Random:
                    return new RandomAction(actions);
                default:
                    throw new System.ArgumentException("Unknown ActionGroupType.");
            }
        }

        public static AIActionGroup SyncActions(ActionGroupType type, AIActionGroup group, IEnumerable<IAIAction> actions, ParallelPassOptions parPassOptions = 0)
        {
            var repeat = (group != null) ? group.Repeat : RepeatMode.Never;
            var always = (group != null) ? group.AlwaysSucceed : false;

            switch (type)
            {
                case ActionGroupType.Sequential:
                    {
                        if (group is SequenceAction)
                            group.SetActions(actions);
                        else
                            group = new SequenceAction(actions);
                        break;
                    }
                case ActionGroupType.Selector:
                    {
                        if (group is SelectorAction)
                            group.SetActions(actions);
                        else
                            group = new SelectorAction(actions);
                        break;
                    }
                case ActionGroupType.Parrallel:
                    {
                        if (group is ParallelAction)
                        {
                            group.SetActions(actions);
                            (group as ParallelAction).PassOptions = parPassOptions;
                        }
                        else
                            group = new ParallelAction(actions) { PassOptions = parPassOptions };
                        break;
                    }
                case ActionGroupType.Random:
                    {
                        if (group is RandomAction)
                            group.SetActions(actions);
                        else
                            group = new RandomAction(actions);
                        break;
                    }
                default:
                    throw new System.ArgumentException("Unknown ActionGroupType.");
            }

            group.Repeat = repeat;
            group.AlwaysSucceed = always;
            return group;
        }

        public static AIActionGroup SyncActions(ActionGroupType type, AIActionGroup group, GameObject source, bool findWeightSupplier, ParallelPassOptions parPassOptions = 0)
        {
            var repeat = (group != null) ? group.Repeat : RepeatMode.Never;
            var always = (group != null) ? group.AlwaysSucceed : false;

            //var actions = source.GetComponentsAlt<IAIAction>();
            using (var actions = com.spacepuppy.Collections.TempCollection.GetList<IAIAction>())
            {
                source.GetComponents<IAIAction>(actions, (c) =>
                {
                    if (c is IAIAction) return c as IAIAction;
                    else if (c is com.spacepuppy.Scenario.ITriggerableMechanism) return new TriggerableMechanismAsAIActionWrapper(c as com.spacepuppy.Scenario.ITriggerableMechanism);
                    else return null;
                });

                switch (type)
                {
                    case ActionGroupType.Sequential:
                        {
                            if (group is SequenceAction)
                                group.SetActions(actions);
                            else
                                group = new SequenceAction(actions);
                            break;
                        }
                    case ActionGroupType.Selector:
                        {
                            if (group is SelectorAction)
                                group.SetActions(actions);
                            else
                                group = new SelectorAction(actions);
                            break;
                        }
                    case ActionGroupType.Parrallel:
                        {
                            if (group is ParallelAction)
                            {
                                group.SetActions(actions);
                                (group as ParallelAction).PassOptions = parPassOptions;
                            }
                            else
                                group = new ParallelAction(actions) { PassOptions = parPassOptions };
                            break;
                        }
                    case ActionGroupType.Random:
                        {
                            if (group is RandomAction)
                                group.SetActions(actions);
                            else
                                group = new RandomAction(actions);

                            if (findWeightSupplier)
                            {
                                IAIActionWeightSupplier supplier;
                                if (source.GetComponent<IAIActionWeightSupplier>(out supplier))
                                {
                                    (group as RandomAction).WeightSupplier = supplier;
                                }
                            }
                            break;
                        }
                    default:
                        throw new System.ArgumentException("Unknown ActionGroupType.");
                }
            }

            group.Repeat = repeat;
            group.AlwaysSucceed = always;
            return group;
        }

        #endregion

        #region Special Types

        private class NullAIActionGroup : AIActionGroup
        {

            public override int ActionCount { get { return 0; } }

            public override void SetActions(IEnumerable<IAIAction> actions)
            {
            }

            protected override IEnumerable<IAIAction> GetActions()
            {
                return Enumerable.Empty<IAIAction>();
            }

            protected override ActionResult OnTick(IAIController ai)
            {
                return ActionResult.Success;
            }
        }

        #endregion

    }
}
