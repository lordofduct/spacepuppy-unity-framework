#pragma warning disable 0618 // deprecated enum entry
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public enum SPInputAxis : sbyte
    {
        Unknown = -1,
        Axis1 = 0,
        Axis2 = 1,
        Axis3 = 2,
        Axis4 = 3,
        Axis5 = 4,
        Axis6 = 5,
        Axis7 = 6,
        Axis8 = 7,
        Axis9 = 8,
        Axis10 = 9,
        Axis11 = 10,
        Axis12 = 11,
        Axis13 = 12,
        Axis14 = 13,
        Axis15 = 14,
        Axis16 = 15,
        Axis17 = 16,
        Axis18 = 17,
        Axis19 = 18,
        Axis20 = 19,
        Axis21 = 20,
        Axis22 = 21,
        Axis23 = 22,
        Axis24 = 23,
        Axis25 = 24,
        Axis26 = 25,
        Axis27 = 26,
        Axis28 = 27,
        MouseAxis1 = 28,
        MouseAxis2 = 29,
        MouseAxis3 = 30
    }

    public enum SPInputButton : sbyte
    {
        Unknown = -1,
        Button0 = 0,
        Button1 = 1,
        Button2 = 2,
        Button3 = 3,
        Button4 = 4,
        Button5 = 5,
        Button6 = 6,
        Button7 = 7,
        Button8 = 8,
        Button9 = 9,
        Button10 = 10,
        Button11 = 11,
        Button12 = 12,
        Button13 = 13,
        Button14 = 14,
        Button15 = 15,
        Button16 = 16,
        Button17 = 17,
        Button18 = 18,
        Button19 = 19,
        MouseButton0 = 20,
        MouseButton1 = 21,
        MouseButton2 = 22,
        MouseButton3 = 23,
        MouseButton4 = 24,
        MouseButton5 = 25,
        MouseButton6 = 26
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
