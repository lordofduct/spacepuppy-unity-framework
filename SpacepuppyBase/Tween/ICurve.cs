using UnityEngine;

using System;
using System.Collections.Generic;

namespace com.spacepuppy.Tween
{
	public interface ICurve
	{

        /// <summary>
        /// The duration of this curve from beginning to end, including any delays.
        /// </summary>
        float TotalDuration { get; }

        /// <summary>
        /// Updates the targ in an appropriate manner, if the targ is of a type that can be updated by this curve.
        /// </summary>
        /// <param name="targ">The target to be updated.</param>
        /// <param name="dt">The change in time since last update.</param>
        /// <param name="t">A value from 0 to TotalDuration representing the position the curve aught to be at.</param>
        void Update(object targ, float dt, float t);

	}
}
