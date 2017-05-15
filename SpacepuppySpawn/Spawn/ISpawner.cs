using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    /// <summary>
    /// Implement by a spawner that can have modifiers attached to it.
    /// 
    /// Make sure to implement spawning via the SelfTrackingSpawnerMechanism in the spawner.
    /// </summary>
    public interface ISpawner : IComponent, INotificationDispatcher
    {

        /// <summary>
        /// The TrackingMechanism used for spawning entities.
        /// </summary>
        SelfTrackingSpawnerMechanism Mechanism { get; }

        /// <summary>
        /// Total number of entities, dead or alive, that have spawned since this Spawner started.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Number of entities currently spawned and active.
        /// </summary>
        int ActiveCount { get; }
        
        /// <summary>
        /// Spawn whatever this spawner spawns.
        /// </summary>
        /// <returns></returns>
        void Spawn();

        

    }

}
