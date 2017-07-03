using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{

    public interface ISceneManager : IService
    {

        event System.EventHandler<BeforeSceneLoadedEventArgs> BeforeSceneLoaded;
        event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        event System.EventHandler<SceneLoadedEventArgs> SceneLoaded;
        event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;

        Scene LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
        LoadSceneAsyncWaitHandle LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
        AsyncOperation UnloadScene(Scene scene);
        Scene GetActiveScene();
    }
    
    public class SPSceneManager : ServiceScriptableObject<ISceneManager>, ISceneManager
    {

        #region Fields

        private BeforeSceneLoadedEventArgs _beforeArgs;
        private SceneUnloadedEventArgs _unloadArgs;
        private SceneLoadedEventArgs _loadArgs;
        private ActiveSceneChangedEventArgs _activeChangeArgs;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            SceneManager.sceneUnloaded += this.OnSceneUnloaded;
            SceneManager.sceneLoaded += this.OnSceneLoaded;
            SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
            SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
        }

        #endregion

        #region ISceneManager Interface

        public event System.EventHandler<BeforeSceneLoadedEventArgs> BeforeSceneLoaded;
        public event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        public event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        public event System.EventHandler<SceneLoadedEventArgs> SceneLoaded;
        public event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;

        public Scene LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            this.OnBeforeSceneLoaded(sceneName, mode, null);
            SceneManager.LoadScene(sceneName, mode);
            return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        public LoadSceneAsyncWaitHandle LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = new LoadSceneAsyncWaitHandle();
            this.OnBeforeSceneLoaded(sceneName, mode, handle);
            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            handle.Init(op, SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            return handle;
        }

        public AsyncOperation UnloadScene(Scene scene)
        {
            this.OnBeforeSceneUnloaded(scene);
            return SceneManager.UnloadSceneAsync(scene);
        }

        public Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }

        #endregion

        #region EventHandlers

        protected virtual void OnBeforeSceneLoaded(string sceneName, LoadSceneMode mode, IProgressingYieldInstruction async)
        {
            var d = this.BeforeSceneLoaded;
            if (d == null) return;

            var e = _beforeArgs;
            _beforeArgs = null;
            if (e == null)
                e = new BeforeSceneLoadedEventArgs(sceneName, mode, async);
            else
            {
                e.SceneName = sceneName;
                e.Mode = mode;
                e.AsyncHandle = async;
            }

            d(this, e);

            _beforeArgs = e;
            _beforeArgs.SceneName = null;
            _beforeArgs.Mode = default(LoadSceneMode);
            _beforeArgs.AsyncHandle = null;
        }

        protected virtual void OnBeforeSceneUnloaded(Scene scene)
        {
            var d = this.BeforeSceneUnloaded;
            if (d == null) return;

            var e = _unloadArgs;
            _unloadArgs = null;
            if (e == null)
                e = new SceneUnloadedEventArgs(scene);
            else
                e.Scene = scene;

            d(this, e);

            _unloadArgs = e;
            _unloadArgs.Scene = default(Scene);
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
            var d = this.SceneUnloaded;
            if (d == null) return;

            var e = _unloadArgs;
            _unloadArgs = null;
            if (e == null)
                e = new SceneUnloadedEventArgs(scene);
            else
                e.Scene = scene;

            d(this, e);

            _unloadArgs = e;
            _unloadArgs.Scene = default(Scene);
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var d = this.SceneLoaded;
            if (d == null) return;

            var e = _loadArgs;
            _loadArgs = null;
            if (e == null)
                e = new SceneLoadedEventArgs(scene, mode);
            else
            {
                e.Scene = scene;
                e.Mode = mode;
            }

            d(this, e);

            _loadArgs = null;
            _loadArgs.Scene = default(Scene);
            _loadArgs.Mode = default(LoadSceneMode);
        }

        protected virtual void OnActiveSceneChanged(Scene lastScene, Scene nextScene)
        {
            var d = this.ActiveSceneChanged;
            if (d == null) return;

            var e = _activeChangeArgs;
            _activeChangeArgs = null;
            if (e == null)
                e = new ActiveSceneChangedEventArgs(lastScene, nextScene);
            else
            {
                e.LastScene = lastScene;
                e.NextScene = nextScene;
            }

            d(this, e);

            _activeChangeArgs = e;
            _activeChangeArgs.LastScene = default(Scene);
            _activeChangeArgs.NextScene = default(Scene);
        }

        #endregion

    }

}
