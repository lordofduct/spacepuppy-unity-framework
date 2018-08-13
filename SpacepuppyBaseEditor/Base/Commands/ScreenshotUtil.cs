using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Commands
{

    public static class ScreenshotUtil
    {

        public enum ImageFormat
        {
            PNG = 0,
            Jpeg = 1,
            TGA = 2
        }

        public static TextureFormat TextureFormat = TextureFormat.RGBA32;

        [MenuItem("CONTEXT/Camera/Take Screen Cap (TGA)")]
        private static void ScreenCapFromCameraTGA_ContextMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.TGA, command.context as Camera);
        }

        [MenuItem("CONTEXT/Camera/Take Screen Cap (JPG)")]
        private static void ScreenCapFromCameraJPG_ContextMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.Jpeg, command.context as Camera);
        }

        [MenuItem("CONTEXT/Camera/Take Screen Cap (PNG)")]
        private static void ScreenCapFromCameraPNG_ContextMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.PNG, command.context as Camera);
        }


        [MenuItem("Tools/Screenshot (TGA)")]
        private static void ScreenCapFromCameraTGA_ToolMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.TGA);
        }

        [MenuItem("Tools/Screenshot (JPG)")]
        private static void ScreenCapFromCameraJPG_ToolMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.Jpeg);
        }

        [MenuItem("Tools/Screenshot (PNG)")]
        private static void ScreenCapFromCameraPNG_ToolMenu(MenuCommand command)
        {
            TakeScreenshot(ImageFormat.PNG);
        }

        public static bool TakeScreenshot(ImageFormat format, Camera cam = null)
        {
            var dir = EditorProjectPrefs.Local.GetString("LastScreenshotDirectory", string.Empty);
            string path;
            switch (format)
            {
                case ImageFormat.PNG:
                    path = EditorUtility.SaveFilePanel("Save Screenshot", dir, "Screenshot.png", "png");
                    break;
                case ImageFormat.Jpeg:
                    path = EditorUtility.SaveFilePanel("Save Screenshot", dir, "Screenshot.jpg", "jpg");
                    break;
                case ImageFormat.TGA:
                    path = EditorUtility.SaveFilePanel("Save Screenshot", dir, "Screenshot.tga", "tga");
                    break;
                default:
                    return false;
            }

            if (string.IsNullOrEmpty(path)) return false;
            dir = System.IO.Path.GetDirectoryName(path) + @"\";
            EditorProjectPrefs.Local.SetString("LastScreenShotDirectory", dir);

            return TakeScreenshot(path, format, cam);
        }

        public static bool TakeScreenshot(string path, ImageFormat format, Camera cam = null)
        {
            if (cam != null || format != ImageFormat.PNG)
            {
                Texture2D screenshot;
                if(cam != null)
                {
                    var tmp = RenderTexture.GetTemporary(cam.pixelWidth, cam.pixelHeight);
                    var cache = cam.targetTexture;
                    cam.targetTexture = tmp;
                    cam.Render();

                    RenderTexture.active = tmp;
                    screenshot = new Texture2D(tmp.width, tmp.height, ScreenshotUtil.TextureFormat, false);
                    screenshot.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);

                    RenderTexture.active = null;

                    cam.targetTexture = cache;
                    tmp.Release();
                }
                else
                {
                    screenshot = new Texture2D(Screen.width, Screen.height, ScreenshotUtil.TextureFormat, false);
                    screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    screenshot.Apply();
                }
                
                byte[] bytes;
                switch(format)
                {
                    case ImageFormat.PNG:
                        bytes = screenshot.EncodeToPNG();
                        break;
                    case ImageFormat.Jpeg:
                        bytes = screenshot.EncodeToJPG();
                        break;
                    case ImageFormat.TGA:
                        bytes = screenshot.EncodeToTGA();
                        break;
                    default:
                        UnityEngine.Object.DestroyImmediate(screenshot);
                        return false;
                }
                
                System.IO.File.WriteAllBytes(path, bytes);

                UnityEngine.Object.DestroyImmediate(screenshot);
                return true;
            }
            else
            {
                //Application.CaptureScreenshot(path);
                ScreenCapture.CaptureScreenshot(path);
                return true;
            }
        }

        public static string GetExtension(ImageFormat format)
        {
            switch(format)
            {
                case ImageFormat.PNG:
                    return ".png";
                case ImageFormat.Jpeg:
                    return ".jpg";
                case ImageFormat.TGA:
                    return ".tga";
                default:
                    return string.Empty;
            }
        }
        
    }

}
