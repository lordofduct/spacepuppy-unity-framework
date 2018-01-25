using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput
{
    public static class InputUtil
    {

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
