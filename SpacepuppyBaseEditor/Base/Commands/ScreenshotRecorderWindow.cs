using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Commands
{

    public sealed class ScreenshotRecorderWindow : EditorWindow
    {

        #region Singleton

        private static ScreenshotRecorderWindow _window;

        private void OnEnable()
        {
            if (_window == null) _window = this;
            else Object.DestroyImmediate(this);

            this.titleContent = new GUIContent("Screenshot Recorder");
        }

        private void OnDisable()
        {
            if (_window == this) _window = null;
        }

        #endregion

        #region Menu Entries

        [MenuItem("Tools/Screenshot Recorder")]
        private static void OpenFromMenu()
        {
            if (_window == null)
            {
                _window = EditorWindow.GetWindow<ScreenshotRecorderWindow>();
                _window.Show();
                _window.position = new Rect(20, 80, 500, 300);
            }
            else
            {
                _window.Focus();
            }
        }

        #endregion

        #region Fields

        public static string DefaultOutputFolder
        {
            get
            {
                return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "UnityImgRecorderOutput");
            }
        }

        private string _filePath = DefaultOutputFolder;
        private float _frequency = 1f;
        private ScreenshotUtil.ImageFormat _format;
        private bool _timestamp = true;
        private bool _recording;

        private float _lastRecordTime;
        private string _status = "Idle...";
        private int _frameCount;

        #endregion

        #region Methods

        private void OnGUI()
        {
            _filePath = EditorGUILayout.TextField("File Path:", _filePath);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Browse...", GUILayout.Width(75f)))
            {
                _filePath = EditorUtility.OpenFolderPanel("Select Folder", _filePath, string.Empty);
            }
            GUILayout.EndHorizontal();
            _timestamp = EditorGUILayout.ToggleLeft("Timestamp", _timestamp);

            _format = (ScreenshotUtil.ImageFormat)EditorGUILayout.EnumPopup("Format: ", _format);
            _frequency = EditorGUILayout.FloatField("Frequency/sec:", _frequency);
            if (_frequency <= 0.0001f) _frequency = 0.0001f;

            if(GUILayout.Button(_recording ? "Stop" : "Record"))
            {
                _recording = !_recording;
                _lastRecordTime = Time.unscaledTime;
                _frameCount = 0;
                _status = "Idle...";
            }

            EditorGUILayout.LabelField("Status: ", _status);
        }

        private void Update()
        {
            if (!_recording) return;

            float t = Time.unscaledTime - _lastRecordTime;
            if(t > (1f / _frequency))
            {
                _frameCount++;
                _lastRecordTime = Time.unscaledTime;

                string fn;
                if(_timestamp)
                    fn = string.Format("Image{0}{1:yyyyMMdd_HHmmss_fff}{2}", _frameCount, System.DateTime.Now, ScreenshotUtil.GetExtension(_format));
                else
                    fn = string.Format("Image{0}{1}", _frameCount, ScreenshotUtil.GetExtension(_format));

                ScreenshotUtil.TakeScreenshot(System.IO.Path.Combine(_filePath, fn), _format);
            }
        }

        #endregion

    }

}
