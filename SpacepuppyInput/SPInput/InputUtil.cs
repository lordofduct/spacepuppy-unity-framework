using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput
{
    public static class InputUtil
    {

        public const float DEFAULT_AXLEBTNDEADZONE = 0.707f;

        public static ButtonState ConsumeButtonState(ButtonState current)
        {
            switch(current)
            {
                case ButtonState.Released:
                case ButtonState.None:
                    return ButtonState.None;
                case ButtonState.Down:
                case ButtonState.Held:
                    return ButtonState.Held;
                default:
                    return ButtonState.None;
            }
        }

        public static ButtonState GetNextButtonState(ButtonState current, bool isButtonActive)
        {
            if (isButtonActive)
            {
                switch (current)
                {
                    case ButtonState.None:
                    case ButtonState.Released:
                        return ButtonState.Down;
                    case ButtonState.Down:
                    case ButtonState.Held:
                        return ButtonState.Held;
                }
            }
            else
            {
                switch (current)
                {
                    case ButtonState.None:
                    case ButtonState.Released:
                        return ButtonState.None;
                    case ButtonState.Down:
                    case ButtonState.Held:
                        return ButtonState.Released;
                }
            }

            return ButtonState.None;
        }

        public static float CutoffAxis(float value, float deadzone, DeadZoneCutoff cutoff)
        {
            if (deadzone < 0f) deadzone = 0f;

            //no larger than 1
            if (Mathf.Abs(value) > 1f) return Mathf.Sign(value);

            switch (cutoff)
            {
                case DeadZoneCutoff.Scaled:
                    if (Mathf.Abs(value) < deadzone) return 0f;
                    return Mathf.Sign(value) * (Mathf.Abs(value) - deadzone) / (1f - deadzone);
                case DeadZoneCutoff.Shear:
                    return (Mathf.Abs(value) < deadzone) ? 0f : value;
                default:
                    return value;
            }
        }

        public static Vector2 CutoffDualAxis(Vector2 value, float axleDeadzone, DeadZoneCutoff axleCutoff, float radialDeadzone, DeadZoneCutoff radialCutoff)
        {
            if (axleDeadzone < 0f) axleDeadzone = 0f;
            if (radialDeadzone < 0f) radialDeadzone = 0f;

            if (axleDeadzone > 0f)
            {
                value.x = CutoffAxis(value.x, axleDeadzone, axleCutoff);
                value.y = CutoffAxis(value.y, axleDeadzone, axleCutoff);
            }

            //no larger than 1
            if (value.sqrMagnitude > 1f) return value.normalized;

            if (radialDeadzone > 0f)
            {
                switch (radialCutoff)
                {
                    case DeadZoneCutoff.Scaled:
                        if (value.sqrMagnitude < radialDeadzone * radialDeadzone) return Vector2.zero;
                        value = value.normalized * (value.magnitude - radialDeadzone) / (1f - radialDeadzone);
                        break;
                    case DeadZoneCutoff.Shear:
                        if (value.sqrMagnitude < radialDeadzone * radialDeadzone) value = Vector2.zero;
                        break;
                }
            }

            return value;
        }

        #region Button Duration Methods

        /// <summary>
        /// Returns true if button was pressed down and released in less than duration.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonTapped(this IInputDevice device, string id, float duration)
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState == ButtonState.Released && (Time.realtimeSinceStartup - sig.LastDownTime) <= duration;
        }

        /// <summary>
        /// Returns true if button was pressed down and released in less than duration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonTapped<T>(this IMappedInputDevice<T> device, T id, float duration) where T : struct, System.IConvertible
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState == ButtonState.Released && (Time.realtimeSinceStartup - sig.LastDownTime) <= duration;
        }

        /// <summary>
        /// Returns true if button was pressed down and released after duration.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonPressed(this IInputDevice device, string id, float duration)
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState == ButtonState.Released && (Time.realtimeSinceStartup - sig.LastDownTime) >= duration;
        }

        /// <summary>
        /// Returns true if button was pressed down and released after duration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonPressed<T>(this IMappedInputDevice<T> device, T id, float duration) where T : struct, System.IConvertible
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState == ButtonState.Released && (Time.realtimeSinceStartup - sig.LastDownTime) >= duration;
        }

        /// <summary>
        /// Returns true if button was pressed and is still held down after duration.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonHeld(this IInputDevice device, string id, float duration)
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState > ButtonState.None && (Time.realtimeSinceStartup - sig.LastDownTime) >= duration;
        }

        /// <summary>
        /// Returns true if button was pressed and is still held down after duration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool GetButtonHeld<T>(this IMappedInputDevice<T> device, T id, float duration) where T : struct, System.IConvertible
        {
            if (device == null) return false;

            var sig = device.GetSignature(id) as IButtonInputSignature;
            if (sig == null) return false;

            return sig.CurrentState > ButtonState.None && (Time.realtimeSinceStartup - sig.LastDownTime) >= duration;
        }

        #endregion




        public static bool TestCursorOver(Camera cursorCamera, Vector2 cursorPos, out Collider collider, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            collider = null;
            if (cursorCamera == null) return false;

            var ray = cursorCamera.ScreenPointToRay(cursorPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask, query))
            {
                collider = hit.collider;
                return true;
            }

            return false;
        }

        public static bool TestCursorOverEntity(Camera cursorCamera, Vector2 cursorPos, out SPEntity entity, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            Collider c;
            if(TestCursorOver(cursorCamera, cursorPos, out c, maxDistance, layerMask, query))
            {
                entity = SPEntity.Pool.GetFromSource(c);
                return !object.ReferenceEquals(entity, null);
            }
            else
            {
                entity = null;
                return false;
            }
        }

        public static bool TestCursorOverEntity<T>(Camera cursorCamera, Vector2 cursorPos, out T hitTarget, float maxDistance = float.PositiveInfinity, int layerMask = Physics.AllLayers, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal) where T : class
        {
            Collider c;
            if (TestCursorOver(cursorCamera, cursorPos, out c, maxDistance, layerMask, query))
            {
                hitTarget = com.spacepuppy.Utils.ComponentUtil.FindComponent<T>(c);
                return !object.ReferenceEquals(hitTarget, null);
            }
            else
            {
                hitTarget = null;
                return false;
            }
        }

    }
}
