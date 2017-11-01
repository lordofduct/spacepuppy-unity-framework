using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Pathfinding.Unity
{

    public class UnityPathFactory : IPathFactory
    {

        #region Static Interface

        private static UnityPathFactory _default;

        public static UnityPathFactory Default
        {
            get
            {
                if (_default == null) _default = new UnityPathFactory();
                return _default;
            }
        }

        #endregion

        #region IPathFactory Interface

        public IPath Create(IPathSeeker seeker, Vector3 target)
        {
            Transform t;
            var root = SPEntity.Pool.GetFromSource(seeker);
            if (root != null)
            {
                t = root.transform;
            }
            else
            {
                t = GameObjectUtil.GetTransformFromSource(seeker);
                if (t == null) throw new PathArgumentException("IPathSeeker has no known Transform to get a position from.");
            }

            return seeker.PathFactory.Create(t.position, target);
        }

        public IPath Create(Vector3 start, Vector3 end)
        {
            return new UnityFromToPath(start, end);
        }

        #endregion

    }

}
