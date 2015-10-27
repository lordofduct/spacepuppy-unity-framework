
using com.spacepuppy.Utils;

namespace com.spacepuppy.Scenes
{

    /*

    public interface ISceneBehaviourLoadOptions
    {

        System.Type SceneBehaviourType { get; }

        void OnBeforeSceneLoaded(SceneManager manager, SceneLoadingEventArgs e);

        void OnSceneLoaded(SceneManager manager, SceneLoadingEventArgs e);

        void OnSceneStarted(SceneManager manager, SceneLoadingEventArgs e);

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
            if (!TypeUtil.IsType(tp, typeof(ISceneBehaviour))) throw new TypeArgumentMismatchException(tp, typeof(ISceneBehaviour), "tp");
            _sceneBehaviourType = tp;
        }

        #endregion

        #region Properties

        public System.Type SceneBehaviourType { get { return _sceneBehaviourType; } }

        #endregion

        #region Methods

        public event System.Action<SceneLoadingEventArgs> BeforeSceneLoaded;
        public event System.Action<SceneLoadingEventArgs> SceneLoaded;
        public event System.Action<SceneLoadingEventArgs> SceneStarted;

        protected virtual void OnBeforeSceneLoaded(SceneManager manager, SceneLoadingEventArgs e)
        {
            if (this.BeforeSceneLoaded != null) this.BeforeSceneLoaded(e);
        }

        protected virtual void OnSceneLoaded(SceneManager manager, SceneLoadingEventArgs e)
        {
            if (this.SceneLoaded != null) this.SceneLoaded(e);
        }

        protected virtual void OnSceneStarted(SceneManager manager, SceneLoadingEventArgs e)
        {
            if (this.SceneStarted != null) this.SceneStarted(e);
        }

        #endregion

        #region ISceneLoadOptions Interface

        System.Type ISceneBehaviourLoadOptions.SceneBehaviourType
        {
            get { return _sceneBehaviourType; }
        }

        void ISceneBehaviourLoadOptions.OnBeforeSceneLoaded(SceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnBeforeSceneLoaded(manager, e);
        }

        void ISceneBehaviourLoadOptions.OnSceneLoaded(SceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnSceneLoaded(manager, e);
        }

        void ISceneBehaviourLoadOptions.OnSceneStarted(SceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnSceneStarted(manager, e);
        }

        #endregion

    }

    */

}
