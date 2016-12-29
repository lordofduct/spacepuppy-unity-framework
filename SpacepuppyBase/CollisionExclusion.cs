using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// An object identity for 2 Colliders or IIgnorableCollision objects ignoring one another. 
    /// This also allows you to not have to track ignore relationships. If 2 objects are flagged to ignore in 2 different segments 
    /// of code, this will track them stacking, and won't unignore until BOTH places call to stop the ignoring.
    /// </summary>
    public class CollisionExclusion : System.IDisposable
    {

        #region Fields

        private IIgnorableCollision _collA;
        private IIgnorableCollision _collB;
        private bool _active;

        #endregion

        #region CONSTRUCTOR

        public CollisionExclusion(Collider a, Collider b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            _collA = IgnorableCollider.GetIgnorableCollision(a);
            _collB = IgnorableCollider.GetIgnorableCollision(b);
        }

        public CollisionExclusion(IIgnorableCollision a, IIgnorableCollision b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            _collA = a;
            _collB = b;
        }

        public CollisionExclusion(Collider a, IIgnorableCollision b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            _collA = IgnorableCollider.GetIgnorableCollision(a);
            _collB = b;
        }

        public CollisionExclusion(IIgnorableCollision a, Collider b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            _collA = a;
            _collB = IgnorableCollider.GetIgnorableCollision(b);
        }

        ~CollisionExclusion()
        {
            if (GameLoopEntry.ApplicationClosing) return;

            if (_active) PurgeWhenCan(_collA, _collB);
        }

        #endregion

        #region Properties

        public IIgnorableCollision ColliderA { get { return _collA; } }

        public IIgnorableCollision ColliderB { get { return _collB; } }

        public bool Active { get { return _active; } }

        #endregion

        #region Methods

        public void BeginExclusion()
        {
            if (_active) return;
            if (_collA == null || _collB == null) throw new System.InvalidOperationException("One or more referenced collider is null or destroyed.");

            var token = new PairToken(_collA, _collB);
            int cnt;
            if (_table.TryGetValue(token, out cnt))
            {
                _table[token] = cnt + 1;
            }
            else
            {
                _collA.IgnoreCollision(_collB, true);
                _table[token] = 1;
            }
            _active = true;
        }

        public void EndExclusion()
        {
            var token = new PairToken(_collA, _collB);
            if (token.IsDead)
            {
                _table.Remove(token);
                return;
            }
            if (!_active) return;

            int cnt;
            if (_table.TryGetValue(token, out cnt))
            {
                cnt--;
                if (cnt <= 0)
                {
                    _collA.IgnoreCollision(_collB, false);
                    _table.Remove(token);
                }
                else
                {
                    _table[token] = cnt;
                }
            }
            _active = false;
        }

        public void ForceEndExclusion()
        {
            var token = new PairToken(_collA, _collB);
            if (token.IsDead)
            {
                _table.Remove(token);
                return;
            }

            if(_table.Remove(token))
            {
                _collA.IgnoreCollision(_collB, false);
            }
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.EndExclusion();
        }

        #endregion

        #region Static Interface

        private static Dictionary<PairToken, int> _table = new Dictionary<PairToken, int>(new PairTokenComparer());

        static CollisionExclusion()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnGlobalLevelWasLoaded;
        }

        public static void CleanLinks()
        {
            using (var toRemove = com.spacepuppy.Collections.TempCollection.GetList<PairToken>())
            {
                var e1 = _table.Keys.GetEnumerator();
                while (e1.MoveNext())
                {
                    if (e1.Current.IsDead) toRemove.Add(e1.Current);
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
        
        private static void OnGlobalLevelWasLoaded(UnityEngine.SceneManagement.Scene sc, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            CleanLinks();
        }

        private static void PurgeWhenCan(IIgnorableCollision a, IIgnorableCollision b)
        {
            GameLoopEntry.UpdatePump.BeginInvoke(() =>
                {
                    var token = new PairToken(a, b);
                    if (token.IsDead)
                    {
                        _table.Remove(token);
                        return;
                    }

                    int cnt;
                    if (_table.TryGetValue(token, out cnt))
                    {
                        cnt--;
                        if (cnt <= 0)
                        {
                            a.IgnoreCollision(b, false);
                            _table.Remove(token);
                        }
                        else
                        {
                            _table[token] = cnt;
                        }
                    }
                });
        }

        #endregion

        #region Special Types

        private struct PairToken
        {
            public IIgnorableCollision CollA;
            public IIgnorableCollision CollB;

            public PairToken(IIgnorableCollision a, IIgnorableCollision b)
            {
                CollA = a;
                CollB = b;
            }

            public bool IsDead
            {
                get
                {
                    return this.CollA.IsNullOrDestroyed() || this.CollB.IsNullOrDestroyed();
                }
            }

        }

        private class PairTokenComparer : IEqualityComparer<PairToken>
        {

            public bool Equals(PairToken x, PairToken y)
            {
                //NOTE - this scenario should never happen as long as properly used... so lets not waste the time even testing
                //if (object.ReferenceEquals(x.CollA, null) || object.ReferenceEquals(y.CollA, null) || object.ReferenceEquals(x.CollB, null) || object.ReferenceEquals(y.CollB, null)) return false;

                if (x.CollA.Equals(y.CollB))
                    return x.CollB.Equals(y.CollA);
                else if (x.CollA.Equals(y.CollA))
                    return x.CollA.Equals(y.CollB);
                else
                    return false;
            }

            public int GetHashCode(PairToken obj)
            {
                //NOTE - this scenario should never happen as long as properly used... so lets not waste the time even testing
                //if (object.ReferenceEquals(obj.CollA, null)) return 0;
                //if (object.ReferenceEquals(obj.CollB, null)) return 0;

                return obj.CollA.GetHashCode() ^ obj.CollB.GetHashCode();
            }
        }

        #endregion

    }
    
    public class ColliderExclusion : System.IDisposable
    {

        #region Fields

        private Collider _collA;
        private Collider _collB;
        private bool _active;

        #endregion

        #region CONSTRUCTOR

        public ColliderExclusion(Collider a, Collider b)
        {
            _collA = a;
            _collB = b;
        }

        ~ColliderExclusion()
        {
            if (GameLoopEntry.ApplicationClosing) return;

            if (_active) PurgeWhenCan(_collA, _collB);
        }

        #endregion

        #region Properties

        public Collider ColliderA { get { return _collA; } }

        public Collider ColliderB { get { return _collB; } }

        public bool Active { get { return _active; } }

        #endregion

        #region Methods

        public void BeginExclusion()
        {
            if (_active) return;
            if (_collA == null || _collB == null) throw new System.InvalidOperationException("One or more referenced collider is null or destroyed.");

            var token = new PairToken(_collA, _collB);
            int cnt;
            if (_table.TryGetValue(token, out cnt))
            {
                _table[token] = cnt + 1;
            }
            else
            {
                Physics.IgnoreCollision(_collA, _collB, true);
                _table[token] = 1;
            }
            _active = true;
        }

        public void EndExclusion()
        {
            var token = new PairToken(_collA, _collB);
            if (token.IsDead)
            {
                _table.Remove(token);
                return;
            }
            if (!_active) return;

            int cnt;
            if (_table.TryGetValue(token, out cnt))
            {
                cnt--;
                if (cnt <= 0)
                {
                    Physics.IgnoreCollision(_collA, _collB, false);
                    _table.Remove(token);
                }
                else
                {
                    _table[token] = cnt;
                }
            }
            _active = false;
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.EndExclusion();
        }

        #endregion

        #region Static Interface

        private static Dictionary<PairToken, int> _table = new Dictionary<PairToken, int>(new PairTokenComparer());

        static ColliderExclusion()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnGlobalLevelWasLoaded;
        }

        public static void CleanLinks()
        {
            using (var toRemove = com.spacepuppy.Collections.TempCollection.GetList<PairToken>())
            {
                var e1 = _table.Keys.GetEnumerator();
                while (e1.MoveNext())
                {
                    if (e1.Current.IsDead) toRemove.Add(e1.Current);
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
        
        private static void OnGlobalLevelWasLoaded(UnityEngine.SceneManagement.Scene sc, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            CleanLinks();
        }

        private static void PurgeWhenCan(Collider a, Collider b)
        {
            GameLoopEntry.UpdatePump.BeginInvoke(() =>
            {
                var token = new PairToken(a, b);
                if (token.IsDead)
                {
                    _table.Remove(token);
                    return;
                }

                int cnt;
                if (_table.TryGetValue(token, out cnt))
                {
                    cnt--;
                    if (cnt <= 0)
                    {
                        Physics.IgnoreCollision(token.CollA, token.CollB, false);
                        _table.Remove(token);
                    }
                    else
                    {
                        _table[token] = cnt;
                    }
                }
            });
        }

        #endregion

        #region Special Types

        private struct PairToken
        {
            public Collider CollA;
            public Collider CollB;

            public PairToken(Collider a, Collider b)
            {
                CollA = a;
                CollB = b;
            }

            public bool IsDead
            {
                get
                {
                    return this.CollA == null || this.CollB == null;
                }
            }

        }

        private class PairTokenComparer : IEqualityComparer<PairToken>
        {

            public bool Equals(PairToken x, PairToken y)
            {
                //NOTE - this scenario should never happen as long as properly used... so lets not waste the time even testing
                //if (object.ReferenceEquals(x.CollA, null) || object.ReferenceEquals(y.CollA, null) || object.ReferenceEquals(x.CollB, null) || object.ReferenceEquals(y.CollB, null)) return false;

                int a = x.CollA.GetInstanceID();
                int b = x.CollB.GetInstanceID();
                int c = y.CollA.GetInstanceID();
                int d = y.CollA.GetInstanceID();

                if (a == c)
                    return b == d;
                else if (a == d)
                    return b == c;
                else
                    return false;
            }

            public int GetHashCode(PairToken obj)
            {
                //NOTE - this scenario should never happen as long as properly used... so lets not waste the time even testing
                //if (object.ReferenceEquals(obj.CollA, null)) return 0;
                //if (obj.CollB == null) return 0;

                return obj.CollA.GetInstanceID() ^ obj.CollB.GetInstanceID();
            }
        }

        #endregion

    }

}
