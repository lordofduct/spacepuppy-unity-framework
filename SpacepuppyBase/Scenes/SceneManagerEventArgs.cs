using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Scenes
{
    
    public class BeforeSceneLoadedEventArgs : System.EventArgs
    {
        public BeforeSceneLoadedEventArgs(string sceneName, LoadSceneMode mode, IProgressingYieldInstruction async)
        {
            this.SceneName = sceneName;
            this.Mode = mode;
            this.AsyncHandle = async;
        }
        
        public string SceneName
        {
            get;
            set;
        }

        public LoadSceneMode Mode
        {
            get;
            set;
        }

        public IProgressingYieldInstruction AsyncHandle
        {
            get;
            set;
        }

        public bool IsAsync
        {
            get { return AsyncHandle != null; }
        }
        
    }

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

    public class SceneLoadedEventArgs : System.EventArgs
    {

        private SceneLoadedEventArgs()
        {
            //protect
        }

        public SceneLoadedEventArgs(Scene scene, LoadSceneMode mode)
        {
            this.Scene = scene;
            this.Mode = mode;
        }

        public Scene Scene
        {
            get;
            set;
        }

        public LoadSceneMode Mode
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
