using System;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Dynamic
{

    /// <summary>
    /// A dynamic class for storing the state of another object in. This can be used for serializing arbitrary state information, 
    /// or used with the tween engine for dynamically configured state animation. As well as any number of other applications you may deem fit.
    /// </summary>
    [System.Serializable]
    public class StateToken : IDynamic, IEnumerable<KeyValuePair<string, object>>, System.IDisposable, System.Runtime.Serialization.ISerializable
    {

        private const int TEMP_STACKSIZE = 3;

        #region Fields

        private Dictionary<string, object> _table = new Dictionary<string, object>();

        #endregion

        #region Constructor

        public StateToken()
        {

        }

        #endregion

        #region Properties

        public object this[string sMemberName]
        {
            get
            {
                return this.GetValue(sMemberName);
            }
            set
            {
                this.SetValue(sMemberName, value);
            }
        }

        #endregion

        #region Methods

        public void SetValue(string skey, object value)
        {
            _table[skey] = value;
        }

        public bool LerpValue(string skey, object value, float t)
        {
            object a;
            if (_table.TryGetValue(skey, out a))
            {
                a = Evaluator.TryLerp(a, value, t);
                _table[skey] = a;
                return true;
            }
            else
            {
                return false;
            }
        }

        public object GetValue(string skey)
        {
            if (_table.ContainsKey(skey))
                return _table[skey];
            else
                return null;
        }

        public bool TryGetValue(string skey, out object result)
        {
            return _table.TryGetValue(skey, out result);
        }

        public T GetValue<T>(string skey)
        {
            object obj;
            if (_table.TryGetValue(skey, out obj))
            {
                if (obj is T) return (T)obj;
                if (ConvertUtil.IsSupportedType(typeof(T))) return ConvertUtil.ToPrim<T>(obj);
                return default(T);
            }
            else
            {
                return default(T);
            }
        }

        public bool TryGetValue<T>(string skey, out T result)
        {
            object obj;
            if (_table.TryGetValue(skey, out obj))
            {
                if (obj is T)
                {
                    result = (T)obj;
                    return true;
                }
                else if (ConvertUtil.IsSupportedType(typeof(T)))
                {
                    result = ConvertUtil.ToPrim<T>(obj);
                    return true;
                }
            }

            result = default(T);
            return false;
        }

        public bool HasKey(string skey)
        {
            return _table.ContainsKey(skey);
        }

        /// <summary>
        /// Iterates over members of the collection and attempts to set them to an object as if they 
        /// were property names on that object.
        /// </summary>
        /// <param name="obj"></param>
        public void CopyTo(object obj)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value);
            }
        }

        /// <summary>
        /// Iterates over keys in this collection and attempts to update the values associated with that 
        /// key to the value pulled from a property on object.
        /// </summary>
        /// <param name="obj"></param>
        public void SyncFrom(object obj)
        {
            using (var lst = TempCollection.GetList<string>())
            {
                var e = _table.Keys.GetEnumerator();
                while (e.MoveNext())
                {
                    lst.Add(e.Current);
                }

                var e2 = lst.GetEnumerator();
                while (e2.MoveNext())
                {
                    _table[e2.Current] = DynamicUtil.GetValue(obj, e2.Current);
                }
            }
        }

        /// <summary>
        /// Dumps the keys in this collection, and then copies keys to match the entire state of the passed in object.
        /// </summary>
        /// <param name="obj"></param>
        public void CopyFrom(object obj)
        {
            _table.Clear();
            foreach (var m in DynamicUtil.GetMembers(obj, false, System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Field))
            {
                _table[m.Name] = DynamicUtil.GetValue(obj, m);
            }
        }

        /// <summary>
        /// Lerp the target objects values to the state of the StateToken. If the member doesn't have a current state/undefined, 
        /// then the member is set to the current state in this StateToken.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        public void LerpTo(object obj, float t)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                object value;
                if (DynamicUtil.TryGetValue(obj, e.Current.Key, out value))
                {
                    value = Evaluator.TryLerp(value, e.Current.Value, t);
                    DynamicUtil.SetValue(obj, e.Current.Key, value);
                }
                else
                {
                    DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value);
                }
            }
        }

        public void TweenTo(com.spacepuppy.Tween.TweenHash hash, com.spacepuppy.Tween.Ease ease, float dur)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                var value = e.Current.Value;
                if (value == null) continue;

                switch (VariantReference.GetVariantType(value.GetType()))
                {
                    case VariantType.Integer:
                    case VariantType.Float:
                    case VariantType.Double:
                    case VariantType.Vector2:
                    case VariantType.Vector3:
                    case VariantType.Vector4:
                    case VariantType.Quaternion:
                    case VariantType.Color:
                    case VariantType.Rect:
                        hash.To(e.Current.Key, ease, value, dur);
                        break;
                }
            }
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return this.GetValue(sMemberName);
            }
            set
            {
                this.SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            this.SetValue(sMemberName, value);
            return true;
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            return this.GetValue(sMemberName);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            return this.TryGetValue(sMemberName, out result);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            return this.GetValue(sMemberName);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            return this.HasKey(sMemberName);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            var tp = this.GetType();
            foreach (var k in _table.Keys)
            {
                yield return new DynamicPropertyInfo(k, tp);
            }
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (_table.ContainsKey(sMemberName))
                return new DynamicPropertyInfo(sMemberName, this.GetType());
            else
                return null;
        }

        #endregion

        #region IEnumerable Interface

        public Dictionary<string, object>.Enumerator GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        #endregion

        #region IDisposable Interface

        void IDisposable.Dispose()
        {
            _table.Clear();
            if (_tempTokens == null) _tempTokens = new ObjectCachePool<StateToken>(TEMP_STACKSIZE);
            _tempTokens.Release(this);
        }

        #endregion

        #region ISerializable Interface

        protected StateToken(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            var e = info.GetEnumerator();
            while (e.MoveNext())
            {
                _table[e.Current.Name] = e.Current.Value;
            }
        }

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                try
                {
                    info.AddValue(e.Current.Key, e.Current.Value);
                }
                catch (System.Exception)
                {
                    //tried to add a non-serializable value
                }
            }
        }

        #endregion

        #region Static Utils

        private static ObjectCachePool<StateToken> _tempTokens;

        public static StateToken GetTempToken()
        {
            StateToken t;
            if (_tempTokens != null && _tempTokens.TryGetInstance(out t))
            {
                return t;
            }
            else
            {
                return new StateToken();
            }
        }

        public static void ReleaseTempToken(StateToken token)
        {
            if (token != null) (token as System.IDisposable).Dispose();
        }

        #endregion

    }

}
