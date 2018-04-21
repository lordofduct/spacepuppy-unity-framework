using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents the skeletal rig of an entity. This should be attached to the parent of the root bone of the skeleton. This is the same GameObject 
    /// on which the Animator/Animation should be set as well. Animator only for sake standards, but Animation HAS to be here anyways for the animations 
    /// to even work. Make sure also that the root of the entity is tagged as root, as this will be critical to finding the 'MovementController' when 
    /// ragdolling.
    /// </summary>
    [AddComponentMenu("SpacePuppy/Armature Rig")]
    public class ArmatureRig : SPEntityComponent, IIgnorableCollision
    {

        #region Fields

        public bool LoadJointListOnStart = true;
        public bool SetJointsKinematicOnLoad = true;
        public bool SetCollidersDisabledOnLoad = true;

        [SerializeField()]
        private Rigidbody[] _joints = new Rigidbody[] { };

        private bool _ragdolled;
        private MovementController _cachedController;
        private MovementController.IGameObjectMover _cachedMover;
        private RadicalCoroutine _falseJointCoroutine;
        private bool _mecanimEnabledCache;
        private bool _animatorEnabledCache;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            if (this.LoadJointListOnStart) this.ReloadJointList();
            if (this.SetJointsKinematicOnLoad) this.SetJointsKinematic(true);
            if (this.SetCollidersDisabledOnLoad) this.SetCollidersEnabled(false);
        }

        #endregion

        #region Properties

        public Rigidbody RootJoint { get { return _joints.FirstOrDefault(); } }

        public int JointCount { get {return (_joints != null) ? _joints.Length : 0; } }

        public IEnumerable<Rigidbody> Joints { get { return _joints; } }

        public bool Ragdolled { get { return _ragdolled; } }

        #endregion

        #region Methods

        public Rigidbody GetJoint(int i)
        {
            return _joints[i];
        }

        public void SetJoints(IEnumerable<Rigidbody> joints)
        {
            if (joints == null) _joints = new Rigidbody[] { };
            _joints = (from j in joints where j != null select j).Distinct().ToArray();
        }

        public void ReloadJointList()
        {
            var lst = new List<Rigidbody>();
            foreach (var t in this.GetAllChildrenAndSelf())
            {
                var rb = t.GetComponent<Rigidbody>();
                if (rb != null && t.HasComponent<CharacterJoint>())
                {
                    lst.Add(rb);
                }
            }

            foreach (var go in lst)
            {
                var j = go.GetComponent<CharacterJoint>();
                var other = j.connectedBody;
                if (!lst.Contains(other))
                {
                    lst.Insert(0, other);
                    break;
                }
            }

            _joints = lst.ToArray();
        }

        public void Ragdoll(bool movementControllerAbsent = false)
        {
            if (_ragdolled) return;

            _cachedController = null;
            if(!movementControllerAbsent) _cachedController = this.FindComponent<MovementController>();
            if (_cachedController != null)
            {
                var ragdollMover = new MovementController.RagdollBodyMover();
                _cachedMover = _cachedController.SwapMover(ragdollMover);
            }
            else
            {
                this.SetJointsKinematic(false);
                this.SetCollidersEnabled(true);
                _falseJointCoroutine = this.StartRadicalCoroutine(this.FalseRootJointUpdate(), RadicalCoroutineDisableMode.Pauses);
            }

            var mecanim = this.GetComponent<Animator>();
            if (mecanim != null)
            {
                _mecanimEnabledCache = mecanim.enabled;
                mecanim.enabled = false;
            }

            var anim = this.GetComponent<Animation>();
            if (anim != null)
            {
                _animatorEnabledCache = anim.enabled;
                anim.Stop();
                anim.enabled = false;
            }


            _ragdolled = true;
        }

        public void UndoRagdoll()
        {
            if (!_ragdolled) return;

            if (_cachedController != null)
            {
                _cachedController.ChangeMover(_cachedMover);
            }
            else
            {
                this.SetJointsKinematic(true);
                this.SetCollidersEnabled(false);
                if (_falseJointCoroutine != null) _falseJointCoroutine.Cancel();
            }

            var mecanim = this.GetComponent<Animator>();
            if (mecanim != null) mecanim.enabled = _mecanimEnabledCache;

            var anim = this.GetComponent<Animator>();
            if (anim != null) anim.enabled = _animatorEnabledCache;

            _cachedController = null;
            _cachedMover = null;
            _falseJointCoroutine = null;
            _mecanimEnabledCache = false;
            _animatorEnabledCache = false;

            _ragdolled = false;
        }

        public void SetJointsKinematic(bool bIsKinematic)
        {
            if (_joints != null)
            {
                foreach (var j in _joints)
                {
                    j.isKinematic = bIsKinematic;
                }
            }
        }

        public void SetCollidersEnabled(bool bEnabled)
        {
            if (_joints != null)
            {
                foreach (var j in _joints)
                {
                    var coll = j.GetComponent<Collider>();
                    if (coll != null) coll.enabled = bEnabled;
                }
            }
        }

        public void SetJointsLayer(int layer)
        {
            if (_joints != null)
            {
                foreach (var j in _joints)
                {
                    j.gameObject.layer = layer;
                }
            }
        }



        public float CalculateMass()
        {
            if (_joints == null || _joints.Length == 0) return 0;
            //return (from j in _joints where j != null select j.mass).Sum();

            float m = 0f;
            foreach(var j in _joints)
            {
                m += j.mass;
            }
            return m;
        }

        public com.spacepuppy.Geom.Sphere CalculateBoundingSphere(bool bIncludeTriggers = false)
        {
            if (this.RootJoint == null) return new com.spacepuppy.Geom.Sphere(this.transform.position, 0f);

            var sphere = new com.spacepuppy.Geom.Sphere(this.RootJoint.worldCenterOfMass, 0f);
            if (bIncludeTriggers)
            {
                foreach (var coll in this.RootJoint.GetChildComponents<Collider>())
                {
                    sphere.Encapsulate(com.spacepuppy.Geom.Sphere.FromCollider(coll));
                }
            }
            else
            {
                foreach (var coll in from c in this.RootJoint.GetChildComponents<Collider>() where !c.isTrigger select c)
                {
                    sphere.Encapsulate(com.spacepuppy.Geom.Sphere.FromCollider(coll));
                }
            }

            return sphere;
        }

        #endregion

        #region Routines

        private System.Collections.IEnumerable FalseRootJointUpdate()
        {
            while(true)
            {
                yield return null;
                if (!_ragdolled) yield break;

                if (this.RootJoint != null && this.RootJoint.transform != this.entityRoot.transform)
                {
                    var pos = this.RootJoint.transform.position;
                    this.entityRoot.transform.position = pos;
                    this.RootJoint.transform.position = pos;
                }
            }
        }

        #endregion

        #region IIgnorableCollision Interface

        public void IgnoreCollision(Collider collider, bool ignore)
        {
            if (_joints == null || _joints.Length == 0) return;
            if (collider == null || !collider.enabled) return;

            Collider jc;
            foreach (var j in _joints)
            {
                jc = j.GetComponent<Collider>();
                if (jc.IsActiveAndEnabled()) Physics.IgnoreCollision(collider, jc, ignore);
            }
        }

        public void IgnoreCollision(IIgnorableCollision coll, bool ignore)
        {
            if (_joints == null || _joints.Length == 0) return;
            if (coll == null) return;

            Collider jc;
            foreach (var j in _joints)
            {
                jc = j.GetComponent<Collider>();
                if (jc.IsActiveAndEnabled()) coll.IgnoreCollision(jc, ignore);
            }
        }

        #endregion

    }

}