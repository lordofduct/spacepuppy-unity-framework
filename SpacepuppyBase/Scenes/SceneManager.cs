using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;


namespace com.spacepuppy.Scenes
{
    public class SceneManager : Singleton
    {

        #region Events

        public event System.EventHandler<SceneLoadingEventArgs> SceneCreated;
        public event System.EventHandler<SceneLoadingEventArgs> SceneLoaded;
        public event System.EventHandler<SceneLoadingEventArgs> SceneStarted;

        #endregion

        #region Fields

        [System.NonSerialized()]
        private GameObject _currentSceneBehaviourGameObject;
        [System.NonSerialized()]
        private ISceneBehaviour _currentSceneBehaviour;

        private WaitForSceneLoaded _loadingOp;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public ISceneBehaviour Current { get { return _currentSceneBehaviour; } }

        #endregion

        #region LoadScene Methods

        public void Unload()
        {
            if (_loadingOp != null)
            {
                _loadingOp.Cancel();
                _loadingOp = null;
            }

            if (_currentSceneBehaviourGameObject != null)
            {
                if(_currentSceneBehaviour != null) _currentSceneBehaviour.EndScene();
                GameObject.Destroy(_currentSceneBehaviourGameObject);
                _currentSceneBehaviourGameObject = null;
                _currentSceneBehaviour = null;
            }
        }

        public IProgressingYieldInstruction LoadScene<T>() where T : class, ISceneBehaviour
        {
            return this.LoadScene(new SceneBehaviourLoadOptions<T>());
        }

        public IProgressingYieldInstruction LoadScene(System.Type tp)
        {
            if (tp == null || !TypeUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");
            return this.LoadScene(new SceneBehaviourLoadOptions(tp));
        }

        public IProgressingYieldInstruction LoadScene(ISceneBehaviourLoadOptions options)
        {
            if (options == null) throw new System.ArgumentNullException("options");

            var tp = options.SceneBehaviourType;
            if (tp == null || !TypeUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");

            this.Unload();

            _currentSceneBehaviourGameObject = new GameObject(tp.Name);
            _currentSceneBehaviourGameObject.transform.parent = this.transform;
            _currentSceneBehaviourGameObject.transform.position = Vector3.zero;

            _currentSceneBehaviour = _currentSceneBehaviourGameObject.AddComponent(tp) as ISceneBehaviour;

            _loadingOp = new WaitForSceneLoaded();
            _loadingOp.Start(this, _currentSceneBehaviour, options);
            return _loadingOp;
        }



        protected virtual void OnSceneCreated(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            e.LoadOptions.OnSceneCreated(this, e.Scene);
            if (this.SceneCreated != null) this.SceneCreated(this, e);
        }

        protected virtual void OnSceneLoaded(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            e.LoadOptions.OnSceneLoaded(this, e.Scene);
            if (this.SceneLoaded != null) this.SceneLoaded(this, e);
        }

        protected virtual void OnSceneStarted(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            e.LoadOptions.OnSceneStarted(this, e.Scene);
            if (this.SceneStarted != null) this.SceneStarted(this, e);
        }

        #endregion

        #region Special Types

        private class WaitForSceneLoaded : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            #region Fields

            private SceneManager _manager;
            private ISceneBehaviour _scene;
            private ISceneBehaviourLoadOptions _options;
            private IProgressingYieldInstruction _loadOp;

            private RadicalCoroutine _routine;

            #endregion

            #region Methods

            public void Start(SceneManager manager, ISceneBehaviour scene, ISceneBehaviourLoadOptions options)
            {
                _manager = manager;
                _scene = scene;
                _options = options;
                _routine = GameLoopEntry.Hook.StartRadicalCoroutine(this.DoLoad());
            }

            public void Cancel()
            {
                if (_routine != null)
                {
                    _routine.Cancel();
                    _routine = null;
                }

                this.SetSignal();
            }

            private System.Collections.IEnumerator DoLoad()
            {
                var args = new SceneLoadingEventArgs(_manager, _scene, _options);

                _manager.OnSceneCreated(args);
                _loadOp = _scene.LoadScene();
                yield return _loadOp;
                _manager.OnSceneLoaded(args);
                _scene.BeginScene();
                this.SetSignal();
                _manager.OnSceneStarted(args);
            }

            protected override object Tick()
            {
                return null;
            }

            #endregion

            #region IProgressAsyncOperation Interface

            public float Progress
            {
                get
                {
                    if (this.IsComplete)
                        return 1f;
                    else if (_loadOp == null)
                        return 0f;
                    else
                        return _loadOp.Progress * 0.99f;
                }
            }

            #endregion

        }

        #endregion

    }
}
