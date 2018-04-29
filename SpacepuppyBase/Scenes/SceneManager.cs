using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{

    public interface ISceneManager : IService
    {

        event System.EventHandler<LoadSceneWaitHandle> BeforeSceneLoaded;
        event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        event System.EventHandler<SceneLoadedEventArgs> SceneLoaded;
        event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;
        
        LoadSceneWaitHandle LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);
        LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);

        AsyncOperation UnloadScene(Scene scene);
        Scene GetActiveScene();

        /// <summary>
        /// Test if a scene by the name exists.
        /// </summary>
        /// <param name="excludeInactive">False to test if the scene exists as a loadable scene, True if to test if the scene exists and is actively loaded.</param>
        /// <returns></returns>
        bool SceneExists(string sceneName, bool excludeInactive = false);

    }
    
    public class SPSceneManager : ServiceScriptableObject<ISceneManager>, ISceneManager
    {

        #region Fields
        
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

        public event System.EventHandler<LoadSceneWaitHandle> BeforeSceneLoaded;
        public event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        public event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        public event System.EventHandler<SceneLoadedEventArgs> SceneLoaded;
        public event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;
        
        public LoadSceneWaitHandle LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async)
        {
            switch (behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        SceneManager.LoadScene(sceneName, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        op.allowSceneActivation = false;
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
            }

            throw new System.InvalidOperationException("Unsupported LoadSceneBehaviour.");
        }

        public LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async)
        {
            if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) throw new System.IndexOutOfRangeException("sceneBuildIndex");
            
            string sceneName = "#" + sceneBuildIndex.ToString();

            switch (behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        SceneManager.LoadScene(sceneBuildIndex, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        op.allowSceneActivation = false;
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
            }

            throw new System.InvalidOperationException("Unsupported LoadSceneBehaviour.");
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

        public bool SceneExists(string sceneName, bool excludeInactive = false)
        {
            if(excludeInactive)
            {
                var sc = SceneManager.GetSceneByName(sceneName);
                return sc.IsValid();
            }
            else
            {
                return SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0;
            }
        }
        
        #endregion

        #region EventHandlers

        protected virtual void OnBeforeSceneLoaded(LoadSceneWaitHandle handle)
        {
            var d = this.BeforeSceneLoaded;
            if (d == null) return;

            d(this, handle);
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

            _loadArgs = e;
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
