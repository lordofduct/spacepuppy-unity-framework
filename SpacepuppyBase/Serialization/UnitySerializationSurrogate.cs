using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System.Runtime.Serialization;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization
{

    [System.Obsolete("No longer used.")]
    internal class UnitySerializationSurrogate : ISerializationSurrogate, ISurrogateSelector
    {

        #region Fields

        private UnitySerializationInfo _unitySerialInfo = new UnitySerializationInfo();

        #endregion

        #region Properties

        internal void StartSerialization()
        {
            _unitySerialInfo.Reset();
        }

        internal UnityEngine.Object[] StopSerialization()
        {
            var refs = _unitySerialInfo.UnityObjectReferences.ToArray();
            _unitySerialInfo.Reset();
            return refs;
        }

        internal void StartDeserialization(IEnumerable<UnityEngine.Object> refs)
        {
            _unitySerialInfo.Reset(refs);
        }

        internal void StopDeserialization()
        {
            _unitySerialInfo.Reset();
        }

        #endregion

        #region ISerializationSurrogate Interface

        public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _unitySerialInfo.StartContext(info, context);

            var tp = obj.GetType();
            if (obj is IUnitySerializable)
            {
                (obj as IUnitySerializable).GetObjectData(_unitySerialInfo);
            }
            else
            {
                foreach(var fi in GetAllSerializableFields(tp))
                {
                    try
                    {
                        _unitySerialInfo.AddValue(fi.Name, fi.GetValue(obj), fi.FieldType);
                    } catch (SerializationException ex)
                    {
                        UnityEngine.Debug.LogWarning("Spacepuppy Unity Serialization: " + ex.Message);
                    }
                }
            }
            _unitySerialInfo.EndContext();
        }

        public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
        {
            _unitySerialInfo.StartContext(info, context);

            if (obj is IUnitySerializable)
            {
                (obj as IUnitySerializable).SetObjectData(_unitySerialInfo);
            }
            else
            {
                var tp = obj.GetType();
                foreach (var fi in GetAllSerializableFields(tp))
                {
                    try
                    {
                        var value = _unitySerialInfo.GetValue(fi.Name, fi.FieldType);
                        fi.SetValue(obj, value);
                    }
                    catch
                    {
                        //do nothing
                    }
                }
            }
            _unitySerialInfo.EndContext();

            return obj;
        }

        #endregion

        #region ISurrogateSelector Interface

        void ISurrogateSelector.ChainSelector(ISurrogateSelector selector)
        {
        }

        ISurrogateSelector ISurrogateSelector.GetNextSelector()
        {
            return null;
        }

        ISerializationSurrogate ISurrogateSelector.GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (UnityDataFormatter.IsUnitySerializable(type))
            {
                selector = this;
                return this;
            }
            else
            {
                selector = null;
                return null;
            }
        }

        #endregion

        #region Static Utils

        internal static IEnumerable<FieldInfo> GetAllSerializableFields(System.Type tp)
        {
            var nonSerializedAttrib = typeof(System.NonSerializedAttribute);
            var serializedAttrib = typeof(UnityEngine.SerializeField);

            while(tp != null)
            {
                foreach(var fi in tp.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (UnityDataFormatter.IsUnitySerializable(fi.FieldType))
                    {
                        if (fi.IsPublic)
                        {
                            if (!System.Attribute.IsDefined(fi, nonSerializedAttrib)) yield return fi;
                        }
                        else
                        {
                            if (System.Attribute.IsDefined(fi, serializedAttrib)) yield return fi;
                        }
                    }
                }
                tp = tp.BaseType;
            }
        }

        #endregion

    }

}
