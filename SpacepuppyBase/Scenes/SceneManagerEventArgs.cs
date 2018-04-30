using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{
    
    public class SceneUnloadedEventArgs : System.EventArgs
    {

        public SceneUnloadedEventArgs(Scene scene)
        {
            this.Scene = scene;
        }

        public Scene Scene
        {
            get;
            set;
        }

    }
    
    public class ActiveSceneChangedEventArgs : System.EventArgs
    {

        public ActiveSceneChangedEventArgs(Scene lastScene, Scene nextScene)
        {
            this.LastScene = lastScene;
            this.NextScene = nextScene;
        }

        public Scene LastScene
        {
            get;
            set;
        }

        public Scene NextScene
        {
            get;
            set;
        }

    }

}
