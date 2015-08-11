using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{

    /// <summary>
    /// Handles loading a sequence of SceneBehaviours one after the other.
    /// </summary>
    public class SceneBehaviourSequence : ISceneSequenceBehaviour
    {

        #region Fields

        private ISceneSequenceBehaviour _owner;
        private bool _storeSceneBehavioursOnChildGameObject;

        private bool _isCurrentlyLoaded;

        private Queue<System.Type> _scenes = new Queue<System.Type>();

        private int _currentSceneCount;
        private ISceneBehaviour _currentScene;

        #endregion

        #region CONSTRUCTOR

        public SceneBehaviourSequence(ISceneSequenceBehaviour owner, bool storeSceneBehaviourOnChildGameObject)
        {
            if (owner == null) throw new System.ArgumentNullException("owner");
            _owner = owner;
            _storeSceneBehavioursOnChildGameObject = storeSceneBehaviourOnChildGameObject;
        }

        #endregion

        #region Properties

        public ISceneSequenceBehaviour Owner { get { return _owner; } }

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

        public IProgressingYieldInstruction LoadScene()
        {
            _isCurrentlyLoaded = true;
            return this.LoadNextScene();
        }

        public IProgressingYieldInstruction LoadNextScene()
        {
            if (!_isCurrentlyLoaded) throw new System.InvalidOperationException("This SceneBehaviour must first be loaded before it can continue to the next scene.");
            if (_scenes.Count == 0) throw new System.InvalidOperationException("Scene queue is empty.");


            if (_currentScene != null)
            {
                _currentScene.EndScene();
                if (_storeSceneBehavioursOnChildGameObject)
                    Object.Destroy(_currentScene.gameObject);
                else
                    Object.Destroy(_currentScene.component);
                _currentScene = null;
            }

            _currentSceneCount++;
            if (_storeSceneBehavioursOnChildGameObject)
            {
                var go = new GameObject("Scene_" + _currentSceneCount.ToString("00"));
                this.transform.AddChild(go);
                _currentScene = go.AddComponent(_scenes.Dequeue()) as ISceneBehaviour;
            }
            else
            {
                _currentScene = this.gameObject.AddComponent(_scenes.Dequeue()) as ISceneBehaviour;
            }

            var op = new LoadSubSceneOperation();
            op.Start(_currentScene);
            return op;
        }

        public void BeginScene()
        {
            if (_currentScene == null) return;
            _currentScene.BeginScene();
        }

        public void EndScene()
        {
            if (_currentScene == null) return;
            _currentScene.EndScene();
        }

        #endregion

        #region IComponent Interface

        public bool enabled
        {
            get
            {
                return _owner.enabled;
            }
            set
            {
                _owner.enabled = value;
            }
        }

        public bool isActiveAndEnabled
        {
            get { return _owner.isActiveAndEnabled; }
        }

        public Component component
        {
            get { return _owner.component; }
        }

        public GameObject gameObject
        {
            get { return _owner.gameObject; }
        }

        public Transform transform
        {
            get { return _owner.transform; }
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
                yield return null;
                _scene.BeginScene();
                this.SetSignal();
            }


            #region IProgressingYieldInstruction Interface

            public float Progress
            {
                get { return (_innerOp != null) ? _innerOp.Progress : 0f; }
            }

            #endregion

        }

        #endregion

    }
}
