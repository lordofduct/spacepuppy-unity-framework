#pragma warning disable 0618 // deprecated enum entry
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public enum SPInputId : byte
    {
        Unknown = 0,
        Axis1 = 1,
        Axis2 = 2,
        Axis3 = 3,
        Axis4 = 4,
        Axis5 = 5,
        Axis6 = 6,
        Axis7 = 7,
        Axis8 = 8,
        Axis9 = 9,
        Axis10 = 10,
        Axis11 = 11,
        Axis12 = 12,
        Axis13 = 13,
        Axis14 = 14,
        Axis15 = 15,
        Axis16 = 16,
        Axis17 = 17,
        Axis18 = 18,
        Axis19 = 19,
        Axis20 = 20,
        Axis21 = 21,
        Axis22 = 22,
        Axis23 = 23,
        Axis24 = 24,
        Axis25 = 25,
        Axis26 = 26,
        Axis27 = 27,
        Axis28 = 28,
        MouseAxis1 = 29,
        MouseAxis2 = 30,
        MouseAxis3 = 31,
        Button0 = 32,
        Button1 = 33,
        Button2 = 34,
        Button3 = 35,
        Button4 = 36,
        Button5 = 37,
        Button6 = 38,
        Button7 = 39,
        Button8 = 40,
        Button9 = 41,
        Button10 = 42,
        Button11 = 43,
        Button12 = 44,
        Button13 = 45,
        Button14 = 46,
        Button15 = 47,
        Button16 = 48,
        Button17 = 49,
        Button18 = 50,
        Button19 = 51,
        MouseButton0 = 52,
        MouseButton1 = 53,
        MouseButton2 = 54,
        MouseButton3 = 55,
        MouseButton4 = 56,
        MouseButton5 = 57,
        MouseButton6 = 58
    }

    public enum SPMouseId
    {
        MouseX = SPInputId.MouseAxis1,
        MouseY = SPInputId.MouseAxis2,
        MouseScroll = SPInputId.MouseAxis3,
        MouseButton0 = SPInputId.MouseButton0,
        MouseButton1 = SPInputId.MouseButton1,
        MouseButton2 = SPInputId.MouseButton2,
        MouseButton3 = SPInputId.MouseButton3,
        MouseButton4 = SPInputId.MouseButton4,
        MouseButton5 = SPInputId.MouseButton5,
        MouseButton6 = SPInputId.MouseButton6
    }

    public enum InputType
    {
        Unknown,
        Joystick,
        Keyboard,
        Custom
    }

    public enum InputMode
    {
        Axis,
        Trigger,
        LongTrigger,
        Button,
        AxleButton
    }

    [System.Flags()]
    public enum TargetPlatform : int
    {
        All = -1,
        Unknown = 0,
        Windows = 1,
        MacOSX = 2,
        Linux = 4,
        IPhone = 8,
        Android = 16,
        WindowsStoreApp = 32,
        Blackberry = 64,
        WebGL = 128,
        PS3 = 256,
        PS4 = 512,
        PSP = 1024,
        Xbox360 = 2048,
        XboxOne = 4096,
        WiiU = 8192,
        Switch = 16384,
        SamsungTV = 32768,
        tvOS = 65536,
        Tizen = 131072
    }

    public static class TargetPlatformUtil
    {

        public static TargetPlatform ToTargetPlatform(this RuntimePlatform platform)
        {
            switch(platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return TargetPlatform.MacOSX;
                case RuntimePlatform.WindowsPlayer:
                    return TargetPlatform.Windows;
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.OSXDashboardPlayer:
                    return TargetPlatform.MacOSX;
                case RuntimePlatform.WindowsWebPlayer:
                case RuntimePlatform.WindowsEditor:
                    return TargetPlatform.Windows;
                case RuntimePlatform.IPhonePlayer:
                    return TargetPlatform.IPhone;
                case RuntimePlatform.PS3:
                    return TargetPlatform.PS3;
                case RuntimePlatform.XBOX360:
                    return TargetPlatform.Xbox360;
                case RuntimePlatform.Android:
                    return TargetPlatform.Android;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return TargetPlatform.Linux;
                case RuntimePlatform.WebGLPlayer:
                    return TargetPlatform.WebGL;
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    return TargetPlatform.WindowsStoreApp;
                case RuntimePlatform.BlackBerryPlayer:
                    return TargetPlatform.Blackberry;
                case RuntimePlatform.TizenPlayer:
                    return TargetPlatform.Tizen;
                case RuntimePlatform.PSP2:
                    return TargetPlatform.PSP;
                case RuntimePlatform.PS4:
                    return TargetPlatform.PS4;
                case RuntimePlatform.PSM:
                    return TargetPlatform.Unknown; //need to add Playstation Mobile...
                case RuntimePlatform.XboxOne:
                    return TargetPlatform.XboxOne;
                case RuntimePlatform.SamsungTVPlayer:
                    return TargetPlatform.SamsungTV;
                case RuntimePlatform.WiiU:
                    return TargetPlatform.WiiU;
                case RuntimePlatform.tvOS:
                    return TargetPlatform.tvOS;
                case RuntimePlatform.Switch:
                    return TargetPlatform.Switch;
                default:
                    return TargetPlatform.Unknown;
            }
        }

    }

}
