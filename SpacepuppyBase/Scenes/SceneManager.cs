using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{

    /*
     * NOTES - some information to keep me reminded about scenes.
     * 
     * The Scene struct is really just a wrapper around an int/handle. All calls and comparisons reach through. 
     * So even though it's a struct, it acts more like a reference type.
     * 
     */


    public interface ISceneManager : IService
    {

        event System.EventHandler<LoadSceneWaitHandle> BeforeSceneLoaded;
        event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        event System.EventHandler<LoadSceneWaitHandle> SceneLoaded;
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
        private ActiveSceneChangedEventArgs _activeChangeArgs;

        private Dictionary<Scene, LoadSceneWaitHandle> _table = new Dictionary<Scene, LoadSceneWaitHandle>();

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
        public event System.EventHandler<LoadSceneWaitHandle> SceneLoaded;
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
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        op.allowSceneActivation = false;
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, op);
                        return handle;
                    }
            }

            throw new System.InvalidOperationException("Unsupported LoadSceneBehaviour.");
        }

        public LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async)
        {
            if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) throw new System.IndexOutOfRangeException("sceneBuildIndex");

            //string sceneName = "#" + sceneBuildIndex.ToString();
            string sceneName = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);

            switch (behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        SceneManager.LoadScene(sceneBuildIndex, mode);
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        this.OnBeforeSceneLoaded(handle);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        op.allowSceneActivation = false;
                        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                        _table[scene] = handle;
                        handle.Init(scene, op);
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
            LoadSceneWaitHandle handle;
            if (_table.TryGetValue(scene, out handle))
            {
                _table.Remove(scene);
            }
            var d = this.SceneLoaded;
            if (d == null) return;

            if(handle == null)
            {
                handle = new LoadSceneWaitHandle(scene.name, mode, LoadSceneBehaviour.Standard);
                handle.Init(scene, null);
            }
            
            d(this, handle);
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
