using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    public abstract class GameSettingsBase : ScriptableObject
    {

        public const string PATH_DEFAULTSETTINGS = "GameSettings";
        public const string PATH_DEFAULTSETTINGS_FULL = @"Assets/Resources/GameSettings.asset";



        #region Static Factory

        public static GameSettingsBase GetGameSettings(string path)
        {
            var settings = Object.Instantiate(Resources.Load(path)) as GameSettingsBase;
            return settings;
        }

        public static T GetGameSettings<T>(string path) where T : GameSettingsBase
        {
            var settings = Object.Instantiate(Resources.Load(path)) as T;
            return settings;
        }

        #endregion

    }

}
