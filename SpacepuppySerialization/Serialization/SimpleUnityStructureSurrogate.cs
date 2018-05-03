using System;

using System.Runtime.Serialization;

namespace com.spacepuppy.Serialization
{

    public class SimpleUnityStructureSurrogate : ISerializationSurrogate, ISurrogateSelector
    {

        #region Fields

        private ISurrogateSelector _nextSelector;

        #endregion

        #region ISurrogateSelector Interface

        public void ChainSelector(ISurrogateSelector selector)
        {
            if (selector == this) throw new System.ArgumentException("Cyclical SurrogateSelector");

            if (_nextSelector == null)
                _nextSelector = selector;
            else
                _nextSelector.ChainSelector(selector);
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _nextSelector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if(SimpleUnityStructureSurrogate.IsSpeciallySerialized(type))
            {
                selector = this;
                return this;
            }
            else if (_nextSelector != null)
            {
                return _nextSelector.GetSurrogate(type, context, out selector);
            }
            else
            {
                selector = null;
                return null;
            }
        }

        #endregion

        #region ISerializationSurrogate Interface

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            SimpleUnityStructureSurrogate.AddValue(obj, info, context);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return SimpleUnityStructureSurrogate.GetValue(obj, info, context);
        }

        #endregion

        #region Static Is Serialized By This Test

        private static Type[] _specialTypes = new Type[]
        {
            typeof(UnityEngine.Vector2),
            typeof(UnityEngine.Vector3),
            typeof(UnityEngine.Vector4),
            typeof(UnityEngine.Quaternion),
            typeof(UnityEngine.Matrix4x4),
            typeof(UnityEngine.Color),
            typeof(UnityEngine.LayerMask)
        };

        public static bool IsSpeciallySerialized(System.Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            
            return Array.IndexOf(_specialTypes, tp) >= 0;
        }

        #endregion

        #region Static Add/Get Values

        internal static bool AddValue(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is UnityEngine.Vector2)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Vector2)obj);
                return true;
            }
            else if (obj is UnityEngine.Vector3)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Vector3)obj);
                return true;
            }
            else if (obj is UnityEngine.Vector4)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Vector4)obj);
                return true;
            }
            else if (obj is UnityEngine.Quaternion)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Quaternion)obj);
                return true;
            }
            else if (obj is UnityEngine.Color)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Color)obj);
                return true;
            }
            else if (obj is UnityEngine.LayerMask)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.LayerMask)obj);
                return true;
            }
            else if (obj is UnityEngine.Matrix4x4)
            {
                SimpleUnityStructureSurrogate.AddValue(info, (UnityEngine.Matrix4x4)obj);
                return true;
            }

            return false;
        }

        internal static object GetValue(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is UnityEngine.Vector2)
            {
                return SimpleUnityStructureSurrogate.GetVector2(info);
            }
            else if (obj is UnityEngine.Vector3)
            {
                return SimpleUnityStructureSurrogate.GetVector3(info);
            }
            else if (obj is UnityEngine.Vector4)
            {
                return SimpleUnityStructureSurrogate.GetVector4(info);
            }
            else if (obj is UnityEngine.Quaternion)
            {
                return SimpleUnityStructureSurrogate.GetQuaternion(info);
            }
            else if (obj is UnityEngine.Color)
            {
                return SimpleUnityStructureSurrogate.GetColor(info);
            }
            else if (obj is UnityEngine.LayerMask)
            {
                return SimpleUnityStructureSurrogate.GetLayerMask(info);
            }
            else if (obj is UnityEngine.Matrix4x4)
            {
                return SimpleUnityStructureSurrogate.GetMatrix4x4(info);
            }

            return null;
        }




        public static void AddValue(SerializationInfo info, UnityEngine.Vector2 value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.Vector3 value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.Vector4 value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
            info.AddValue("w", value.w);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.Quaternion value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("x", value.x);
            info.AddValue("y", value.y);
            info.AddValue("z", value.z);
            info.AddValue("w", value.w);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.Color value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("r", value.r);
            info.AddValue("g", value.g);
            info.AddValue("b", value.b);
            info.AddValue("a", value.a);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.LayerMask value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("mask", value.value);
        }

        public static void AddValue(SerializationInfo info, UnityEngine.Matrix4x4 value)
        {
            if (info == null) throw new System.ArgumentNullException("info");

            info.AddValue("m00", value.m00);
            info.AddValue("m01", value.m01);
            info.AddValue("m02", value.m02);
            info.AddValue("m03", value.m03);
            info.AddValue("m10", value.m10);
            info.AddValue("m11", value.m11);
            info.AddValue("m12", value.m12);
            info.AddValue("m13", value.m13);
            info.AddValue("m20", value.m20);
            info.AddValue("m21", value.m21);
            info.AddValue("m22", value.m22);
            info.AddValue("m23", value.m23);
            info.AddValue("m30", value.m30);
            info.AddValue("m31", value.m31);
            info.AddValue("m32", value.m32);
            info.AddValue("m33", value.m33);
        }





        public static UnityEngine.Vector2 GetVector2(SerializationInfo info)
        {
            return new UnityEngine.Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }

        public static UnityEngine.Vector3 GetVector3(SerializationInfo info)
        {
            return new UnityEngine.Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }

        public static UnityEngine.Vector4 GetVector4(SerializationInfo info)
        {
            return new UnityEngine.Vector4(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"), info.GetSingle("w"));
        }

        public static UnityEngine.Quaternion GetQuaternion(SerializationInfo info)
        {
            return new UnityEngine.Quaternion(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"), info.GetSingle("w"));
        }

        public static UnityEngine.Color GetColor(SerializationInfo info)
        {
            return new UnityEngine.Color(info.GetSingle("r"), info.GetSingle("g"), info.GetSingle("b"), info.GetSingle("a"));
        }

        public static UnityEngine.LayerMask GetLayerMask(SerializationInfo info)
        {
            var m = new UnityEngine.LayerMask();
            m.value = info.GetInt32("mask");
            return m;
        }

        public static UnityEngine.Matrix4x4 GetMatrix4x4(SerializationInfo info)
        {
            var m = new UnityEngine.Matrix4x4();
            m.m00 = info.GetSingle("00");
            m.m01 = info.GetSingle("01");
            m.m02 = info.GetSingle("02");
            m.m03 = info.GetSingle("03");
            m.m10 = info.GetSingle("10");
            m.m11 = info.GetSingle("11");
            m.m12 = info.GetSingle("12");
            m.m13 = info.GetSingle("13");
            m.m20 = info.GetSingle("20");
            m.m21 = info.GetSingle("21");
            m.m22 = info.GetSingle("22");
            m.m23 = info.GetSingle("23");
            m.m30 = info.GetSingle("30");
            m.m31 = info.GetSingle("31");
            m.m32 = info.GetSingle("32");
            m.m33 = info.GetSingle("33");
            return m;
        }
        
        #endregion

    }

}
