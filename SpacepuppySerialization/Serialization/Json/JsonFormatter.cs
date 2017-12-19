using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Serialization.Json
{

    public class JsonFormatter : IFormatter
    {

        private const string ID_TYPE = "@type";

        #region Fields

        private JsonReader _reader;
        private JsonWriter _writer;

        #endregion

        #region IFormatter Interface

        public SerializationBinder Binder
        {
            get;
            set;
        }

        public StreamingContext Context
        {
            get;
            set;
        }

        public ISurrogateSelector SurrogateSelector
        {
            get;
            set;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            if (serializationStream == null) throw new System.ArgumentNullException("serializationStream");
            if (graph == null) throw new System.ArgumentNullException("graph");

            if (_writer == null) _writer = new JsonWriter();

            var writer = new StreamWriter(serializationStream);

            try
            {
                _writer.Init(writer);

                this.WriteObject(graph);
            }
            catch (SerializationException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new SerializationException("Object graph is malformed.", ex);
            }
            finally
            {
                writer.Flush();
                _writer.Clear();
            }
        }

        private void WriteObject(object graph)
        {
            _writer.WriteStartObject();
            _writer.WritePropertyName(ID_TYPE);
            _writer.WriteValue(graph.GetType().FullName);

            var members = FormatterServices.GetSerializableMembers(graph.GetType(), Context);
            var objs = FormatterServices.GetObjectData(graph, members);

            for (int i = 0; i < objs.Length; i++)
            {
                _writer.WritePropertyName(members[i].Name);
                if (objs[i] == null)
                {
                    _writer.WriteValue(null);
                    continue;
                }
                else
                {
                    this.WriteValue(objs[i]);
                }
            }

            _writer.WriteEndObject();
        }

        private void WriteValue(object value)
        {
            if (value == null)
            {
                _writer.WriteValue(null);
                return;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    _writer.WriteValue(null);
                    break;
                case TypeCode.Object:
                    var tp = value.GetType();
                    if (tp.IsArray || (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        _writer.WriteStartArray();
                        _writer.WriteValue(tp.FullName);

                        var arr = value as System.Collections.IList;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            this.WriteValue(arr[i]);
                        }

                        _writer.WriteEndArray();
                    }
                    else
                    {
                        this.WriteObject(value);
                    }
                    break;
                case TypeCode.Boolean:
                    _writer.WriteValue(Convert.ToBoolean(value));
                    break;
                case TypeCode.Char:
                    _writer.WriteValue(Convert.ToString(value));
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    _writer.WriteValue(Convert.ToInt32(value));
                    break;
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    _writer.WriteValue(Convert.ToDouble(value));
                    break;
                case TypeCode.Decimal:
                    _writer.WriteValue(Convert.ToDouble(value));
                    break;
                case TypeCode.DateTime:
                    //TODO - add support
                    break;
                case TypeCode.String:
                    _writer.WriteValue(Convert.ToString(value));
                    break;
            }
        }



        public object Deserialize(Stream serializationStream)
        {
            if (serializationStream == null) throw new System.ArgumentNullException("serializationStream");

            if (_reader == null) _reader = new JsonReader();

            var reader = new StreamReader(serializationStream);

            try
            {
                _reader.Init(reader);

                if (!_reader.Read()) return null;
                if (_reader.NodeType != JsonNodeType.Object) throw new SerializationException("Failed to deserialize due to malformed json: json must start with a json object.");

                return this.ReadObject();
            }
            finally
            {
                _reader.Clear();
            }
        }

        private object ReadObject()
        {
            if (_reader.NodeType != JsonNodeType.Object) throw new SerializationException("Failed to deserialize due to malformed json.");

            _reader.Read();
            if (_reader.NodeType != JsonNodeType.String || _reader.Name != ID_TYPE) throw new SerializationException("Failed to deserialize due to malformed json: objects must contain a @type property.");

            var tp = Type.GetType(_reader.Value as string);
            if (tp == null) tp = TypeUtil.FindType(_reader.Value as string, true);
            if (tp == null) throw new SerializationException("Failed to deserialize due to malformed json: objects must contain a @type property.");
            var members = FormatterServices.GetSerializableMembers(tp, Context);
            var data = new object[members.Length];

            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case JsonNodeType.None:
                    case JsonNodeType.EndArray:
                        throw new SerializationException("Failed to deserialize due to malformed json.");
                    case JsonNodeType.Object:
                        {
                            var nm = _reader.Name;
                            int i = GetIndexOfMemberName(members, nm);
                            if (i < 0) throw new SerializationException("Failed to deserialize due to malformed json. (" + nm + ")");
                            data[i] = this.ReadObject();
                        }
                        break;
                    case JsonNodeType.Array:
                        {
                            var nm = _reader.Name;
                            int i = GetIndexOfMemberName(members, nm);
                            if (i < 0) throw new SerializationException("Failed to deserialize due to malformed json. (" + nm + ")");

                            _reader.Read();
                            if (_reader.NodeType != JsonNodeType.String) throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");

                            System.Type arrayType;
                            try
                            {
                                var stp = _reader.Value as string;
                                bool isArray = (stp != null && stp.EndsWith("[]"));
                                if (isArray) stp = stp.Substring(0, stp.Length - 2);
                                arrayType = Type.GetType(stp);
                                if (arrayType == null) arrayType = TypeUtil.FindType(stp, true);
                                if (arrayType != null && isArray) arrayType = arrayType.MakeArrayType();
                            }
                            catch (System.Exception)
                            {
                                throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");
                            }
                            if(arrayType == null) throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");

                            var innerType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
                            data[i] = this.ReadArray(nm, innerType, !arrayType.IsArray);
                        }
                        break;
                    case JsonNodeType.String:
                    case JsonNodeType.Number:
                    case JsonNodeType.Boolean:
                    case JsonNodeType.Null:
                        {
                            int i = GetIndexOfMemberName(members, _reader.Name);
                            if (i < 0) throw new SerializationException("Failed to deserialize due to malformed json. (" + _reader.Name + ")");
                            data[i] = ConvertJsonValueToType(_reader.Value, (members[i] as System.Reflection.FieldInfo).FieldType);
                        }
                        break;
                    case JsonNodeType.EndObject:
                        goto Result;
                }
            }

            Result:
            var result = FormatterServices.GetUninitializedObject(tp);
            return FormatterServices.PopulateObjectMembers(result, members, data);
        }

        private object ReadArray(string nm, System.Type innerType, bool keepAsList)
        {
            var ltp = typeof(List<>);
            ltp = ltp.MakeGenericType(innerType);
            var lst = Activator.CreateInstance(ltp) as System.Collections.IList;
            if (lst == null) throw new SerializationException("Failed to deserialize due to malformed json.");

            while (_reader.Read())
            {
                switch (_reader.NodeType)
                {
                    case JsonNodeType.None:
                    case JsonNodeType.EndObject:
                        throw new SerializationException("Failed to deserialize due to malformed json.");
                    case JsonNodeType.Object:
                        {
                            lst.Add(ConvertJsonValueToType(this.ReadObject(), innerType));
                        }
                        break;
                    case JsonNodeType.Array:
                        {
                            _reader.Read();
                            if (_reader.NodeType != JsonNodeType.String) throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");

                            System.Type arrayType;
                            try
                            {
                                var stp = _reader.Value as string;
                                bool isArray = (stp != null && stp.EndsWith("[]"));
                                if (isArray) stp = stp.Substring(0, stp.Length - 2);
                                arrayType = Type.GetType(stp);
                                if (arrayType == null) arrayType = TypeUtil.FindType(stp, true);
                                if (arrayType != null && isArray) arrayType = arrayType.MakeArrayType();
                            }
                            catch (System.Exception)
                            {
                                throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");
                            }
                            if (arrayType == null) throw new SerializationException("Failed to deserialize due to malformed json: array must begin with a @type string. (" + nm + ")");

                            var nextInnerType = arrayType.IsArray ? arrayType.GetElementType() : arrayType.GetGenericArguments()[0];
                            lst.Add(this.ReadArray("", nextInnerType, !arrayType.IsArray));
                        }
                        break;
                    case JsonNodeType.String:
                    case JsonNodeType.Number:
                    case JsonNodeType.Boolean:
                    case JsonNodeType.Null:
                        {
                            var obj = ConvertJsonValueToType(_reader.Value, innerType);
                            lst.Add(obj);
                        }
                        break;
                    case JsonNodeType.EndArray:
                        goto Result;
                }
            }

            Result:
            if (keepAsList)
                return lst;
            else
            {
                var arr = Array.CreateInstance(innerType, lst.Count);
                lst.CopyTo(arr, 0);
                return arr;
            }
        }

        #endregion

        #region Utils

        private static int GetIndexOfMemberName(System.Reflection.MemberInfo[] members, string name)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null && members[i].Name == name) return i;
            }
            return -1;
        }

        private static object ConvertJsonValueToType(object value, System.Type tp)
        {
            if (value == null) return null;

            switch (Type.GetTypeCode(tp))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return null;
                case TypeCode.Object:
                    {
                        if (tp.IsAssignableFrom(value.GetType()))
                            return value;
                        else
                            return null;
                    }
                case TypeCode.Boolean:
                    return (value is bool) ? value : false;
                case TypeCode.Char:
                    return Convert.ToChar(value);
                case TypeCode.SByte:
                    return Convert.ToSByte(value);
                case TypeCode.Byte:
                    return Convert.ToByte(value);
                case TypeCode.Int16:
                    return Convert.ToInt16(value);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(value);
                case TypeCode.Int32:
                    return Convert.ToInt32(value);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(value);
                case TypeCode.Int64:
                    return Convert.ToInt64(value);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value);
                case TypeCode.Single:
                    return Convert.ToSingle(value);
                case TypeCode.Double:
                    return Convert.ToDouble(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);
                case TypeCode.DateTime:
                    return Convert.ToDateTime(value);
                case TypeCode.String:
                    return Convert.ToString(value);
            }

            return null;
        }

        #endregion

    }

}
