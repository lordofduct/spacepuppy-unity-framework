
namespace com.spacepuppy
{

    /// <summary>
    /// Receive a signal that the yielding RadicalCoroutine was paused, so that the instruction can deal with that state accordingly 
    /// if it must do something special. For example WaitForDuration wants to store the time so that when the coroutine resumes 
    /// it starts counting from the appropriate time instead of appearing finished.
    /// </summary>
    public interface IPausibleYieldInstruction : IRadicalYieldInstruction
    {

        void OnPause();
        void OnResume();

    }

}
