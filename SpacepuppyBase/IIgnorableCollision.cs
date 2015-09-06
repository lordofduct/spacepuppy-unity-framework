using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy
{

    public interface IIgnorableCollision
    {

        void IgnoreCollision(Collider coll, bool ignore);
        void IgnoreCollision(IIgnorableCollision coll, bool ignore);

    }

    public class IgnorableCollider : IIgnorableCollision, IComponent
    {

        #region Fields

        private Collider _collider;

        #endregion

        #region CONSTRUCTOR

        private IgnorableCollider(Collider coll)
        {
            _collider = coll;
        }

        #endregion

        #region Properties

        public Collider Collider { get { return _collider; } }

        #endregion

        #region IComponent Interface

        GameObject IGameObjectSource.gameObject
        {
            get { return _collider.gameObject; }
        }

        Transform IGameObjectSource.transform
        {
            get { return _collider.transform; }
        }

        public bool enabled
        {
            get
            {
                return _collider.enabled;
            }
            set
            {
                _collider.enabled = value;
            }
        }

        public bool isActiveAndEnabled
        {
            get { return _collider.enabled && _collider.gameObject.activeInHierarchy; }
        }

        public Component component
        {
            get { return _collider; }
        }

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider coll, bool ignore)
        {
            if (_collider == null || coll == null || _collider == coll) return;

            Physics.IgnoreCollision(_collider, coll, ignore);
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (_collider == null || coll == null || this == coll) return;

            coll.IgnoreCollision(_collider, ignore);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            var a = obj as IgnorableCollider;
            if (object.ReferenceEquals(a, null)) return false;
            return this.GetHashCode() == a.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (object.ReferenceEquals(_collider, null)) return 0;
            return _collider.GetInstanceID();
        }

        public static bool operator ==(IgnorableCollider a, IgnorableCollider b)
        {
            if (object.ReferenceEquals(a, null))
                return object.ReferenceEquals(b, null);
            else if (object.ReferenceEquals(b, null)) return false;

            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(IgnorableCollider a, IgnorableCollider b)
        {
            if (object.ReferenceEquals(a, null))
                return !object.ReferenceEquals(b, null);
            else if (object.ReferenceEquals(b, null)) return true;

            return a.GetHashCode() != b.GetHashCode();
        }

        #endregion

        #region Static Interface

        private static Dictionary<Collider, IgnorableCollider> _table = new Dictionary<Collider, IgnorableCollider>(com.spacepuppy.Collections.ObjectInstanceIDEqualityComparer<Collider>.Default);

        private IgnorableCollider()
        {
            GameLoopEntry.LevelWasLoaded += (s, e) => { IgnorableCollider.Clean(); };
            com.spacepuppy.Timers.SystemTimers.CreateGypsyTimer(60f, 0, null, (t) =>
            {
                IgnorableCollider.Clean();
            });
        }

        public static IgnorableCollider GetIgnorableCollider(Collider coll)
        {
            if (coll == null) return null;

            lock (_table)
            {
                IgnorableCollider aspect;
                if (_table.TryGetValue(coll, out aspect))
                {
                    return aspect;
                }
                else
                {
                    aspect = new IgnorableCollider(coll);
                    _table.Add(coll, aspect);
                    return aspect;
                }
            }
        }

        public static void Clean()
        {
            lock (_table)
            {
                using (var toRemove = com.spacepuppy.Collections.TempCollection<Collider>.GetCollection())
                {
                    var e1 = _table.Keys.GetEnumerator();
                    while (e1.MoveNext())
                    {
                        if (e1.Current == null) toRemove.Add(e1.Current);
                    }

                    if (toRemove.Count > 0)
                    {
                        var e2 = toRemove.GetEnumerator();
                        while (e2.MoveNext())
                        {
                            _table.Remove(e2.Current);
                        }
                    }
                }
            }
        }

        #endregion

    }

}
