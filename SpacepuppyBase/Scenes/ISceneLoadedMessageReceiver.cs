using System;

namespace com.spacepuppy.Scenes
{

    public interface ISceneLoadedMessageReceiver
    {

        void OnSceneLoaded(LoadSceneWaitHandle handle);

    }

}
