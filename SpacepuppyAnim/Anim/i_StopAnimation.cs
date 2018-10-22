#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Anim;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim
{
    public class i_StopAnimation : TriggerableMechanism
    {

        public enum StopMode
        {
            All,
            Id,
            Layer
        }

        #region Fields

        [SerializeField()]
        private TriggerableTargetObject _targetAnimator;

        [SerializeField]
        private StopMode _mode;

        [SerializeField()]
        private string _id;
        [SerializeField()]
        private int _layer;

        #endregion

        #region Properties

        public TriggerableTargetObject TargetAnimator
        {
            get { return _targetAnimator; }
        }

        public StopMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        #endregion

        #region Methods

        private object ResolveTargetAnimator(object arg)
        {
            var obj = _targetAnimator.GetTarget<UnityEngine.Object>(arg);

            ISPAnimationSource src = null;
            ISPAnimator spanim = null;
            Animation anim = null;

            if (ObjUtil.GetAsFromSource<ISPAnimationSource>(obj, out src))
                return src;
            if (ObjUtil.GetAsFromSource<ISPAnimator>(obj, out spanim))
                return spanim;
            if (ObjUtil.GetAsFromSource<Animation>(obj, out anim))
                return anim;

            if (obj is SPEntity || _targetAnimator.ImplicityReducesEntireEntity)
            {
                var go = obj is SPEntity ? (obj as SPEntity).gameObject : GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;

                SPAnimationController spcont;
                if (go.FindComponent<SPAnimationController>(out spcont))
                    return spcont;

                if (go.FindComponent<Animation>(out anim))
                    return anim;
            }

            /*
            var obj = _targetAnimator.GetTarget<UnityEngine.Object>(arg);
            if (obj == null || obj is SPAnimationController || obj is Animation)
            {
                return obj;
            }
            else if (_targetAnimator.ImplicityReducesEntireEntity)
            {
                var go = GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;

                SPAnimationController spcont;
                if (go.FindComponent<SPAnimationController>(out spcont))
                    return spcont;

                Animation anim;
                if (go.FindComponent<Animation>(out anim))
                    return anim;
            }
            */

            return null;
        }






        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var animator = this.ResolveTargetAnimator(arg);
            if (animator == null) return false;

            switch(_mode)
            {
                case StopMode.All:
                    if(animator is Animation)
                    {
                        (animator as Animation).Stop();
                        return true;
                    }
                    else if(animator is SPAnimationController)
                    {
                        (animator as SPAnimationController).StopAll();
                        return true;
                    }
                    break;
                case StopMode.Id:
                    if (animator is Animation)
                    {
                        (animator as Animation).Stop(_id);
                        return true;
                    }
                    else if (animator is SPAnimationController)
                    {
                        (animator as SPAnimationController).Stop(_id);
                        return true;
                    }
                    break;
                case StopMode.Layer:
                    if (animator is Animation)
                    {
                        (animator as Animation).Stop(_layer);
                        return true;
                    }
                    else if (animator is SPAnimationController)
                    {
                        (animator as SPAnimationController).Stop(_layer);
                        return true;
                    }
                    break;
            }

            return false;
        }

        #endregion




        #region Static Interface

        public static bool IsAcceptibleAnimator(object obj)
        {
            return obj is SPAnimationController || obj is Animation;
        }

        #endregion

    }
}
