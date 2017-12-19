using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.spacepuppy.Serialization.Json
{

    internal class JsonReader : System.IDisposable
    {

        private enum ObjectState : byte
        {
            None,
            Object,
            Array
        }

        #region Fields

        private TextReader _reader;
        private JsonNodeType _nodeType;

        private string _name;
        private object _value;

        private Stack<ObjectState> _stack = new Stack<ObjectState>();
        private bool _lastWasComma;
        private StringBuilder _builder = new StringBuilder();

        #endregion

        #region CONSTRUCTOR

        public JsonReader()
        {

        }

        public JsonReader(string value)
        {
            this.Init(value);
        }

        public JsonReader(TextReader reader)
        {
            this.Init(reader);
        }

        #endregion

        #region Properties

        public JsonNodeType NodeType
        {
            get { return _nodeType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
        }

        #endregion

        #region Methods

        public void Init(string value)
        {
            _reader = new StringReader(value);
            _nodeType = JsonNodeType.None;
            _name = string.Empty;
            _value = null;
            _stack.Clear();
            _lastWasComma = false;
            _builder.Length = 0;
        }

        public void Init(TextReader reader)
        {
            _reader = reader;
            _nodeType = JsonNodeType.None;
            _name = string.Empty;
            _value = null;
            _stack.Clear();
            _lastWasComma = false;
            _builder.Length = 0;
        }

        public void Clear()
        {
            _reader = null;
            _nodeType = JsonNodeType.None;
            _name = string.Empty;
            _value = null;
            _stack.Clear();
            _lastWasComma = false;
            _builder.Length = 0;
        }




        public bool Read()
        {
            _nodeType = JsonNodeType.None;
            _name = string.Empty;
            _value = null;
            if (_reader == null) return false;

            if (_stack.Count == 0)
            {
                if (this.PeekToNext() < 0)
                {
                    _reader = null;
                    _nodeType = JsonNodeType.None;
                    _lastWasComma = false;
                    _builder.Length = 0;
                    return false;
                }
                else
                {
                    _lastWasComma = true;
                    return this.ReadNextEntry();
                }
            }
            else if (_lastWasComma)
            {
                switch (_stack.Peek())
                {
                    case ObjectState.Object:
                        return this.ReadNextPair();
                    case ObjectState.Array:
                        return this.ReadNextEntry();
                    default:
                        throw new System.InvalidOperationException("JsonReader entered an invalid state.");
                }
            }
            else
            {
                this.ReadEntryEnd();
                return true;
            }
        }

        private bool ReadNextEntry()
        {
            int i = this.PeekToNext();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            char c = (char)_reader.Read();

            switch (c)
            {
                case '{':
                    {
                        _lastWasComma = true;
                        _nodeType = JsonNodeType.Object;
                        _stack.Push(ObjectState.Object);
                        return true;
                    }
                case '[':
                    {
                        _lastWasComma = true;
                        _nodeType = JsonNodeType.Array;
                        _stack.Push(ObjectState.Array);
                        return true;
                    }
                case '"':
                    {
                        //string
                        _value = this.ReadString(c);
                        _nodeType = JsonNodeType.String;
                        this.ValidateComma();
                        return true;
                    }
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        _value = this.ReadNumber(c);
                        _nodeType = JsonNodeType.Number;
                        this.ValidateComma();
                        return true;
                    }
                case 't':
                case 'T':
                case 'f':
                case 'F':
                    {
                        //bool
                        _value = this.ReadBool(c);
                        _nodeType = JsonNodeType.Boolean;
                        this.ValidateComma();
                        return true;
                    }
                case 'n':
                case 'N':
                    {
                        //null
                        _value = this.ReadNull(c);
                        _nodeType = JsonNodeType.Null;
                        this.ValidateComma();
                        return true;
                    }
                default:
                    {
                        throw new JsonException("Syntax error, failed to parse Json.");
                    }
            }
        }

        private bool ReadNextPair()
        {
            int i = this.PeekToNext();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            char c = (char)_reader.Read();

            if (c != '"') throw new JsonException("Syntax error, failed to parse Json.");

            //read name
            _name = this.ReadString(c);

            i = this.PeekToNext();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            c = (char)_reader.Read();

            if (c != ':') throw new JsonException("Syntax error, failed to parse Json.");

            return this.ReadNextEntry();
        }

        private string ReadString(char c)
        {
            if (c != '"') throw new JsonException("Syntax error, failed to parse Json.");

            _builder.Length = 0;
            while (true)
            {
                int i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");

                c = (char)i;
                switch (c)
                {
                    case '"':
                        {
                            var result = _builder.ToString();
                            _builder.Length = 0;
                            return result;
                        }
                    case '\\':
                        {
                            //escaped char
                            i = _reader.Read();
                            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");

                            c = (char)i;
                            switch (c)
                            {
                                case '\\':
                                case '"':
                                case '/':
                                    _builder.Append(c);
                                    break;
                                case 'b':
                                    _builder.Append('\b');
                                    break;
                                case 't':
                                    _builder.Append('\t');
                                    break;
                                case 'n':
                                    _builder.Append('\n');
                                    break;
                                case 'f':
                                    _builder.Append('\f');
                                    break;
                                case 'r':
                                    _builder.Append('\r');
                                    break;
                                case 'u':
                                    int hex = 0;
                                    for (int j = 0; j < 4; j++)
                                    {
                                        i = _reader.Read();
                                        if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");

                                        c = (char)i;
                                        if (char.IsNumber(c))
                                            hex |= (i - 30);
                                        else if (char.IsLetter(c))
                                            hex |= ((int)char.ToUpper(c) - 31);
                                        else
                                            throw new JsonException("Syntax error, failed to parse Json.");

                                        hex = hex << 4;
                                    }
                                    _builder.Append((char)hex);
                                    break;
                            }
                        }
                        break;
                    default:
                        {
                            _builder.Append(c);
                        }
                        break;
                }
            }
        }

        private double ReadNumber(char c)
        {
            bool dotFound = false;
            bool eFound = false;

            _builder.Length = 0;
            _builder.Append(c);
            while (true)
            {
                switch (_reader.Peek())
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        c = (char)_reader.Read();
                        _builder.Append(c);
                        break;
                    case '.':
                        if (dotFound) throw new JsonException("Malformed numeric value.");
                        dotFound = true;
                        c = (char)_reader.Read();
                        _builder.Append(c);
                        break;
                    case 'e':
                    case 'E':
                        if (eFound) throw new JsonException("Malformed numeric value.");
                        eFound = true;
                        dotFound = true;
                        c = (char)_reader.Read();
                        _builder.Append(char.ToLower(c));
                        switch ((char)_reader.Peek())
                        {
                            case '+':
                            case '-':
                                c = (char)_reader.Read();
                                _builder.Append(c);
                                break;
                        }
                        break;
                    case ',':
                    case '}':
                    case ']':
                    case ' ':
                    case '\t':
                    case '\n':
                        goto done;
                    default:
                        throw new JsonException("Malformed numeric value.");
                }
            }

            done:
            double value;
            if (double.TryParse(_builder.ToString(), out value))
                return value;
            else
                return 0d;
        }

        private bool ReadBool(char c)
        {
            c = char.ToLower(c);
            if (c == 't')
            {
                int i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'r') throw new JsonException("Syntax error, failed to parse Json.");

                i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'u') throw new JsonException("Syntax error, failed to parse Json.");

                i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'e') throw new JsonException("Syntax error, failed to parse Json.");

                return true;
            }
            else if (c == 'f')
            {
                int i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'a') throw new JsonException("Syntax error, failed to parse Json.");

                i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'l') throw new JsonException("Syntax error, failed to parse Json.");

                i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 's') throw new JsonException("Syntax error, failed to parse Json.");

                i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
                c = char.ToLower((char)i);
                if (c != 'e') throw new JsonException("Syntax error, failed to parse Json.");

                return false;
            }
            else
            {
                throw new JsonException("Syntax error, failed to parse Json.");
            }
        }

        private object ReadNull(char c)
        {
            c = char.ToLower(c);
            if (c != 'n') throw new JsonException("Syntax error, failed to parse Json.");

            int i = _reader.Read();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            c = char.ToLower((char)i);
            if (c != 'u') throw new JsonException("Syntax error, failed to parse Json.");

            i = _reader.Read();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            c = char.ToLower((char)i);
            if (c != 'l') throw new JsonException("Syntax error, failed to parse Json.");

            i = _reader.Read();
            if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");
            c = char.ToLower((char)i);
            if (c != 'l') throw new JsonException("Syntax error, failed to parse Json.");

            return null;
        }




        private void ReadEntryEnd()
        {
            char c;
            while (true)
            {
                int i = _reader.Read();
                if (i < 0) throw new JsonException("Syntax error, failed to parse Json.");

                if (!char.IsWhiteSpace((char)i))
                {
                    c = (char)i;
                    break;
                }
            }

            switch (c)
            {
                case '}':
                    {
                        if (_stack.Count == 0 || _stack.Peek() != ObjectState.Object)
                        {
                            throw new JsonException("Syntax error, failed to parse Json.");
                        }
                        _stack.Pop();

                        _nodeType = JsonNodeType.EndObject;
                        this.ValidateComma();
                    }
                    break;
                case ']':
                    {
                        if (_stack.Count == 0 || _stack.Peek() != ObjectState.Array)
                        {
                            throw new JsonException("Syntax error, failed to parse Json.");
                        }
                        _stack.Pop();

                        _nodeType = JsonNodeType.EndArray;
                        this.ValidateComma();
                    }
                    break;
                default:
                    {
                        throw new JsonException("Syntax error, failed to parse Json.");
                    }
            }
        }

        private void ValidateComma()
        {
            const int CHAR_COMMA = (int)',';
            if (this.PeekToNext() == CHAR_COMMA)
            {
                _reader.Read();
                _lastWasComma = true;
            }
            else
            {
                _lastWasComma = false;
            }
        }


        private int PeekToNext()
        {
            while (true)
            {
                int i = _reader.Peek();
                if (i < 0) return i;

                if (char.IsWhiteSpace((char)i))
                {
                    _reader.Read();
                }
                else
                {
                    return i;
                }
            }
        }

        #endregion



        #region IDisposable Interface

        public void Dispose()
        {
            _reader = null;
            _nodeType = JsonNodeType.None;
            _name = string.Empty;
            _value = null;
            _stack.Clear();
            _lastWasComma = false;
            _builder.Length = 0;
        }

        #endregion

    }

}
