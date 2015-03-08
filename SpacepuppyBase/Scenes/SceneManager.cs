using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;


namespace com.spacepuppy.Scenes
{
    public class SceneManager : Singleton
    {

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
            if (tp == null || !ObjUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");
            return this.LoadScene(new SceneBehaviourLoadOptions(tp));
        }

        public IProgressingYieldInstruction LoadScene(ISceneBehaviourLoadOptions options)
        {
            if (options == null) throw new System.ArgumentNullException("options");

            var tp = options.SceneBehaviourType;
            if (tp == null || !ObjUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");

            this.Unload();

            _currentSceneBehaviourGameObject = new GameObject(tp.Name);
            _currentSceneBehaviourGameObject.transform.parent = this.transform;
            _currentSceneBehaviourGameObject.transform.position = Vector3.zero;

            _currentSceneBehaviour = _currentSceneBehaviourGameObject.AddComponent(tp) as ISceneBehaviour;

            _loadingOp = new WaitForSceneLoaded();
            _loadingOp.Start(_currentSceneBehaviour, options);
            return _loadingOp;
        }

        #endregion

        #region Special Types

        private class WaitForSceneLoaded : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            #region Fields

            private ISceneBehaviour _scene;
            private ISceneBehaviourLoadOptions _options;
            private IProgressingYieldInstruction _loadOp;

            private RadicalCoroutine _routine;

            #endregion

            #region Methods

            public void Start(ISceneBehaviour scene, ISceneBehaviourLoadOptions options)
            {
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
                _options.OnSceneCreated(_scene);
                _loadOp = _scene.LoadScene();
                yield return _loadOp;
                _options.OnSceneLoaded(_scene);
                _scene.BeginScene();
                this.SetSignal();
                _options.OnSceneStarted(_scene);
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
