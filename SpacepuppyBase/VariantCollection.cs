using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using com.spacepuppy.Dynamic;
using System;
using System.Collections;

namespace com.spacepuppy
{

    [System.Serializable()]
    public class VariantCollection : IStateToken, ISerializationCallbackReceiver, ISerializable, IEnumerable<KeyValuePair<string, object>>
    {
        
        #region Fields

        [System.NonSerialized()]
        private Dictionary<string, VariantReference> _table = new Dictionary<string,VariantReference>();

        [SerializeField()]
        private string[] _keys;
        [SerializeField()]
        private VariantReference[] _values;
        
        #endregion

        #region CONSTRUCTOR

        public VariantCollection()
        {
        }
        
        #endregion

        #region Properties
        
        public object this[string key]
        {
            get
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    return v.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    v.Value = value;
                }
                else
                {
                    _table.Add(key, new VariantReference(value));
                }
            }
        }

        public IEnumerable<string> Names { get { return _table.Keys; } }

        public int Count
        {
            get { return _table.Count; }
        }

        #endregion

        #region Methods

        public object GetValue(string key)
        {
            return this[key];
        }

        public bool TryGetValue(string skey, out object result)
        {
            VariantReference v;
            if(_table.TryGetValue(skey, out v))
            {
                result = v.Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public void SetValue(string key, object value)
        {
            this[key] = value;
        }

        public VariantReference GetVariant(string key)
        {
            return _table[key];
        }

        public VariantReference GetVariant(string key, bool createIfNotExist)
        {
            if(createIfNotExist)
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    return v;
                }
                else
                {
                    v = new VariantReference();
                    _table.Add(key, v);
                    return v;
                }
            }
            else
            {
                return _table[key];
            }
        }

        public bool GetBool(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.BoolValue;
            else
                return false;
        }

        public void SetBool(string key, bool value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.BoolValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    BoolValue = value
                });
            }
        }

        public int GetInt(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.IntValue;
            else
                return 0;
        }

        public void SetInt(string key, int value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.IntValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    IntValue = value
                });
            }
        }

        public float GetFloat(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.FloatValue;
            else
                return 0f;
        }

        public void SetFloat(string key, float value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.FloatValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    FloatValue = value
                });
            }
        }

        public double GetDouble(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.DoubleValue;
            else
                return 0f;
        }

        public void SetDouble(string key, double value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.DoubleValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    DoubleValue = value
                });
            }
        }

        public Vector2 GetVector2(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.Vector2Value;
            else
                return Vector2.zero;
        }

        public void SetVector2(string key, Vector2 value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.Vector2Value = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    Vector2Value = value
                });
            }
        }

        public Vector3 GetVector3(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.Vector3Value;
            else
                return Vector3.zero;
        }

        public void SetVector3(string key, Vector3 value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.Vector3Value = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    Vector3Value = value
                });
            }
        }

        public Quaternion GetQuaternion(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.QuaternionValue;
            else
                return Quaternion.identity;
        }

        public void SetQuaternion(string key, Quaternion value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.QuaternionValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    QuaternionValue = value
                });
            }
        }

        public Color GetColor(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.ColorValue;
            else
                return Color.black;
        }

        public void SetColor(string key, Color value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.ColorValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    ColorValue = value
                });
            }
        }





        public bool HasMember(string key)
        {
            return _table.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _table.Remove(key);
        }

        #endregion

        #region IToken Interface

        /// <summary>
        /// Iterates over members of the collection and attempts to set them to an object as if they 
        /// were property names on that object.
        /// </summary>
        /// <param name="obj"></param>
        public void CopyTo(object obj)
        {
            var e = _table.GetEnumerator();
            while(e.MoveNext())
            {
                DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value.Value);
            }
        }

        /// <summary>
        /// Iterates over keys in this collection and attempts to update the values associated with that 
        /// key to the value pulled from a property on object.
        /// </summary>
        /// <param name="obj"></param>
        public void SyncFrom(object obj)
        {
            var e = _table.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Value.Value = DynamicUtil.GetValue(obj, e.Current.Key);
            }
        }

        /// <summary>
        /// Lerp the target objects values to the state of the VarianteCollection. If the member doesn't have a current state/undefined, 
        /// then the member is set to the current state in this VariantCollection.
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
                    value = Evaluator.TryLerp(value, e.Current.Value.Value, t);
                    DynamicUtil.SetValue(obj, e.Current.Key, value);
                }
                else
                {
                    DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value.Value);
                }
            }
        }

        public void TweenTo(com.spacepuppy.Tween.TweenHash hash, com.spacepuppy.Tween.Ease ease, float dur)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                var value = e.Current.Value.Value;
                if (value == null) continue;

                switch(VariantReference.GetVariantType(value.GetType()))
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
                        hash.To(e.Current.Key, ease, dur, value);
                        break;
                }
            }
        }

        #endregion

        #region ITokenizable Interface

        public object CreateStateToken()
        {
            if (_table.Count == 0) return com.spacepuppy.Utils.ArrayUtil.Empty<KeyValuePair<string, VariantReference>>();
            KeyValuePair<string, VariantReference>[] arr = new KeyValuePair<string, VariantReference>[_table.Count];
            var e = _table.GetEnumerator();
            int i = 0;
            while(e.MoveNext())
            {
                arr[i] = e.Current;
                i++;
            }
            return arr;
        }

        public void RestoreFromStateToken(object token)
        {
            if(token is KeyValuePair<string, VariantReference>[])
            {
                _table.Clear();
                var arr = token as KeyValuePair<string, VariantReference>[];
                foreach(var pair in arr)
                {
                    _table[pair.Key] = pair.Value;
                }
            }
            else
            {
                DynamicUtil.CopyState(this, token);
            }
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get { return (this as IDynamic).GetValue(sMemberName); }
            set { (this as IDynamic).SetValue(sMemberName, value); }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            if (_table.ContainsKey(sMemberName))
            {
                this[sMemberName] = value;
                return true;
            }
            else if (DynamicUtil.HasMemberDirect(this, sMemberName, true))
                return DynamicUtil.SetValueDirect(this, sMemberName, value, index);
            else
            {
                this[sMemberName] = value;
                return true;
            }
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            if (_table.ContainsKey(sMemberName))
                return this[sMemberName];
            else
                return DynamicUtil.GetValueDirect(this, sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            if (_table.ContainsKey(sMemberName))
            {
                result = this[sMemberName];
                return true;
            }
            else
                return DynamicUtil.TryGetValueDirect(this, sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            //throw new System.NotSupportedException();
            return DynamicUtil.InvokeMethodDirect(this, sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            if (_table.ContainsKey(sMemberName))
                return true;
            else
                return DynamicUtil.TypeHasMember(this.GetType(), sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            var tp = this.GetType();
            if(Application.isEditor && !Application.isPlaying)
            {
                var ptp = typeof(Variant);
                for(int i = 0; i < _keys.Length; i++)
                {
                    yield return new DynamicPropertyInfo(_keys[i], tp, ptp);
                }
            }
            else
            {
                var ptp = typeof(Variant);
                var e = _table.GetEnumerator();
                while(e.MoveNext())
                {
                    yield return new DynamicPropertyInfo(e.Current.Key, tp, ptp);
                }
            }

            foreach(var p in DynamicUtil.GetMembersFromType(tp, includeNonPublic))
            {
                if(p.Name != "_table" && p.Name != "_values" && p.Name != "_keys")
                    yield return p;
            }
        }

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return _table.Keys;
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if(_keys.Contains(sMemberName)) return new DynamicPropertyInfo(sMemberName, this.GetType(), typeof(Variant));
            }
            else if(_table.ContainsKey(sMemberName))
            {
                return new DynamicPropertyInfo(sMemberName, this.GetType(), typeof(Variant));
            }

            return DynamicUtil.GetMemberFromType(this.GetType(), sMemberName, includeNonPublic);
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _table.Clear();
            var cnt = Mathf.Min(_values.Length, _keys.Length);
            for(int i = 0; i < cnt; i++)
            {
                _table.Add(_keys[i], _values[i]);
            }

            //_keys = null;
            //_values = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keys = _table.Keys.ToArray();
            _values = _table.Values.ToArray();
        }

        #endregion

        #region ISerializable Interface

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            (this as ISerializationCallbackReceiver).OnBeforeSerialize();
            info.AddValue("_keys", _keys);
            info.AddValue("_values", _values);
        }

        protected VariantCollection(SerializationInfo info, StreamingContext context)
        {
            _keys = info.GetValue("_keys", typeof(string[])) as string[];
            _values = info.GetValue("_values", typeof(VariantReference[])) as VariantReference[];

            (this as ISerializationCallbackReceiver).OnAfterDeserialize();
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, object>>
        {

            private Dictionary<string, VariantReference>.Enumerator _e;
            private KeyValuePair<string, object> _current;

            internal Enumerator(VariantCollection coll)
            {
                _e = coll._table.GetEnumerator();
                _current = default(KeyValuePair<string, object>);
            }



            public KeyValuePair<string, object> Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
                _current = default(KeyValuePair<string, object>);
            }

            public bool MoveNext()
            {
                if(_e.MoveNext())
                {
                    _current = new KeyValuePair<string, object>(_e.Current.Key, _e.Current.Value.Value);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as System.Collections.IEnumerator).Reset();
            }
        }

        #endregion

        #region Special Types

        /// <summary>
        /// Configure the list to include name/type pairs reflected from a target type.
        /// </summary>
        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class AsPropertyListAttribute : System.Attribute
        {

            private System.Type _tp;

            public AsPropertyListAttribute(System.Type tp)
            {
                _tp = tp;
            }

            public System.Type TargetType { get { return _tp; } }

        }

        /// <summary>
        /// Configure the list to only accept a single type through the inspector.
        /// </summary>
        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class AsTypedList : System.Attribute
        {

            private System.Type _tp;

            public AsTypedList(System.Type tp)
            {
                _tp = tp;
            }

            public System.Type TargetType { get { return _tp; } }

        }





        [System.Obsolete("No longer used by VariantCollectionPropertyDrawer")]
        private class EditorHelper : System.Collections.IList
        {

            private VariantCollection _coll;

            #region Properties

            public VariantCollection Collection { get { return _coll; } }

            public int Count { get { return (_coll == null) ? 0 : _coll._table.Count; } }

            public System.Type EntryType { get { return typeof(KeyValuePair<string, VariantReference>); } }

            #endregion

            #region Methods

            public void UpdateCollection(VariantCollection coll)
            {
                _coll = coll;
            }

            public string GetNameAt(int index)
            {
                if (_coll == null) return null;

                if (index < 0 || index >= _coll._table.Count) throw new System.IndexOutOfRangeException();
                int i = 0;
                foreach (var key in _coll._table.Keys)
                {
                    if (i == index) return key;
                    i++;
                }
                return null;
            }

            public VariantReference GetVariant(string key)
            {
                if (_coll == null) return null;

                return _coll.GetVariant(key);
            }

            public VariantReference AddEntry()
            {
                if (_coll == null) return null;

                int cnt = _coll._table.Count + 1;
                string key = "Entry " + cnt.ToString();
                while(_coll._table.ContainsKey(key))
                {
                    cnt++;
                    key = "Entry " + cnt.ToString();
                }

                var v = new VariantReference();
                _coll._table.Add(key, v);
                return v;
            }

            public bool ChangeEntryName(string name, string newName)
            {
                if (_coll == null) return false;
                if (string.IsNullOrEmpty(newName)) return false;

                var names = _coll._table.Keys.ToArray();
                int index = System.Array.IndexOf(names, name);
                if (index < 0 || names.Contains(newName)) return false;

                var values = _coll._table.Values.ToArray();
                _coll._table.Clear();
                names[index] = newName;

                for(int i = 0; i < names.Length; i++)
                {
                    _coll._table.Add(names[i], values[i]);
                }
                return true;
            }

            #endregion


            #region IList Interface

            int System.Collections.IList.Add(object value)
            {
                if (_coll == null) return -1;
                var key = System.Convert.ToString(value);
                if (_coll._table.ContainsKey(key)) return -1;

                _coll._table.Add(key, new VariantReference());
                return _coll._table.Count - 1;
            }

            void System.Collections.IList.Clear()
            {
                if (_coll == null) return;

                _coll._table.Clear();
            }

            bool System.Collections.IList.Contains(object value)
            {
                if (_coll == null) return false;
                return _coll.HasMember(System.Convert.ToString(value));
            }

            int System.Collections.IList.IndexOf(object value)
            {
                if(_coll == null) return -1;

                string svalue = System.Convert.ToString(value);
                int i = 0;
                foreach(var key in _coll._table.Keys)
                {
                    if (key == svalue) return i;
                    i++;
                }
                return -1;
            }

            void System.Collections.IList.Insert(int index, object value)
            {
                throw new System.NotSupportedException();
            }

            bool System.Collections.IList.IsFixedSize
            {
                get { return false; }
            }

            bool System.Collections.IList.IsReadOnly
            {
                get { return false; }
            }

            void System.Collections.IList.Remove(object value)
            {
                if (_coll == null) return;
                var key = System.Convert.ToString(value);
                _coll._table.Remove(key);
            }

            void System.Collections.IList.RemoveAt(int index)
            {
                if (_coll == null) return;

                var key = this.GetNameAt(index);
                _coll.Remove(key);
            }

            object System.Collections.IList.this[int index]
            {
                get
                {
                    return this.GetNameAt(index);
                }
                set
                {
                    if (_coll == null) return;
                    string key = this.GetNameAt(index);
                    this.ChangeEntryName(key, System.Convert.ToString(value));
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                if (_coll == null) return Enumerable.Empty<string>().GetEnumerator();

                return _coll._table.Keys.GetEnumerator();
            }

            void System.Collections.ICollection.CopyTo(System.Array array, int index)
            {
                if (_coll == null) return;

                (_coll._table.Keys as System.Collections.ICollection).CopyTo(array, index);
            }

            int System.Collections.ICollection.Count
            {
                get { return (_coll == null) ? 0 : _coll._table.Count; }
            }

            bool System.Collections.ICollection.IsSynchronized
            {
                get { return false; }
            }

            object System.Collections.ICollection.SyncRoot
            {
                get { return this; }
            }

            #endregion

        }

        #endregion

    }

}
