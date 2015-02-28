using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            Application.LoadLevel(_sceneAssetId);
        }

        protected internal override IProgressingYieldInstruction DoLoadAsync()
        {
            return new com.spacepuppy.Async.AsyncOperationWrapper(Application.LoadLevelAsync(_sceneAssetId));
        }

        protected internal override void DoLoadAdditive()
        {
            Application.LoadLevelAdditive(_sceneAssetId);
        }

        protected internal override IProgressingYieldInstruction DoLoadAdditiveAsync()
        {
            return new com.spacepuppy.Async.AsyncOperationWrapper(Application.LoadLevelAdditiveAsync(_sceneAssetId));
        }

        #endregion

    }
}
