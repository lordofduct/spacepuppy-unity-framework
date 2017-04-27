using UnityEngine;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// A singleton Game entry point
    /// </summary>
    public abstract class GameSettings : ScriptableObject
    {
        
        public const string PATH_DEFAULTSETTINGS = "GameSettings";
        public const string PATH_DEFAULTSETTINGS_FULL = @"Assets/Resources/GameSettings.asset";

        #region CONSTRUCTOR

        protected void Awake()
        {
            if (_instance != null && _instance != this)
                throw new System.InvalidOperationException("Attempted to create multiple GameSettings. Please get instances of the game settings via the static interface GameSettingsBase.GetGameSettings.");
        }

        protected abstract void OnInitialized();

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        #endregion

        #region Static Factory

        private static GameSettings _instance;

        public static GameSettings GetGameSettings(string path = null)
        {
            if(_instance == null)
            {
                if (path == null) path = PATH_DEFAULTSETTINGS;
                //_instance = Object.Instantiate(Resources.Load(path)) as GameSettings;
                _instance = Resources.Load(path) as GameSettings;
                if (_instance != null) _instance.OnInitialized();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }

        public static T GetGameSettings<T>(string path = null) where T : GameSettings
        {
            if (_instance == null)
            {
                if (path == null) path = PATH_DEFAULTSETTINGS;
                //_instance = Object.Instantiate(Resources.Load(path)) as T;
                _instance = Resources.Load(path) as T;
                if (_instance != null) _instance.OnInitialized();
                return _instance as T;
            }
            else
            {
                return _instance as T;
            }
        }

        #endregion

    }

}
