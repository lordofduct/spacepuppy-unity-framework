using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{

    public abstract class Scene
    {

        #region Fields

        private string _id;
        private string _name;
        private bool _loaded;

        #endregion

        #region CONSTRUCTOR

        public Scene(string id)
        {
            _id = id;
        }

        #endregion

        #region Properties

        public string Id { get { return _id; } }

        public string Name { get { return _name; } set { _name = value; } }

        public bool Loaded { get { return _loaded; } }

        #endregion

        #region Methods

        public void Load()
        {
            _lastLoadCall = null;

            this.DoLoad();
            Scene.RegisterLoadedScene(this);
        }

        public IProgressingYieldInstruction LoadAsync()
        {
            _lastLoadCall = null;

            var op = this.DoLoadAsync();
            _lastLoadCall = new LoadSceneAsyncOperation();
            _lastLoadCall.Start(this, op);
            return _lastLoadCall;
        }


        protected internal virtual void SetLoaded(bool loaded)
        {
            _loaded = loaded;
        }

        protected internal abstract void DoLoad();

        protected internal abstract IProgressingYieldInstruction DoLoadAsync();

        protected internal abstract void DoLoadAdditive();

        protected internal abstract IProgressingYieldInstruction DoLoadAdditiveAsync();

        #endregion




        #region Static Interface

        private static Scene _currentScene;
        //private static Scene[] _loadedScenes;
        private static LoadSceneAsyncOperation _lastLoadCall;

        public static Scene CurrentScene { get { return _currentScene; } }

        private static void PurgeLoadedScene()
        {
            if (_currentScene != null)
            {
                _currentScene.SetLoaded(false);
                _currentScene = null;
            }

            //_loadedScenes = null;
        }

        private static void RegisterLoadedScene(Scene scene)
        {
            if (_currentScene != null) PurgeLoadedScene();
            _currentScene = scene;
            _currentScene.SetLoaded(true);

            //var lst = new List<Scene>();
            //lst.Add(_currentScene);
            //if(_currentScene is SceneContainer)
            //{
            //    lst.AddRange((_currentScene as SceneContainer).GetAllChildren());
            //}
            //_loadedScenes = lst.ToArray();
        }

        public static IEnumerable<Scene> Find(string id)
        {
            if (_currentScene == null) yield break;

            if (_currentScene.Id == id) yield return _currentScene;
            if (_currentScene is SceneContainer)
            {
                foreach (var scene in (_currentScene as SceneContainer).Find(id)) yield return scene;
            }


            //if (_loadedScenes == null || _loadedScenes.Length == 0) yield break;
            //for(int i = 0; i < _loadedScenes.Length; i++)
            //{
            //    if (_loadedScenes[i].Id == id) yield return _loadedScenes[i];
            //}
        }

        public static IEnumerable<Scene> GetLoadedScenes()
        {
            if (_currentScene == null) yield break;

            yield return _currentScene;
            if (_currentScene is SceneContainer)
            {
                foreach (var scene in (_currentScene as SceneContainer).GetAllChildren()) yield return scene;
            }


            //return _loadedScenes;
        }

        #endregion

        #region Special Types

        private class LoadSceneAsyncOperation : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            #region Fields

            private Scene _scene;
            private IProgressingYieldInstruction _op;

            #endregion

            #region Methods

            public void Start(Scene scene, IProgressingYieldInstruction innerOp)
            {
                _scene = scene;
                _op = innerOp;
                GameLoopEntry.Hook.StartRadicalCoroutine(this.WaitForLoadRoutine());
            }

            private System.Collections.IEnumerator WaitForLoadRoutine()
            {
                yield return _op;
                if (this != Scene._lastLoadCall)
                {
                    //another load async was called
                    this.SetSignal();
                    yield break;
                }

                Scene._lastLoadCall = null;
                Scene.RegisterLoadedScene(_scene);
                this.SetSignal();
            }

            protected override object Tick()
            {
                return null;
            }

            #endregion

            #region IProgressingYieldInstruction Interface

            public float Progress
            {
                get
                {
                    if (this.IsComplete) return 1f;
                    if (_op == null) return 0f;
                    return _op.Progress * 0.99f;
                }
            }

            #endregion

        }

        #endregion

    }

}
