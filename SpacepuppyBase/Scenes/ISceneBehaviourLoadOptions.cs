using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{

    public interface ISceneBehaviourLoadOptions
    {

        System.Type SceneBehaviourType { get; }

        void OnSceneCreated(ISceneBehaviour sceneBehaviour);

        void OnSceneLoaded(ISceneBehaviour sceneBehaviour);

        void OnSceneStarted(ISceneBehaviour sceneBehaviour);

    }

    public class SceneBehaviourLoadOptions : ISceneBehaviourLoadOptions
    {

        #region Fields

        private System.Type _sceneBehaviourType;

        #endregion

        #region CONSTRUCTOR

        public SceneBehaviourLoadOptions(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!ObjUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");
            _sceneBehaviourType = tp;
        }

        #endregion

        #region Methods

        public event System.Action<ISceneBehaviour> SceneCreated;
        public event System.Action<ISceneBehaviour> SceneLoaded;
        public event System.Action<ISceneBehaviour> SceneStarted;

        protected virtual void OnSceneCreated(ISceneBehaviour sceneBehaviour)
        {
            if (this.SceneCreated != null) this.SceneCreated(sceneBehaviour);
        }

        protected virtual void OnSceneLoaded(ISceneBehaviour sceneBehaviour)
        {
            if (this.SceneLoaded != null) this.SceneLoaded(sceneBehaviour);
        }

        protected virtual void OnSceneStarted(ISceneBehaviour sceneBehaviour)
        {
            if (this.SceneStarted != null) this.SceneStarted(sceneBehaviour);
        }

        #endregion

        #region ISceneLoadOptions Interface

        System.Type ISceneBehaviourLoadOptions.SceneBehaviourType
        {
            get { return _sceneBehaviourType; }
        }

        void ISceneBehaviourLoadOptions.OnSceneCreated(ISceneBehaviour sceneBehaviour)
        {
            this.OnSceneCreated(sceneBehaviour);
        }

        void ISceneBehaviourLoadOptions.OnSceneLoaded(ISceneBehaviour sceneBehaviour)
        {
            this.OnSceneLoaded(sceneBehaviour);
        }

        void ISceneBehaviourLoadOptions.OnSceneStarted(ISceneBehaviour sceneBehaviour)
        {
            this.OnSceneStarted(sceneBehaviour);
        }

        #endregion

    }

    public class SceneBehaviourLoadOptions<T> : ISceneBehaviourLoadOptions where T : class, ISceneBehaviour
    {

        #region Methods

        public event System.Action<T> SceneCreated;
        public event System.Action<T> SceneLoaded;
        public event System.Action<T> SceneStarted;

        protected virtual void OnSceneCreated(T sceneBehaviour)
        {
            if (this.SceneCreated != null) this.SceneCreated(sceneBehaviour);
        }

        protected virtual void OnSceneLoaded(T sceneBehaviour)
        {
            if (this.SceneLoaded != null) this.SceneLoaded(sceneBehaviour);
        }

        protected virtual void OnSceneStarted(T sceneBehaviour)
        {
            if (this.SceneStarted != null) this.SceneStarted(sceneBehaviour);
        }

        #endregion

        #region ISceneLoadOptions Interface

        System.Type ISceneBehaviourLoadOptions.SceneBehaviourType
        {
            get { return typeof(T); }
        }

        void ISceneBehaviourLoadOptions.OnSceneCreated(ISceneBehaviour sceneBehaviour)
        {
            if (!(sceneBehaviour is T)) return;
            this.OnSceneCreated(sceneBehaviour as T);
        }

        void ISceneBehaviourLoadOptions.OnSceneLoaded(ISceneBehaviour sceneBehaviour)
        {
            if (!(sceneBehaviour is T)) return;
            this.OnSceneLoaded(sceneBehaviour as T);
        }

        void ISceneBehaviourLoadOptions.OnSceneStarted(ISceneBehaviour sceneBehaviour)
        {
            if (!(sceneBehaviour is T)) return;
            this.OnSceneStarted(sceneBehaviour as T);
        }

        #endregion

    }

}
