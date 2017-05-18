using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    [System.Serializable]
    public class TextRef : ITextSource
    {
        
        #region Fields

        [SerializeField]
        private string[] _text;
        [SerializeField]
        private UnityEngine.Object _obj;

        #endregion

        #region Properties

        public UnityEngine.Object Source
        {
            get { return _obj; }
        }

        #endregion

        #region ITextSource Interface

        public int Count
        {
            get
            {
                if (_obj != null && _obj is ITextSource)
                    return (_obj as ITextSource).Count;
                else if (_obj is TextAsset)
                    return 1;
                else
                    return _text != null ? _text.Length : 0;
            }
        }

        public string text
        {
            get
            {
                if (_obj != null && _obj is ITextSource)
                    return (_obj as ITextSource).text;
                else if (_obj is TextAsset)
                    return (_obj as TextAsset).text;
                else
                    return _text != null && _text.Length > 0 ? _text[0] : null;
            }
        }

        public string this[int index]
        {
            get
            {
                if (_obj != null && _obj is ITextSource)
                    return (_obj as ITextSource)[index];
                else if (_obj is TextAsset)
                {
                    if(index != 0)
                        throw new System.IndexOutOfRangeException("index");
                    return (_obj as TextAsset).text;
                }
                else
                {
                    if(_text == null || _text.Length == 0 || index < 0 || index >= _text.Length)
                        throw new System.IndexOutOfRangeException("index");
                    return _text[index];
                }
            }
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<string> GetEnumerator()
        {
            if (_obj != null && _obj is ITextSource)
                return (_obj as ITextSource).GetEnumerator();
            else if (_obj is TextAsset)
                return FromTextAsset(_obj as TextAsset);
            else if (_text != null && _text.Length > 0)
                return (_text as IEnumerable<string>).GetEnumerator();
            else
                return Enumerable.Empty<string>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        private static IEnumerator<string> FromTextAsset(TextAsset text)
        {
            yield return text.text;
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public bool DisallowFoldout;

            public ConfigAttribute(bool disallowFoldout)
            {
                this.DisallowFoldout = disallowFoldout;
            }

        }

        #endregion

    }
}
