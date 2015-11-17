

namespace com.spacepuppy.Scenes
{
    public interface ISceneBehaviour : IComponent
    {

        IRadicalYieldInstruction BeginScene();
        IRadicalYieldInstruction EndScene();
    }

    public interface ISceneSequenceBehaviour : ISceneBehaviour
    {
        IProgressingYieldInstruction LoadNextScene();
    }

    public class SceneBehaviour : SPNotifyingComponent, ISceneBehaviour
    {

        #region Static Interface

        private static ISceneBehaviour _activeInstance;
        /// <summary>
        /// This is used by the SceneManager to check if a SceneBehaviour was loaded during 'Load' so 
        /// it can get a ref of it. Only ever set this during Awake of a ISceneBehaviour that doesn't 
        /// inherit from SceneBehaviour.
        /// </summary>
        public static ISceneBehaviour SceneLoadedInstance
        {
            get { return _activeInstance; }
            set { _activeInstance = value; }
        }

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _activeInstance = this;
        }

        protected override void OnDestroy()
        {
            if (object.ReferenceEquals(_activeInstance, this)) _activeInstance = null;

            base.OnDestroy();
        }

        #endregion


        public virtual IRadicalYieldInstruction BeginScene()
        {
            return null;
        }

        public virtual IRadicalYieldInstruction EndScene()
        {
            return null;
        }
        
    }

}
