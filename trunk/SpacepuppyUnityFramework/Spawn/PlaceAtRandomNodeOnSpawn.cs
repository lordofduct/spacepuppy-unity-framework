using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{
    public class PlaceAtRandomNodeOnSpawn : OnSpawnModifier
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Transform[] GetAllNodes()
        {
            return (from g in this.GetAllChildren() where StringUtil.StartsWith(g.name, "node", true) select g).ToArray();
        }

        #endregion

        #region Notification Handlers

        protected internal override void OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
            var nodes = this.GetAllNodes();
            var i = Random.Range(0, nodes.Length);
            var node = nodes[i];

            n.SpawnedObject.transform.position = node.position;
            n.SpawnedObject.transform.rotation = node.rotation;
        }

        #endregion

    }
}
