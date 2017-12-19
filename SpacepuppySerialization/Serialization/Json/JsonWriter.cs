using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.spacepuppy.Serialization.Json
{

    internal class JsonWriter : IDisposable
    {

        public enum ObjectState
        {
            None,
            Object,
            Array,
            Property,
            ObjectLineEnd,
            ArrayLineEnd
        }

        #region Fields

        private TextWriter _writer;

        private Stack<ObjectState> _stack = new Stack<ObjectState>();
        private int _indent = 0;

        #endregion

        #region CONSTRUCTOR

        public JsonWriter()
        {

        }

        public JsonWriter(TextWriter writer)
        {
            this.Init(writer);
        }

        #endregion

        #region Properties

        public ObjectState State
        {
            get
            {
                return (_stack.Count > 0) ? _stack.Peek() : ObjectState.None;
            }
        }

        #endregion

        #region Methods

        public void Init(TextWriter writer)
        {
            _writer = writer;
            _stack.Clear();
            _indent = 0;
        }

        public void Clear()
        {
            _writer = null;
            _stack.Clear();
            _indent = 0;
        }


        public void WriteStartObject()
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.Object:
                    throw new JsonException("Can not nest json object in json object.");
                case ObjectState.ObjectLineEnd:
                    throw new JsonException("Objects must be written after a property or as a member of an array.");
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write("{\n");

            _stack.Push(ObjectState.Object);
            _indent++;
            this.AdjustForIndent();
        }

        public void WriteEndObject()
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Array:
                case ObjectState.ArrayLineEnd:
                    throw new JsonException("Can not end object that has not been started.");
                case ObjectState.Property:
                    throw new JsonException("Value expected to match a property.");
                case ObjectState.ObjectLineEnd:
                    _writer.Write("\n");
                    _indent--;
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write("}");
            _stack.Pop();
            if (this.State == ObjectState.Property) _stack.Pop();
            this.ValidateLineEnd();
        }

        public void WriteStartArray()
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");
            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                case ObjectState.ObjectLineEnd:
                    throw new JsonException("Arrays must be written after a property or as a member of an array.");
                case ObjectState.Array:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write("[\n");

            _stack.Push(ObjectState.Array);
            _indent++;
            this.AdjustForIndent();
        }

        public void WriteEndArray()
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                case ObjectState.ObjectLineEnd:
                    throw new JsonException("Can not end array that has not been started.");
                case ObjectState.Property:
                    throw new JsonException("Value expected to match a property.");
                case ObjectState.ArrayLineEnd:
                    _writer.Write("\n");
                    _indent--;
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write("]");
            _stack.Pop();
            if (this.State == ObjectState.Property) _stack.Pop();
            this.ValidateLineEnd();
        }

        public void WritePropertyName(string name, bool escape = false)
        {
            if (name == null) throw new System.ArgumentNullException("name");
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Array:
                case ObjectState.Property:
                case ObjectState.ArrayLineEnd:
                    throw new JsonException("Can not write a property name if no object was started.");
                case ObjectState.ObjectLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write('"');
            if (escape)
                _writer.Write(JsonEscape(name));
            else
                _writer.Write(name);
            _writer.Write('"');

            _writer.Write(" : ");

            _stack.Push(ObjectState.Property);
        }

        public void WriteValue(string value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                    throw new JsonException("Values must be written after a property or as a member of an array.");
                case ObjectState.Property:
                    _stack.Pop();
                    break;
                case ObjectState.ObjectLineEnd:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            if (value == null)
            {
                _writer.Write("null");
            }
            else
            {
                _writer.Write('"');
                _writer.Write(JsonEscape(value));
                _writer.Write('"');
            }

            this.ValidateLineEnd();
        }

        public void WriteValue(bool value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                    throw new JsonException("Values must be written after a property or as a member of an array.");
                case ObjectState.Property:
                    _stack.Pop();
                    break;
                case ObjectState.ObjectLineEnd:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write(value ? "true" : "false");

            this.ValidateLineEnd();
        }

        public void WriteValue(int value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                    throw new JsonException("Values must be written after a property or as a member of an array.");
                case ObjectState.Property:
                    _stack.Pop();
                    break;
                case ObjectState.ObjectLineEnd:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write(value.ToString());

            this.ValidateLineEnd();
        }

        public void WriteValue(float value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                    throw new JsonException("Values must be written after a property or as a member of an array.");
                case ObjectState.Property:
                    _stack.Pop();
                    break;
                case ObjectState.ObjectLineEnd:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write(value.ToString());

            this.ValidateLineEnd();
        }

        public void WriteValue(double value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            switch (this.State)
            {
                case ObjectState.None:
                case ObjectState.Object:
                    throw new JsonException("Values must be written after a property or as a member of an array.");
                case ObjectState.Property:
                    _stack.Pop();
                    break;
                case ObjectState.ObjectLineEnd:
                case ObjectState.ArrayLineEnd:
                    _writer.Write(",\n");
                    this.AdjustForIndent();
                    _stack.Pop();
                    break;
            }

            _writer.Write(value.ToString());

            this.ValidateLineEnd();
        }
        
        public void WriteValue(object value)
        {
            if (_writer == null) throw new System.InvalidOperationException("Must initialize JsonWriter before writing.");

            if (value == null)
            {
                switch (this.State)
                {
                    case ObjectState.None:
                    case ObjectState.Object:
                        throw new JsonException("Values must be written after a property or as a member of an array.");
                    case ObjectState.Property:
                        _stack.Pop();
                        break;
                    case ObjectState.ObjectLineEnd:
                    case ObjectState.ArrayLineEnd:
                        _writer.Write(",\n");
                        this.AdjustForIndent();
                        _stack.Pop();
                        break;
                }

                _writer.Write("null");

                this.ValidateLineEnd();
            }
            else if (value is string)
                this.WriteValue(value as string);
            else if (value is bool)
                this.WriteValue((bool)value);
            else if (IsNumericType(value))
            {
                if (value is int)
                    this.WriteValue((int)value);
                else if (value is float)
                    this.WriteValue((float)value);
                else if (value is double)
                    this.WriteValue((double)value);
                else
                    this.WriteValue(System.Convert.ToDouble(value));
            }
            else
            {
                throw new JsonException("A value can not be of type " + value.GetType().ToString());
            }
        }

        #endregion

        #region Utils

        private void AdjustForIndent()
        {
            for (int i = 0; i < _indent; i++)
            {
                _writer.Write('\t');
            }
        }

        private void ValidateLineEnd()
        {
            switch (this.State)
            {
                case ObjectState.None:
                    break;
                case ObjectState.Object:
                    _stack.Push(ObjectState.ObjectLineEnd);
                    break;
                case ObjectState.Array:
                    _stack.Push(ObjectState.ArrayLineEnd);
                    break;
                default:
                    throw new JsonException("Json order is malformed."); //this should never be reached
            }
        }

        private static string JsonEscape(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        private static bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            _writer = null;
            _stack.Clear();
            _indent = 0;
        }

        #endregion

    }

}
