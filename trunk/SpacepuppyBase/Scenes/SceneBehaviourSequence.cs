using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{

    public class SceneBehaviourSequence : SPComponent, ISceneSequenceBehaviour
    {

        #region Fields

        private bool _isCurrentlyLoaded;

        private Queue<System.Type> _scenes = new Queue<System.Type>();
        private ISceneBehaviour _currentScene;

        #endregion

        #region Properties

        public IEnumerable<System.Type> SceneQueue { get { return _scenes; } }

        public bool SceneQueueIsEmpty { get { return _scenes.Count == 0; } }

        public ISceneBehaviour Current { get { return _currentScene; } }

        #endregion

        #region Methods

        public void Append<T>() where T : class, ISceneBehaviour
        {
            _scenes.Enqueue(typeof(T));
        }

        public void Append(System.Type tp)
        {
            if (tp == null || !TypeUtil.IsType(typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");

            _scenes.Enqueue(tp);
        }

        public void Clear()
        {
            _scenes.Clear();
        }

        #endregion

        #region ISceneSequenceBehaviour Interface

        IProgressingYieldInstruction ISceneBehaviour.LoadScene()
        {
            _isCurrentlyLoaded = true;
            return this.LoadNextScene();
        }

        void ISceneBehaviour.BeginScene()
        {
            _currentScene.BeginScene();
        }

        void ISceneBehaviour.EndScene()
        {
            _currentScene.EndScene();
        }

        public IProgressingYieldInstruction LoadNextScene()
        {
            if (!_isCurrentlyLoaded) throw new System.InvalidOperationException("This SceneBehaviour must first be loaded before it can continue to the next scene.");
            if (_scenes.Count == 0) throw new System.InvalidOperationException("Scene queue is empty.");

            if (_currentScene != null)
            {
                _currentScene.EndScene();
                Object.Destroy(_currentScene.component);
                _currentScene = null;
            }

            _currentScene = this.AddComponent(_scenes.Dequeue()) as ISceneBehaviour;
            var op = new LoadSubSceneOperation();
            op.Start(_currentScene);
            return op;
        }

        #endregion

        #region Special Types

        private class LoadSubSceneOperation : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            private ISceneBehaviour _scene;
            private IProgressingYieldInstruction _innerOp;

            public void Start(ISceneBehaviour scene)
            {
                _scene = scene;

                GameLoopEntry.Hook.StartRadicalCoroutine(this.DoLoad(), RadicalCoroutineDisableMode.Default);
            }

            private System.Collections.IEnumerator DoLoad()
            {
                _innerOp = _scene.LoadScene();
                yield return _innerOp;
                _scene.BeginScene();
            }


            #region IProgressingYieldInstruction Interface

            public float Progress
            {
                get { return (_innerOp != null) ? _innerOp.Progress : 0f; }
            }

            protected override object Tick()
            {
                return null;
            }

            #endregion

        }

        #endregion

    }

}
