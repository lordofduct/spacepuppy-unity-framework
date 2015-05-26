using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.spacepuppy.Dynamic
{

    /// <summary>
    /// Facilitates creating a wrapper to access an object of an otherwise unknown type. This is useful for reflecting out an internal 
    /// class in an assembly you don't have direct access to.
    /// </summary>
    public class TypeAccessWrapper
    {

        private const BindingFlags PUBLIC_MEMBERS = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags PUBLIC_STATIC_MEMBERS = BindingFlags.Static | BindingFlags.Public;

        #region Fields

        private object _wrappedObject;
        private Type _wrappedType;

        private bool _includeNonPublic = false;

        #endregion

        #region CONSTRUCTOR

        public TypeAccessWrapper(Type type, bool includeNonPublic = false)
        {
            if (type == null) throw new ArgumentNullException("type");

            _wrappedType = type;
            _includeNonPublic = includeNonPublic;
            this.WrappedObject = null;
        }

        public TypeAccessWrapper(Type type, object obj, bool includeNonPublic = false)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (obj != null && !type.IsAssignableFrom(obj.GetType())) throw new ArgumentException("Wrapped Object must be of type assignable to target type.");

            _wrappedType = type;
            _includeNonPublic = includeNonPublic;
            this.WrappedObject = obj;
        }

        #endregion

        #region Properties

        public Type WrappedType { get { return _wrappedType; } }

        public object WrappedObject
        {
            get { return _wrappedObject; }
            set
            {
                if (value != null && !_wrappedType.IsAssignableFrom(value.GetType())) throw new ArgumentException("Wrapped Object must be of type assignable to target type.");
                _wrappedObject = value;
            }
        }

        public bool IncludeNonPublic { get { return _includeNonPublic; } set { _includeNonPublic = value; } }

        #endregion

        #region Instance Acccess

        public MemberInfo[] GetMembers()
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;
            return _wrappedType.GetMembers(binding);
        }

        public MethodInfo[] GetMethods()
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;
            return _wrappedType.GetMethods(binding);
        }

        public string[] GetPropertyNames()
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;
            return (from p in _wrappedType.GetProperties(binding) select p.Name).Union(from f in _wrappedType.GetFields(binding) select f.Name).ToArray();
        }

        public Delegate GetMethod(string name, System.Type delegShape)
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");
            if (!delegShape.IsSubclassOf(typeof(Delegate))) throw new ArgumentException("Type must inherit from Delegate.", "delegShape");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var invokeMeth = delegShape.GetMethod("Invoke");
            var paramTypes = (from p in invokeMeth.GetParameters() select p.ParameterType).ToArray();
            var meth = _wrappedType.GetMethod(name, binding, null, paramTypes, null);

            if (meth != null)
            {
                try
                {
                    return Delegate.CreateDelegate(delegShape, _wrappedObject, meth);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("A method matching the name and shape requested could not be found.", ex);
                }
            }
            else
            {
                throw new InvalidOperationException("A method matching the name and shape requested could not be found.");
            }

        }

        public object CallMethod(string name, System.Type delegShape, params object[] args)
        {
            var d = GetMethod(name, delegShape);
            return d.DynamicInvoke(args);
        }

        public object GetProperty(string name)
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var prop = _wrappedType.GetProperty(name, binding, null, null, Type.EmptyTypes, null);
            if (prop != null)
            {
                return prop.GetValue(_wrappedObject, null);
            }

            var field = _wrappedType.GetField(name, binding);
            if (field != null)
            {
                return field.GetValue(_wrappedObject);
            }

            return null;
        }

        public void SetProperty(string name, object value)
        {
            if (_wrappedObject == null) throw new InvalidOperationException("Can only access static members.");

            var binding = PUBLIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var prop = _wrappedType.GetProperty(name, binding, null, null, Type.EmptyTypes, null);
            if (prop != null)
            {
                try
                {
                    prop.SetValue(_wrappedObject, value, null);
                    return;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Mismatch when attempting to set property.", ex);
                }
            }

            var field = _wrappedType.GetField(name, binding);
            if (field != null)
            {
                try
                {
                    field.SetValue(_wrappedObject, value);
                    return;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Mismatch when attempting to set property.", ex);
                }
            }
        }

        #endregion

        #region Static Access

        public MemberInfo[] GetStaticMembers()
        {
            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            return _wrappedType.GetMembers(binding);
        }

        public MethodInfo[] GetStaticMethods()
        {
            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            return _wrappedType.GetMethods(binding);
        }

        public string[] GetStaticPropertyNames()
        {
            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            return (from p in _wrappedType.GetProperties(binding) select p.Name).Union(from f in _wrappedType.GetFields(binding) select f.Name).ToArray();
        }

        public Delegate GetStaticMethod(string name, System.Type delegShape)
        {
            if (!delegShape.IsSubclassOf(typeof(Delegate))) throw new ArgumentException("Type must inherit from Delegate.", "delegShape");

            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var invokeMeth = delegShape.GetMethod("Invoke");
            var paramTypes = (from p in invokeMeth.GetParameters() select p.ParameterType).ToArray();
            var meth = _wrappedType.GetMethod(name, binding, null, paramTypes, null);

            if (meth != null)
            {
                try
                {
                    return Delegate.CreateDelegate(delegShape, meth);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("A method matching the name and shape requested could not be found.", ex);
                }
            }
            else
            {
                throw new InvalidOperationException("A method matching the name and shape requested could not be found.");
            }

        }

        public object CallStaticMethod(string name, System.Type delegShape, params object[] args)
        {
            var d = GetMethod(name, delegShape);
            return d.DynamicInvoke(args);
        }

        public object GetStaticProperty(string name)
        {
            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var prop = _wrappedType.GetProperty(name, binding, null, null, Type.EmptyTypes, null);
            if (prop != null)
            {
                return prop.GetValue(null, null);
            }

            var field = _wrappedType.GetField(name, binding);
            if (field != null)
            {
                return field.GetValue(null);
            }

            return null;
        }

        public void SetStaticProperty(string name, object value)
        {
            var binding = PUBLIC_STATIC_MEMBERS;
            if (_includeNonPublic) binding |= BindingFlags.NonPublic;

            var prop = _wrappedType.GetProperty(name, binding, null, null, Type.EmptyTypes, null);
            if (prop != null)
            {
                try
                {
                    prop.SetValue(null, value, null);
                    return;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Mismatch when attempting to set property.", ex);
                }
            }

            var field = _wrappedType.GetField(name, binding);
            if (field != null)
            {
                try
                {
                    field.SetValue(null, value);
                    return;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Mismatch when attempting to set property.", ex);
                }
            }
        }

        #endregion

    }

}
