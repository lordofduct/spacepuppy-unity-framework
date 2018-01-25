using System;
using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity
{

    public static class SPInputDirect
    {

        public static bool GetButton(SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return UnityEngine.Input.GetButton(GetButtonInputId(button, joystick));
        }

        public static bool GetButtonDown(SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return UnityEngine.Input.GetButtonDown(GetButtonInputId(button, joystick));
        }

        public static bool GetButtonUp(SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return UnityEngine.Input.GetButtonUp(GetButtonInputId(button, joystick));
        }

        public static float GetAxis(SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return UnityEngine.Input.GetAxis(GetAxisInputId(axis, joystick));
        }

        public static float GetAxisRaw(SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return UnityEngine.Input.GetAxisRaw(GetAxisInputId(axis, joystick));
        }





        public static SPInputButton PollButton(SPJoystick joystick = SPJoystick.All)
        {
            for (int i = 0; i < (int)SPInputButton.MouseButton6; i++)
            {
                if (UnityEngine.Input.GetButton(GetButtonInputId((SPInputButton)i, joystick)))
                {
                    return (SPInputButton)i;
                }
            }
            return SPInputButton.Unknown;
        }

        public static bool TryPollButton(out SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            button = SPInputButton.Button0;
            for (int i = 0; i < (int)SPInputButton.MouseButton6; i++)
            {
                if (UnityEngine.Input.GetButton(GetButtonInputId((SPInputButton)i, joystick)))
                {
                    button = (SPInputButton)i;
                    return true;
                }
            }
            return false;
        }

        public static SPInputAxis PollAxis(SPJoystick joystick = SPJoystick.All)
        {
            for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
            {
                if (UnityEngine.Input.GetButton(GetAxisInputId((SPInputAxis)i, joystick)))
                {
                    return (SPInputAxis)i;
                }
            }
            return SPInputAxis.Unknown;
        }

        public static bool TryPollAxis(out SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            axis = SPInputAxis.Axis1;
            for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
            {
                if (UnityEngine.Input.GetButton(GetAxisInputId((SPInputAxis)i, joystick)))
                {
                    axis = (SPInputAxis)i;
                    return true;
                }
            }
            return false;
        }

        public static SPInputAxis[] PollAllAxes(SPJoystick joystick = SPJoystick.All)
        {
            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<SPInputAxis>())
            {
                for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
                {
                    if (Mathf.Abs(UnityEngine.Input.GetAxis(GetAxisInputId((SPInputAxis)i, joystick))) > 0.5f)
                    {
                        lst.Add((SPInputAxis)i);
                    }
                }

                return lst.Count > 0 ? lst.ToArray() : com.spacepuppy.Utils.ArrayUtil.Empty<SPInputAxis>();
            }
        }

        public static UnityEngine.KeyCode PollKey()
        {
            if (_allKeyCodes == null) _allKeyCodes = System.Enum.GetValues(typeof(UnityEngine.KeyCode)) as UnityEngine.KeyCode[];

            for (int i = 0; i < _allKeyCodes.Length; i++)
            {
                if (UnityEngine.Input.GetKey(_allKeyCodes[i])) return _allKeyCodes[i];
            }
            return UnityEngine.KeyCode.None;
        }

        public static bool TryPollKey(out UnityEngine.KeyCode key)
        {
            if (_allKeyCodes == null) _allKeyCodes = System.Enum.GetValues(typeof(UnityEngine.KeyCode)) as UnityEngine.KeyCode[];

            key = KeyCode.None;
            for (int i = 0; i < _allKeyCodes.Length; i++)
            {
                if (UnityEngine.Input.GetKey(_allKeyCodes[i]))
                {
                    key = _allKeyCodes[i];
                    return true;
                }
            }
            return false;
        }




        #region ID Lookup

        private static Dictionary<int, string> _buttonToId;
        private static Dictionary<int, string> _axisToId;
        private static UnityEngine.KeyCode[] _allKeyCodes;

        public static string GetButtonInputId(SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            if (button == SPInputButton.Unknown) return null;

            if (_buttonToId == null) _buttonToId = new Dictionary<int, string>();
            int hash = (int)joystick | ((int)button << 4);
            string id;
            if (_buttonToId.TryGetValue(hash, out id))
                return id;
            
            if (joystick == SPJoystick.All)
            {
                if (button <= SPInputButton.Button19)
                    id = string.Format("JoyAll-Button{0:00}", (int)button);
                else
                    id = string.Format("MouseButton{0:0}", (int)button - (int)SPInputButton.MouseButton0);
            }
            else
            {
                if (button <= SPInputButton.Button19)
                    id = string.Format("Joy{0:0}-Button{0:00}", (int)joystick, (int)button);
                else
                    id = string.Format("MouseButton{0:0}", (int)button - (int)SPInputButton.MouseButton0);
            }

            _buttonToId[hash] = id;
            return id;
        }

        public static string GetAxisInputId(SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            if (axis == SPInputAxis.Unknown) return null;

            if (_axisToId == null) _axisToId = new Dictionary<int, string>();
            int hash = (int)joystick | ((int)axis << 4);
            string id;
            if (_axisToId.TryGetValue(hash, out id))
                return id;
            
            if (joystick == SPJoystick.All)
            {
                if (axis <= SPInputAxis.Axis28)
                    id = string.Format("JoyAll-Axis{0:00}", (int)axis + 1);
                else
                    id = string.Format("MouseAxis{0:0}", (int)axis - (int)SPInputAxis.MouseAxis1 + 1);
            }
            else
            {
                if (axis <= SPInputAxis.Axis28)
                    id = string.Format("Joy{0:0}-Axis{0:00}", (int)joystick, (int)axis + 1);
                else
                    id = string.Format("MouseAxis{0:0}", (int)axis - (int)SPInputAxis.MouseAxis1 + 1);
            }

            _axisToId[hash] = id;
            return id;
        }

        #endregion

    }

}
