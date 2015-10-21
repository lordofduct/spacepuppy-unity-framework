using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{

    [System.Obsolete("No longer used.")]
    public sealed class UnitySerializationInfo
    {

        #region Fields

        private const string EXCEPTION_INVALIDSTATE_MSG = "UnitySerializationInfo was not initialized properly and can not be accessed at this time.";

        private Stack<UnitySerializationContext> _contextStack = new Stack<UnitySerializationContext>();

        private List<UnityEngine.Object> _unityObjectReferences = new List<UnityEngine.Object>();

        #endregion

        #region CONSTRUCTOR

        internal UnitySerializationInfo()
        {
        }

        #endregion

        #region Properties

        public bool Ready { get { return _contextStack.Count > 0; } }

        #endregion

        #region Internal Interface

        private SerializationInfo _currentInfo { get { return _contextStack.Peek().SerializationInfo; } }

        internal List<UnityEngine.Object> UnityObjectReferences { get { return _unityObjectReferences; } }

        internal void Reset()
        {
            _unityObjectReferences.Clear();
        }

        internal void Reset(IEnumerable<UnityEngine.Object> refs)
        {
            _unityObjectReferences.Clear();
            if (refs != null) _unityObjectReferences.AddRange(refs);
        }

        internal void StartContext(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            _contextStack.Push(new UnitySerializationContext(info, context));
        }

        internal void EndContext()
        {
            _contextStack.Pop();
        }

        #endregion

        #region AddValue Methods

        public void AddUnityObjectReference(string name, UnityEngine.Object obj)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            if(_unityObjectReferences.Contains(obj))
            {
                _currentInfo.AddValue(name, _unityObjectReferences.IndexOf(obj));
            }
            else
            {
                int index = _unityObjectReferences.Count;
                _unityObjectReferences.Add(obj);
                _currentInfo.AddValue(name, index);
            }
        }

        public void AddValue(string name, object value, System.Type tp)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);
            if (value != null && !TypeUtil.IsType(value.GetType(), tp)) throw new TypeArgumentMismatchException(value.GetType(), tp, "Value must be of the type specified.", "value");

            if (TypeUtil.IsType(tp, typeof(UnityEngine.Object)))
            {
                this.AddUnityObjectReference(name, value as UnityEngine.Object);
            }
            else if(typeof(DummyList).IsAssignableFrom(tp))
            {
                _currentInfo.AddValue(name, value, tp);
            }
            else if (tp.IsListType(true))
            {
                this.AddList(name, value as System.Collections.IList, tp.GetElementTypeOfListType(), tp.IsArray);
            }
            else
            {
                _currentInfo.AddValue(name, value, tp);
            }
        }

        public void AddValue(string name, bool value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, byte value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, char value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.DateTime value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, decimal value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, double value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.Int16 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.Int32 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.Int64 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, sbyte value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, float value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, string value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.UInt16 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.UInt32 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        public void AddValue(string name, System.UInt64 value)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            _currentInfo.AddValue(name, value);
        }

        #endregion

        #region GetValue Methods

        public UnityEngine.Object GetUnityObjectReference(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            int index = _currentInfo.GetInt32(name);
            if(index >= 0 && index < _unityObjectReferences.Count)
            {
                return _unityObjectReferences[index];
            }
            else
            {
                return null;
            }
        }

        public object GetValue(string name, System.Type tp)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            if(TypeUtil.IsType(tp, typeof(UnityEngine.Object)))
            {
                return this.GetUnityObjectReference(name);
            }
            else if(tp.IsListType(true))
            {
                var value = _currentInfo.GetValue(name, typeof(DummyList)) as DummyList;

                if(value != null)
                {
                    return value.List;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _currentInfo.GetValue(name, tp);
            }
        }

        public bool GetBoolean(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetBoolean(name);
        }

        public byte GetByte(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetByte(name);
        }

        public char GetChar(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetChar(name);
        }

        public System.DateTime GetDateTime(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetDateTime(name);
        }

        public decimal GetDecimal(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetDecimal(name);
        }

        public double GetDouble(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetDouble(name);
        }

        public System.Int16 GetInt16(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetInt16(name);
        }

        public System.Int32 GetInt32(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetInt32(name);
        }

        public System.Int64 GetInt64(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetInt64(name);
        }

        public sbyte GetSByte(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetSByte(name);
        }

        public float GetSingle(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetSingle(name);
        }

        public string GetString(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetString(name);
        }

        public System.UInt16 GetUInt16(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetUInt16(name);
        }

        public System.UInt32 GetUInt32(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetUInt32(name);
        }

        public System.UInt64 GetUInt64(string name)
        {
            if (!this.Ready) throw new System.InvalidOperationException(EXCEPTION_INVALIDSTATE_MSG);

            return _currentInfo.GetUInt64(name);
        }

        #endregion

        #region AddList Methods

        public void AddList(string name, System.Collections.IList lst, System.Type elementType)
        {
            this.AddList(name, lst, elementType, false);
        }

        public void AddArray(string name, System.Array lst, System.Type elementType)
        {
            this.AddList(name, lst, elementType, true);
        }

        private void AddList(string name, System.Collections.IList lst, System.Type elementType, bool isArray)
        {
            var dummy = new DummyList(lst, elementType, isArray);
            _currentInfo.AddValue(name, dummy, typeof(DummyList));
        }

        #endregion

        #region GetList Methods

        public System.Collections.IList GetList(string name)
        {
            var dummy = _currentInfo.GetValue(name, typeof(DummyList)) as DummyList;
            if(dummy != null)
            {
                return dummy.List;
            }
            else
            {
                return null;
            }
        }

        public System.Array GetArray(string name)
        {
            var dummy = _currentInfo.GetValue(name, typeof(DummyList)) as DummyList;
            if (dummy != null)
            {
                return dummy.List as System.Array;
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region Special Types

        private struct UnitySerializationContext
        {

            public SerializationInfo SerializationInfo;
            public StreamingContext StreamingContext;

            public UnitySerializationContext(SerializationInfo info, StreamingContext context)
            {
                this.SerializationInfo = info;
                this.StreamingContext = context;
            }

        }


        [System.Serializable()]
        private class DummyList : IUnitySerializable, ISerializable
        {

            [System.NonSerialized()]
            private System.Collections.IList _lst;
            [System.NonSerialized()]
            private System.Type _elementType;
            [System.NonSerialized()]
            private bool _isArray;

            public DummyList()
            {

            }

            public DummyList(System.Collections.IList lst, System.Type elType, bool isArray)
            {
                _lst = lst;
                _elementType = elType;
                _isArray = isArray;
            }

            public System.Collections.IList List { get { return _lst; } }


            #region IUnitySerializable Interface

            public void GetObjectData(UnitySerializationInfo info)
            {
                int cnt = (_lst != null) ? _lst.Count : 0;
                info.AddValue("count", cnt);
                info.AddValue("assembly", _elementType.Assembly.GetName().Name);
                info.AddValue("type", _elementType.FullName);
                info.AddValue("isArray", _isArray);

                for (int i = 0; i < cnt; i++)
                {
                    info.AddValue("element" + i.ToString(), _lst[i], _elementType);
                }
            }

            public void SetObjectData(UnitySerializationInfo info)
            {
                var cnt = info.GetInt32("count");
                _elementType = TypeUtil.ParseType(info.GetString("assembly"), info.GetString("type"));
                _isArray = info.GetBoolean("isArray");

                if (_isArray)
                {
                    var arr = System.Array.CreateInstance(_elementType, cnt);
                    for (int i = 0; i < cnt; i++)
                    {
                        arr.SetValue(info.GetValue("element" + i.ToString(), _elementType), i);
                    }
                    _lst = arr as System.Collections.IList;
                }
                else
                {
                    var lstType = typeof(List<>);
                    var genLstType = lstType.MakeGenericType(_elementType);
                    var lst = System.Activator.CreateInstance(genLstType) as System.Collections.IList;
                    for (int i = 0; i < cnt; i++)
                    {
                        lst.Add(info.GetValue("element" + i.ToString(), _elementType));
                    }
                    _lst = lst;
                }
            }

            #endregion

            #region ISerializable Interface

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                int cnt = (_lst != null) ? _lst.Count : 0;
                info.AddValue("count", cnt);
                info.AddValue("assembly", _elementType.Assembly.GetName().Name);
                info.AddValue("type", _elementType.FullName);
                info.AddValue("isArray", _isArray);

                for (int i = 0; i < cnt; i++)
                {
                    info.AddValue("element" + i.ToString(), _lst[i]);
                }
            }

            protected DummyList(SerializationInfo info, StreamingContext context)
            {
                var cnt = info.GetInt32("count");
                _elementType = TypeUtil.ParseType(info.GetString("assembly"), info.GetString("type"));
                _isArray = info.GetBoolean("isArray");

                if (_isArray)
                {
                    var arr = System.Array.CreateInstance(_elementType, cnt);
                    for (int i = 0; i < cnt; i++)
                    {
                        arr.SetValue(info.GetValue("element" + i.ToString(), _elementType), i);
                    }
                    _lst = arr as System.Collections.IList;
                }
                else
                {
                    var lstType = typeof(List<>);
                    var genLstType = lstType.MakeGenericType(_elementType);
                    var lst = System.Activator.CreateInstance(genLstType) as System.Collections.IList;
                    for (int i = 0; i < cnt; i++)
                    {
                        lst.Add(info.GetValue("element" + i.ToString(), _elementType));
                    }
                    _lst = lst;
                }
            }

            #endregion

        }

        #endregion


        #region Static Utils

        private static string CalculateDepthId(Stack<UnitySerializationContext> stack)
        {
            return string.Join("->", (from c in stack.Reverse() select c.SerializationInfo.FullTypeName).ToArray());
        }

        #endregion

    }

}
