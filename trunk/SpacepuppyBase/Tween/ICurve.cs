using UnityEngine;

using System;
using System.Collections.Generic;

namespace com.spacepuppy.Tween
{
	public interface ICurve
	{

        float TotalDuration { get; }

        void Update(object targ, float dt, float t);

	}
}
