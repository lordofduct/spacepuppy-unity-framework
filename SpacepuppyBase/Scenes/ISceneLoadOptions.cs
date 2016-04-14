using UnityEngine;
using com.spacepuppy.Utils;

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
        private System.Type _sceneBehaviourType;

        #endregion

        #region CONSTRUCTOR

        public SceneLoadOptions()
        {

        }

        public SceneLoadOptions(Scene scene)
        {
            _scene = scene;
        }

        public SceneLoadOptions(System.Type sceneBehaviourType)
        {
            this.SceneBehaviourType = sceneBehaviourType;
        }

        public SceneLoadOptions(Scene scene, System.Type sceneBehaviourType)
        {
            _scene = scene;
            this.SceneBehaviourType = sceneBehaviourType;
        }

        #endregion

        #region Properties

        public Scene Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }

        public System.Type SceneBehaviourType
        {
            get { return _sceneBehaviourType; }
            set
            {
                if (!TypeUtil.IsType(value, typeof(ISceneBehaviour)) || !TypeUtil.IsType(value, typeof(Component)))
                    throw new System.ArgumentException("SceneBehaviourType must be a Component and implement ISceneBehaviour.");
                _sceneBehaviourType = value;
            }
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
            if(_sceneBehaviourType != null)
            {
                var result = SceneBehaviour.SceneLoadedInstance;
                if (result != null && TypeUtil.IsType(result.GetType(), _sceneBehaviourType)) return result;
                if (manager == null) return null;

                var go = new GameObject("SceneBehaviour." + _sceneBehaviourType.Name);
                result = go.AddComponent(_sceneBehaviourType) as ISceneBehaviour;
                go.transform.parent = manager.transform;
                go.transform.localPosition = Vector3.zero;
                return result;
            }
            else
            {
                return SceneBehaviour.SceneLoadedInstance;
            }
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
