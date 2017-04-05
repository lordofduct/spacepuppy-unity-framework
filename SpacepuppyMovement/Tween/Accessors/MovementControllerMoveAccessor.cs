using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Accessors
{

    /// <summary>
    /// Subverts MoveAccessor.
    /// </summary>
    [CustomTweenMemberAccessor(typeof(MovementController), "Move")]
    [CustomTweenMemberAccessor(typeof(MovementController), "*Move")]
    [CustomTweenMemberAccessor(typeof(Rigidbody), "*Move", priority=1)]
    [CustomTweenMemberAccessor(typeof(GameObject), "*Move", priority = 1)]
    [CustomTweenMemberAccessor(typeof(Transform), "*Move", priority = 1)]
    [CustomTweenMemberAccessor(typeof(IGameObjectSource), "*Move", priority = 1)]
    public class MovementControllerMoveAccessor : ITweenMemberAccessor
    {
        
        #region Fields

        private bool _atypicalMove;

        #endregion
        
        #region Properties

        public bool UseAtypicalMove
        {
            get { return _atypicalMove; }
            set { _atypicalMove = value; }
        }

        #endregion

        #region ImplicitCurve Interface

        public System.Type Init(string propName, string args)
        {
            _atypicalMove = ConvertUtil.ToBool(args);
            return typeof(Vector3);
        }

        public object Get(object target)
        {
            var t = GameObjectUtil.GetTransformFromSource(target);
            if(t != null)
            {
                return t.position;
            }
            return Vector3.zero;
        }

        public void Set(object targ, object valueObj)
        {
            var value = ConvertUtil.ToVector3(valueObj);

            var controller = targ as MovementController;
            if (controller != null && !controller.IsPaused)
            {
                var pos = controller.transform.position;
                if (_atypicalMove)
                    controller.AtypicalMove(value - pos);
                else
                    controller.Move(value - pos);
            }
            else if (targ is Rigidbody)
            {
                var rb = targ as Rigidbody;
                rb.MovePosition(value - rb.position);
            }
            else
            {
                var trans = GameObjectUtil.GetTransformFromSource(targ);
                if (trans == null) return;

                controller = trans.GetComponent<MovementController>();
                if (controller != null && !controller.IsPaused)
                {
                    var pos = trans.position;
                    if (_atypicalMove)
                        controller.AtypicalMove(value - pos);
                    else
                        controller.Move(value - pos);
                    return;
                }

                var rb = trans.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    var dp = value - rb.position;
                    rb.velocity = dp / Time.fixedDeltaTime;
                    return;
                }

                //just update the position
                trans.position = value;
            }
        }

        #endregion

    }
}
