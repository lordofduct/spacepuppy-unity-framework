using UnityEngine;

using com.spacepuppy.Utils;


namespace com.spacepuppy.Scenes
{

    [Singleton.Config(DefaultLifeCycle = SingletonLifeCycleRule.LivesForever, LifeCycleReadOnly = true)]
    public class SceneManager : Singleton
    {

        #region Events

        public event System.EventHandler<SceneLoadingEventArgs> BeforeSceneLoaded;
        public event System.EventHandler<SceneLoadingEventArgs> SceneLoaded;
        public event System.EventHandler<SceneLoadingEventArgs> SceneStarted;

        #endregion

        #region Fields

        [System.NonSerialized()]
        private ISceneBehaviour _currentSceneBehaviour;
        [System.NonSerialized()]
        private ISceneBehaviour _lastSceneBehaviour;

        private WaitForSceneLoaded _loadingOp;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public ISceneBehaviour Current { get { return _currentSceneBehaviour ?? _lastSceneBehaviour; } }

        #endregion

        #region LoadScene Methods

        public IRadicalYieldInstruction Unload()
        {
            IRadicalYieldInstruction arg = null;

            if (_loadingOp != null)
            {
                var op = _loadingOp;
                _loadingOp = null;

                op.Cancel();
                if(_currentSceneBehaviour != null)
                {
                    if(op.LastScene == _currentSceneBehaviour)
                    {
                        _currentSceneBehaviour = null;
                    }
                    else
                    {
                        arg = _currentSceneBehaviour.EndScene();
                        if(arg != null)
                        {
                            _lastSceneBehaviour = _currentSceneBehaviour;
                            arg = this.StartRadicalCoroutine(this.ForceUnloadRoutine(arg, _lastSceneBehaviour));
                        }
                    }
                }
            }
            else if (_currentSceneBehaviour != null)
            {
                _currentSceneBehaviour.EndScene();
                if (_currentSceneBehaviour.gameObject != null) ObjUtil.SmartDestroy(_currentSceneBehaviour.gameObject);
            }

            _currentSceneBehaviour = null;
            return arg;
        }

        public IProgressingYieldInstruction LoadScene(ISceneLoadOptions options)
        {
            if (options == null) throw new System.ArgumentNullException("options");

            if (_loadingOp != null)
            {
                _loadingOp.Cancel();
                if (!_loadingOp.NextSceneStarted) _currentSceneBehaviour = null;
                _loadingOp = null;
            }

            _lastSceneBehaviour = _currentSceneBehaviour;
            _currentSceneBehaviour = null;
            _loadingOp = new WaitForSceneLoaded();
            _loadingOp.Start(this, options, _lastSceneBehaviour);
            return _loadingOp;
        }

        private System.Collections.IEnumerator ForceUnloadRoutine(IRadicalYieldInstruction instruction, ISceneBehaviour waitingOn)
        {
            yield return instruction;
            if (_lastSceneBehaviour == waitingOn) _lastSceneBehaviour = null;
        }


        

        protected virtual void OnBeforeSceneLoaded(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            if (this.BeforeSceneLoaded != null) this.BeforeSceneLoaded(this, e);
        }

        protected virtual void OnSceneLoaded(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            if (this.SceneLoaded != null) this.SceneLoaded(this, e);
        }

        protected virtual void OnSceneStarted(SceneLoadingEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");

            if (this.SceneStarted != null) this.SceneStarted(this, e);
        }

        #endregion

        #region Special Types

        private class WaitForSceneLoaded : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            #region Fields

            private SceneManager _manager;
            private ISceneLoadOptions _options;
            private ISceneBehaviour _lastScene;

            private IProgressingYieldInstruction _loadOp;
            private bool _started;

            private RadicalCoroutine _routine;

            #endregion

            #region Properties

            public SceneManager Manager { get { return _manager; } }

            public ISceneLoadOptions LoadOptions { get { return _options; } }

            public ISceneBehaviour LastScene { get {return _lastScene; } }

            public bool NextSceneStarted { get { return _started; } }

            #endregion

            #region Methods

            public void Start(SceneManager manager, ISceneLoadOptions options, ISceneBehaviour lastScene)
            {
                _manager = manager;
                _options = options;
                _lastScene = lastScene;
                _routine = manager.StartRadicalCoroutine(this.DoLoad()); //GameLoopEntry.Hook.StartRadicalCoroutine(this.DoLoad(), RadicalCoroutineDisableMode.Default);
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
                if(_lastScene != null)
                {
                    //end last scene
                    var endInstruction = _lastScene.EndScene();
                    if (endInstruction != null) yield return endInstruction;
                    if(_manager._lastSceneBehaviour == _lastScene) _manager._lastSceneBehaviour = null;
                }

                var args = new SceneLoadingEventArgs(_manager, _options);
                object[] instructions;

                //signal about to load
                _options.OnBeforeSceneLoaded(_manager, args);
                _manager.OnBeforeSceneLoaded(args);
                if (args.ShouldStall(out instructions)) yield return new WaitForAllComplete(GameLoopEntry.Hook, instructions);

                //do load
                var scene = _options.GetScene(_manager);
                _loadOp = (scene != null) ? scene.LoadAsync() : RadicalYieldInstruction.Null;
                yield return _loadOp;

                //get scene behaviour
                var sceneBehaviour = _options.LoadCustomSceneBehaviour(_manager);
                if(sceneBehaviour == null)
                {
                    var go = new GameObject("SceneBehaviour");
                    go.transform.parent = _manager.transform;
                    go.transform.localPosition = Vector3.zero;
                    sceneBehaviour = go.AddComponent<SceneBehaviour>();
                }
                SceneBehaviour.SceneLoadedInstance = sceneBehaviour;
                _manager._currentSceneBehaviour = sceneBehaviour;

                //signal loaded
                _options.OnSceneLoaded(_manager, args);
                _manager.OnSceneLoaded(args);
                if (args.ShouldStall(out instructions)) yield return new WaitForAllComplete(GameLoopEntry.Hook, instructions);
                else yield return null; //wait one last frame to actually begin the scene

                //signal scene begun
                _started = true;
                var beginInstruction = sceneBehaviour.BeginScene();
                _options.OnSceneStarted(_manager, args);
                _manager.OnSceneStarted(args);
                if (args.ShouldStall(out instructions))
                {
                    var waitAll = new WaitForAllComplete(GameLoopEntry.Hook, instructions);
                    if (beginInstruction != null)
                        waitAll.Add(beginInstruction);
                    beginInstruction = waitAll;
                }
                if (beginInstruction != null) yield return beginInstruction;
                _manager._loadingOp = null;
                this.SetSignal();
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
