using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{
    
    public static class SceneManagerUtils
    {

        #region Static Utils

        public static LoadSceneWaitHandle LoadScene(string sceneName, LoadSceneMode mode, LoadSceneBehaviour behaviour)
        {
            var manager = Services.Get<ISceneManager>();
            if (manager != null) return manager.LoadScene(sceneName, mode, behaviour);

            switch (behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        SceneManager.LoadScene(sceneName, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        var op = SceneManager.LoadSceneAsync(sceneName, mode);
                        op.allowSceneActivation = false;
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
            }

            throw new System.InvalidOperationException("Unsupported LoadSceneBehaviour.");
        }

        public static LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode, LoadSceneBehaviour behaviour)
        {
            if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) throw new System.IndexOutOfRangeException("sceneBuildIndex");

            var manager = Services.Get<ISceneManager>();
            if (manager != null) return manager.LoadScene(sceneBuildIndex, mode, behaviour);

            //string sceneName = "#" + sceneBuildIndex.ToString();
            string sceneName = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);

            switch (behaviour)
            {
                case LoadSceneBehaviour.Standard:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        SceneManager.LoadScene(sceneBuildIndex, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), null);
                        return handle;
                    }
                case LoadSceneBehaviour.Async:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
                case LoadSceneBehaviour.AsyncAndWait:
                    {
                        var handle = new LoadSceneWaitHandle(sceneName, mode, behaviour);
                        var op = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
                        op.allowSceneActivation = false;
                        handle.Init(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), op);
                        return handle;
                    }
            }

            throw new System.InvalidOperationException("Unsupported LoadSceneBehaviour.");
        }

        #endregion
        
    }

}
