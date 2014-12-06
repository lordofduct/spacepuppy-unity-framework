using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public interface IKillableObject : IComponent
    {

        bool IsDead { get; }

        void Kill();

    }
}
