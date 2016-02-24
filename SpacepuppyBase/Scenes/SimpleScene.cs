using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.spacepuppy.Scenes
{
    public class SimpleScene : Scene
    {

        #region Fields

        private string _sceneAssetId;

        #endregion

        #region CONSTRUCTOR

        public SimpleScene(string sceneAssetId) : base(sceneAssetId)
        {
            _sceneAssetId = sceneAssetId;
        }

        public SimpleScene(string id, string sceneAssetId) : base(id)
        {
            _sceneAssetId = sceneAssetId;
        }

        #endregion

        #region Methods

        #endregion

        #region IScene Interface

        protected internal override void DoLoad()
        {
            //Application.LoadLevel(_sceneAssetId);
            SceneManager.LoadScene(_sceneAssetId, LoadSceneMode.Single);
        }

        protected internal override IProgressingYieldInstruction DoLoadAsync()
        {
            //return new com.spacepuppy.Async.AsyncOperationWrapper(Application.LoadLevelAsync(_sceneAssetId));
            return new com.spacepuppy.Async.AsyncOperationWrapper(SceneManager.LoadSceneAsync(_sceneAssetId, LoadSceneMode.Single));
        }

        protected internal override void DoLoadAdditive()
        {
            //Application.LoadLevelAdditive(_sceneAssetId);
            SceneManager.LoadScene(_sceneAssetId, LoadSceneMode.Additive);
        }

        protected internal override IProgressingYieldInstruction DoLoadAdditiveAsync()
        {
            //return new com.spacepuppy.Async.AsyncOperationWrapper(Application.LoadLevelAdditiveAsync(_sceneAssetId));
            return new com.spacepuppy.Async.AsyncOperationWrapper(SceneManager.LoadSceneAsync(_sceneAssetId, LoadSceneMode.Additive));
        }

        #endregion

    }
}
