using UnityEngine;

namespace com.spacepuppy.Timers
{
    public interface ITimer
    {

        /// <summary>
        /// Returns true if still running.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        bool Update(float dt);

    }
}
