using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    [AddComponentMenu("SpacePuppy/Armature Rig")]
    public class ArmatureRig : SPComponent
    {

        #region Fields

        public bool LoadJointListOnStart = true;
        public bool SetJointsKinematicOnLoad = true;
        public bool SetCollidersDisabledOnLoad = true;

        public Rigidbody[] Joints;

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

        public Rigidbody RootJoint { get { return (Joints != null && Joints.Length > 0) ? Joints[0] : null; } }

        #endregion

        #region Methods

        public void ReloadJointList()
        {
            var lst = new List<Rigidbody>();
            foreach (var t in this.GetAllChildrenAndSelf())
            {
                if (t.rigidbody != null && t.HasComponent<CharacterJoint>())
                {
                    lst.Add(t.rigidbody);
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
            this.Joints = lst.ToArray();
        }

        public void SetJointsKinematic(bool bIsKinematic)
        {
            if (this.Joints != null)
            {
                foreach (var j in this.Joints)
                {
                    j.isKinematic = bIsKinematic;
                }
            }
        }

        public void SetCollidersEnabled(bool bEnabled)
        {
            if (this.Joints != null)
            {
                foreach (var j in this.Joints)
                {
                    var coll = j.GetComponent<Collider>();
                    if (coll != null) coll.enabled = bEnabled;
                }
            }
        }

        public void SetJointsLayer(int layer)
        {
            if (this.Joints != null)
            {
                foreach (var j in this.Joints)
                {
                    j.gameObject.layer = layer;
                }
            }
        }

        public void IgnoreCollision(ArmatureRig rig, bool ignore)
        {
            if (this.Joints == null || this.Joints.Length == 0) return;
            if (rig == null || rig.Joints == null || rig.Joints.Length == 0) return;
            IgnoreCollision((from j in rig.Joints where j.collider != null select j.collider).ToArray(), ignore);
        }

        public void IgnoreCollision(Collider collider, bool ignore)
        {
            if (this.Joints == null || this.Joints.Length == 0) return;
            if (collider == null) return;

            foreach (var j in this.Joints)
            {
                if (j.collider != null) Physics.IgnoreCollision(collider, j.collider, ignore);
            }
        }

        public void IgnoreCollision(Collider[] colliders, bool ignore)
        {
            if (this.Joints == null || this.Joints.Length == 0) return;
            if (colliders == null || colliders.Length == 0) return;

            foreach (var j in this.Joints)
            {
                if (j.collider != null)
                {
                    foreach (var c in colliders)
                    {
                        Physics.IgnoreCollision(j.collider, c, ignore);
                    }
                }
            }
        }


        public float CalculateMass()
        {
            if (Joints == null || Joints.Length == 0) return 0;
            return (from j in Joints select j.mass).Sum();
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

    }

}