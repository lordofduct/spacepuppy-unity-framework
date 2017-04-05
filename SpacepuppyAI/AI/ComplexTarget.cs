using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.AI.Sensors;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI
{

    public enum ComplexTargetType
    {
        Null = 0,
        Aspect = 1,
        Transform = 2,
        Vector2 = 3,
        Vector3 = 4
    }

    public struct ComplexTarget
    {

        public static IPlanarSurface DefualtSurface;

        #region Fields

        public readonly ComplexTargetType TargetType;
        private readonly object _target;
        private readonly Vector3 _vector;
        private readonly IPlanarSurface _surface;

        #endregion

        #region CONSTRUCTOR

        public ComplexTarget(IAspect aspect)
        {
            if (aspect != null)
            {
                TargetType = ComplexTargetType.Aspect;
                _target = aspect;
            }
            else
            {
                TargetType = ComplexTargetType.Null;
                _target = null;
            }
            _vector = Vector3.zero;
            _surface = DefualtSurface;
        }

        public ComplexTarget(Transform target)
        {
            if (target != null)
            {
                TargetType = ComplexTargetType.Transform;
                _target = target;
            }
            else
            {
                TargetType = ComplexTargetType.Null;
                _target = null;
            }
            _vector = Vector3.zero;
            _surface = DefualtSurface;
        }

        public ComplexTarget(Vector2 location)
        {
            TargetType = ComplexTargetType.Vector2;
            _target = null;
            _vector = (Vector3)location;
            _surface = DefualtSurface;
        }

        public ComplexTarget(Vector3 location)
        {
            TargetType = ComplexTargetType.Vector3;
            _target = null;
            _vector = location;
            _surface = DefualtSurface;
        }

        public ComplexTarget(IAspect aspect, IPlanarSurface surface)
        {
            if (aspect != null)
            {
                TargetType = ComplexTargetType.Aspect;
                _target = aspect;
            }
            else
            {
                TargetType = ComplexTargetType.Null;
                _target = null;
            }
            _vector = Vector3.zero;
            _surface = surface;
        }

        public ComplexTarget(Transform target, IPlanarSurface surface)
        {
            if (target != null)
            {
                TargetType = ComplexTargetType.Transform;
                _target = target;
            }
            else
            {
                TargetType = ComplexTargetType.Null;
                _target = null;
            }
            _vector = Vector3.zero;
            _surface = surface;
        }

        public ComplexTarget(Vector2 location, IPlanarSurface surface)
        {
            TargetType = ComplexTargetType.Vector2;
            _target = null;
            _vector = (Vector3)location;
            _surface = surface;
        }

        public ComplexTarget(Vector3 location, IPlanarSurface surface)
        {
            TargetType = ComplexTargetType.Vector2;
            _target = null;
            _vector = location;
            _surface = surface;
        }

        #endregion

        #region Properties

        public Vector2 Position2D
        {
            get
            {
                switch (this.TargetType)
                {
                    case ComplexTargetType.Null:
                        return Vector2.zero;
                    case ComplexTargetType.Aspect:
                        var a = _target as IAspect;
                        if (a.IsNullOrDestroyed()) return Vector2.zero;
                        else if (_surface == null) return ConvertUtil.ToVector2(a.transform.position);
                        else return _surface.ProjectPosition2D(a.transform.position);
                    case ComplexTargetType.Transform:
                        var t = _target as Transform;
                        if (t.IsNullOrDestroyed()) return Vector2.zero;
                        else if (_surface == null) return ConvertUtil.ToVector2(t.position);
                        else return _surface.ProjectPosition2D(t.position);
                    case ComplexTargetType.Vector2:
                        return ConvertUtil.ToVector2(_vector);
                    case ComplexTargetType.Vector3:
                        if (_surface == null) return ConvertUtil.ToVector2(_vector);
                        else return _surface.ProjectPosition2D(_vector);
                    default:
                        return Vector2.zero;
                }
            }
        }

        public Vector3 Position
        {
            get
            {
                switch (this.TargetType)
                {
                    case ComplexTargetType.Null:
                        return Vector3.zero;
                    case ComplexTargetType.Aspect:
                        var a = _target as IAspect;
                        if (a == null) return Vector3.zero;
                        else return a.transform.position;
                    case ComplexTargetType.Transform:
                        var t = _target as Transform;
                        if (t == null) return Vector3.zero;
                        else return t.position;
                    case ComplexTargetType.Vector2:
                        if (_surface == null) return _vector.SetZ(0f);
                        else return _surface.ProjectPosition3D(ConvertUtil.ToVector2(_vector));
                    case ComplexTargetType.Vector3:
                        return _vector;
                    default:
                        return Vector3.zero;
                }
            }
        }

        public IAspect TargetAspect { get { return _target as IAspect; } }

        public Transform Transform
        {
            get
            {
                switch (TargetType)
                {
                    case ComplexTargetType.Aspect:
                        var a = _target as IAspect;
                        if (a == null) return null;
                        else return a.transform;
                    case ComplexTargetType.Transform:
                        return _target as Transform;
                    default:
                        return null;
                }
            }
        }

        public bool IsNull
        {
            get
            {
                switch (this.TargetType)
                {
                    case ComplexTargetType.Null:
                        return true;
                    case ComplexTargetType.Aspect:
                    case ComplexTargetType.Transform:
                        return _target.IsNullOrDestroyed();
                    case ComplexTargetType.Vector2:
                    case ComplexTargetType.Vector3:
                    default:
                        return false;
                }
            }
        }

        #endregion

        #region Static Methods

        public static ComplexTarget FromObject(object targ)
        {
            if (targ == null) return new ComplexTarget();

            if (targ is Component)
            {
                if (targ is IAspect)
                    return new ComplexTarget(targ as IAspect);
                else if (targ is Transform)
                    return new ComplexTarget(targ as Transform);
                else
                    return new ComplexTarget((targ as Component).transform);
            }
            else if (targ is GameObject)
                return new ComplexTarget((targ as GameObject).transform);
            else if (targ is Vector2)
                return new ComplexTarget((Vector2)targ);
            else if (targ is Vector3)
                return new ComplexTarget((Vector3)targ);
            else
                return new ComplexTarget();
        }

        public static ComplexTarget Null { get { return new ComplexTarget(); } }

        //public static implicit operator ComplexTarget(IAspect o)
        //{
        //    return new ComplexTarget(o);
        //}

        public static implicit operator ComplexTarget(Transform o)
        {
            return new ComplexTarget(o);
        }

        public static implicit operator ComplexTarget(Vector2 v)
        {
            return new ComplexTarget(v);
        }

        public static implicit operator ComplexTarget(Vector3 v)
        {
            return new ComplexTarget(v);
        }

        public static implicit operator ComplexTarget(GameObject go)
        {
            if (go == null) return new ComplexTarget();
            else return new ComplexTarget(go.transform);
        }

        public static implicit operator ComplexTarget(Component c)
        {
            if (c == null) return new ComplexTarget();
            if (c is IAspect)
                return new ComplexTarget(c as IAspect);
            else if (c is Transform)
                return new ComplexTarget(c as Transform);
            else
                return new ComplexTarget(c.transform);
        }

        #endregion

    }

}
