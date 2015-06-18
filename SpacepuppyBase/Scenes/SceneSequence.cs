using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{
    public class SceneSequence : IEnumerable<Scene>
    {

        #region Fields

        private Queue<Scene> _scenes = new Queue<Scene>();
        private Scene _currentScene;

        #endregion

        #region Properties

        public int Count { get { return _scenes.Count; } }

        public Scene CurrentScene { get { return _currentScene; } }

        #endregion

        #region Methods

        public void Append(Scene scene)
        {
            if (scene == null) throw new System.ArgumentNullException("scene");
            _scenes.Enqueue(scene);
        }

        public void Clear()
        {
            _scenes.Clear();
        }

        public IProgressingYieldInstruction LoadNextScene(System.Action callback = null)
        {
            if (_scenes.Count > 0)
            {
                var op = new LoadSceneOperation();
                op.Start(this, _scenes.Dequeue(), callback);
                return op;
            }
            else
            {
                throw new System.InvalidOperationException("Scene queue is empty.");
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<Scene> GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        #endregion


        #region Special Types

        private class LoadSceneOperation : RadicalYieldInstruction, IProgressingYieldInstruction
        {

            private RadicalCoroutine _routine;


            public void Start(SceneSequence owner, Scene scene, System.Action callback)
            {
                _routine = GameLoopEntry.Hook.StartRadicalCoroutine(this.LoadNextSceneRoutine(owner, scene, callback));
            }

            private System.Collections.IEnumerator LoadNextSceneRoutine(SceneSequence owner, Scene scene, System.Action callback)
            {
                owner._currentScene = scene;
                yield return scene.LoadAsync();
                yield return null;
                if (callback != null) callback();
                this.SetSignal();
            }

            protected override bool Tick(out object yieldObject)
            {
                if(_routine == null || _routine.Finished)
                {
                    yieldObject = null;
                    return false;
                }
                else
                {
                    yieldObject = _routine;
                    return true;
                }
            }

            public float Progress
            {
                get { return (this.IsComplete) ? 0f : 1f; }
            }
        }

        #endregion



    }
}
