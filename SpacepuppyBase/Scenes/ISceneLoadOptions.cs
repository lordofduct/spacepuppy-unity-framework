using UnityEngine;

namespace com.spacepuppy.Scenes
{
    public interface ISceneLoadOptions
    {

        Scene GetScene(SPSceneManager manager);

        ISceneBehaviour LoadCustomSceneBehaviour(SPSceneManager manager);

        void OnBeforeSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e);

        void OnSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e);

        void OnSceneStarted(SPSceneManager manager, SceneLoadingEventArgs e);

    }

    public class SceneLoadOptions : ISceneLoadOptions
    {

        #region Fields

        private Scene _scene;

        #endregion

        #region CONSTRUCTOR

        public SceneLoadOptions()
        {

        }

        public SceneLoadOptions(Scene scene)
        {
            _scene = scene;
        }

        #endregion

        #region Properties

        public Scene Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }

        #endregion

        #region ISceneLoadOptions Interface

        protected virtual Scene GetScene(SPSceneManager manager)
        {
            return _scene;
        }
        Scene ISceneLoadOptions.GetScene(SPSceneManager manager)
        {
            return this.GetScene(manager);
        }

        protected virtual ISceneBehaviour LoadCustomSceneBehaviour(SPSceneManager manager)
        {
            return SceneBehaviour.SceneLoadedInstance;
        }
        ISceneBehaviour ISceneLoadOptions.LoadCustomSceneBehaviour(SPSceneManager manager)
        {
            return this.LoadCustomSceneBehaviour(manager);
        }



        protected virtual void OnBeforeSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e)
        {

        }
        void ISceneLoadOptions.OnBeforeSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnBeforeSceneLoaded(manager, e);
        }

        protected virtual void OnSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e)
        {

        }
        void ISceneLoadOptions.OnSceneLoaded(SPSceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnSceneLoaded(manager, e);
        }

        protected virtual void OnSceneStarted(SPSceneManager manager, SceneLoadingEventArgs e)
        {

        }
        void ISceneLoadOptions.OnSceneStarted(SPSceneManager manager, SceneLoadingEventArgs e)
        {
            this.OnSceneStarted(manager, e);
        }


        #endregion

    }

    public class SceneLoadOptions<T> : SceneLoadOptions where T : Component, ISceneBehaviour
    {

        #region CONSTRUCTOR

        public SceneLoadOptions()
        {

        }

        public SceneLoadOptions(Scene scene)
            : base(scene)
        {

        }

        #endregion


        protected override ISceneBehaviour LoadCustomSceneBehaviour(SPSceneManager manager)
        {
            var result = SceneBehaviour.SceneLoadedInstance as T;
            if (result != null) return result;
            if (manager == null) return null;

            var go = new GameObject("SceneBehaviour." + typeof(T).Name);
            result = go.AddComponent<T>();
            go.transform.parent = manager.transform;
            go.transform.localPosition = Vector3.zero;
            return result;
        }
        
    }

}
