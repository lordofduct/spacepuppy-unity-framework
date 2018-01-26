using System;
using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity
{

    public static class SPInputDirect
    {

        public static bool GetButton(SPInputButton button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButton(GetButtonInputId(button, joystick));
        }

        public static bool GetButtonDown(SPInputButton button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButtonDown(GetButtonInputId(button, joystick));
        }

        public static bool GetButtonUp(SPInputButton button, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetButtonUp(GetButtonInputId(button, joystick));
        }

        public static float GetAxis(SPInputAxis axis, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetAxis(GetAxisInputId(axis, joystick));
        }

        public static float GetAxisRaw(SPInputAxis axis, Joystick joystick = Joystick.All)
        {
            return UnityEngine.Input.GetAxisRaw(GetAxisInputId(axis, joystick));
        }





        public static SPInputButton PollButton(Joystick joystick = Joystick.All)
        {
            if (joystick != Joystick.None)
            {
                for (int i = 0; i < (int)SPInputButton.MouseButton6; i++)
                {
                    if (UnityEngine.Input.GetButton(GetButtonInputId((SPInputButton)i, joystick)))
                    {
                        return (SPInputButton)i;
                    }
                }
            }
            return SPInputButton.Unknown;
        }

        public static bool TryPollButton(out SPInputButton button, Joystick joystick = Joystick.All)
        {
            if (joystick != Joystick.None)
            {
                for (int i = 0; i < (int)SPInputButton.MouseButton6; i++)
                {
                    if (UnityEngine.Input.GetButton(GetButtonInputId((SPInputButton)i, joystick)))
                    {
                        button = (SPInputButton)i;
                        return true;
                    }
                }
            }
            button = SPInputButton.Unknown;
            return false;
        }

        public static SPInputAxis PollAxis(Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
                {
                    float v = Input.GetAxis(GetAxisInputId((SPInputAxis)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        return (SPInputAxis)i;
                    }
                }
            }
            return SPInputAxis.Unknown;
        }

        public static bool TryPollAxis(out SPInputAxis axis, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
                {
                    float v = Input.GetAxis(GetAxisInputId((SPInputAxis)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        axis = (SPInputAxis)i;
                        return true;
                    }
                }
            }
            axis = SPInputAxis.Unknown;
            return false;
        }

        public static bool TryPollAxis(out SPInputAxis axis, out float value, Joystick joystick = Joystick.All, float deadZone = InputUtil.DEFAULT_AXLEBTNDEADZONE)
        {
            if (joystick != Joystick.None)
            {
                for (int i = 0; i < (int)SPInputAxis.MouseAxis3; i++)
                {
                    float v = Input.GetAxis(GetAxisInputId((SPInputAxis)i, joystick));
                    if (Mathf.Abs(v) > deadZone)
                    {
                        axis = (SPInputAxis)i;
                        value = v;
                        return true;
                    }
                }
            }
            axis = SPInputAxis.Unknown;
            value = 0f;
            return false;
        }

        public static SPInputAxis[] PollAllAxes(Joystick joystick = Joystick.All)
        {
            if (joystick != Joystick.None)
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

            return com.spacepuppy.Utils.ArrayUtil.Empty<SPInputAxis>();
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

        public static string GetButtonInputId(SPInputButton button, Joystick joystick = Joystick.All)
        {
            if (button == SPInputButton.Unknown) return null;

            if (_buttonToId == null) _buttonToId = new Dictionary<int, string>();
            int hash = (int)joystick | ((int)button << 4);
            string id;
            if (_buttonToId.TryGetValue(hash, out id))
                return id;
            
            if (joystick == Joystick.All)
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

        public static string GetAxisInputId(SPInputAxis axis, Joystick joystick = Joystick.All)
        {
            if (axis == SPInputAxis.Unknown) return null;

            if (_axisToId == null) _axisToId = new Dictionary<int, string>();
            int hash = (int)joystick | ((int)axis << 4);
            string id;
            if (_axisToId.TryGetValue(hash, out id))
                return id;
            
            if (joystick == Joystick.All)
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
