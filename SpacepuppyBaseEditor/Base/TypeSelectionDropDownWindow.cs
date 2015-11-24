using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base
{
    public class TypeSelectionDropDownWindow : EditorWindow
    {

        private const string PREF_SEARCHSTRING = "TypeDropDownSearchHistory";
        private static int s_TypePopupHash = "TypeEditorPopup".GetHashCode();
        private static int s_TypeCustomHash = "TypeEditorCustomDropDown".GetHashCode();
        private const string CNTRL_TYPESEARCH = "TypeSearch";

        #region Fields

        private Styles _style = new Styles();

        private System.Type _baseType = typeof(object);
        private System.Type _defaultType = null;
        private bool _allowAbstractTypes;
        private bool _allowInterfaces;
        private System.Type[] _excludedTypes;
        private TypeDropDownListingStyle _listStyle;


        private string _header;
        private string _search = string.Empty;
        private string _lastSearch = string.Empty;
        private List<SearchElement> _searchElements = new List<SearchElement>();
        private bool _searchIsDirty;

        private Vector2 _scrollPosition;
        private int _selectedIndex = -1;
        private bool _scrollToSelected;

        #endregion

        #region Properties

        public System.Type BaseType
        {
            get { return _baseType; }
            set
            {
                if (_baseType == value) return;
                _baseType = value ?? typeof(object);
                _searchIsDirty = true;
            }
        }

        public System.Type DefaultType
        {
            get { return _defaultType; }
            set
            {
                if (value != null && TypeUtil.IsType(value, _baseType))
                {
                    if (_defaultType == value) return;
                    _defaultType = value;
                    _searchIsDirty = true;
                }
                else
                {
                    if (_defaultType == null) return;
                    _defaultType = null;
                    _searchIsDirty = true;
                }

            }
        }

        public bool AllowAbstractTypes
        {
            get { return _allowAbstractTypes; }
            set
            {
                if (_allowAbstractTypes == value) return;
                _allowAbstractTypes = value;
                _searchIsDirty = true;
            }
        }

        public bool AllowInterface
        {
            get { return _allowInterfaces; }
            set
            {
                if (_allowInterfaces == value) return;
                _allowInterfaces = value;
                _searchIsDirty = true;
            }
        }

        public System.Type[] ExcludedTypes
        {
            get { return _excludedTypes; }
            set
            {
                _excludedTypes = value;
                _searchIsDirty = true;
            }
        }

        public TypeDropDownListingStyle ListingStyle
        {
            get { return _listStyle; }
            set
            {
                if (_listStyle == value) return;
                _listStyle = value;
                _searchIsDirty = true;
            }
        }

        #endregion

        #region Private Redirect Properties

        private SearchElement _activeElement
        {
            get
            {
                if (_selectedIndex < 0 || _selectedIndex >= _searchElements.Count) return null;
                return _searchElements[_selectedIndex];
            }
        }

        #endregion

        #region Methods

        protected virtual void OnEnable()
        {
            _search = EditorPrefs.GetString(PREF_SEARCHSTRING, string.Empty);
            this.wantsMouseMove = true;
        }

        protected virtual void OnDestroy()
        {
            if (object.ReferenceEquals(this, _window)) _window = null;
        }

        protected virtual void OnGUI()
        {
            this.HandleKeyboard();

            GUI.Label(new Rect(0.0f, 0.0f, this.position.width, this.position.height), GUIContent.none, _style.background);
            GUILayout.Space(7f);
            EditorGUI.FocusTextInControl(CNTRL_TYPESEARCH);

            //first draw search area and update search if needed
            var rect = GUILayoutUtility.GetRect(10f, 20f);
            rect.x += 8f;
            rect.width -= 16f;

            GUI.SetNextControlName(CNTRL_TYPESEARCH);
            _search = SPEditorGUI.SearchField(rect, _search);
            if(_search != _lastSearch)
            {
                this.RebuildSearch();
            }
            else if(_searchIsDirty)
            {
                this.RebuildSearch();
            }


            //next draw type header (display the name of the base type)
            rect = GUILayoutUtility.GetRect(10f, 25f);
            EditorGUI.LabelField(rect, _header, _style.header);

            //lastly list found types
            this.DrawList();
        }

        private void DrawList()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            Event current = Event.current;
            Rect selectedRect = new Rect();
            EditorGUI.indentLevel++;
            for(int index = 0; index < _searchElements.Count; index++)
            {
                var e = _searchElements[index];
                Rect rect = GUILayoutUtility.GetRect(16f, 20f, new GUILayoutOption[1]
                {
                    GUILayout.ExpandWidth(true)
                });
                rect.x += 2f;
                rect.width -= 2f;

                if ((current.type == EventType.MouseMove || current.type == EventType.mouseDown) && rect.Contains(current.mousePosition))
                {
                    _selectedIndex = index;
                    this.Repaint();
                }
                bool selectedFlag = false;
                if(index == _selectedIndex)
                {
                    selectedFlag = true;
                    selectedRect = rect;
                }
                if (current.type == EventType.Repaint)
                {
                    _style.componentButton.Draw(rect, e.Content, false, false, selectedFlag, selectedFlag);
                }
                if (current.type == EventType.MouseUp && rect.Contains(current.mousePosition))
                {
                    current.Use();
                    _selectedIndex = index;
                    this.SelectElement(e);
                }
            }
            EditorGUI.indentLevel--;
            
            GUILayout.EndScrollView();



            if (!_scrollToSelected || Event.current.type != EventType.Repaint)
                return;
            _scrollToSelected = false;
            Rect lastRect = GUILayoutUtility.GetLastRect();
            if ((double)selectedRect.yMax - (double)lastRect.height > (double)_scrollPosition.y)
            {
                _scrollPosition.y = selectedRect.yMax - lastRect.height;
                this.Repaint();
            }
            if ((double)selectedRect.y >= (double)_scrollPosition.y)
                return;
            _scrollPosition.y = selectedRect.y;
            this.Repaint();
        }

        private void HandleKeyboard()
        {
            Event current = Event.current;
            if (current.type != EventType.KeyDown)
                return;

            switch(current.keyCode)
            {
                case KeyCode.DownArrow:
                    ++_selectedIndex;
                    _selectedIndex = Mathf.Min(_selectedIndex, _searchElements.Count - 1);
                    this._scrollToSelected = true;
                    current.Use();
                    break;
                case KeyCode.UpArrow:
                    --_selectedIndex;
                    _selectedIndex = Mathf.Max(_selectedIndex, 0);
                    this._scrollToSelected = true;
                    current.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    this.SelectElement(_activeElement);
                    current.Use();
                    break;
                case KeyCode.LeftArrow:
                case KeyCode.Backspace:
                    //if (this.hasSearch) return;
                    //this.GoToParent();
                    //current.Use();
                    break;
                case KeyCode.RightArrow:
                    //if (this.hasSearch) return;
                    //this.GoToChild(this.activeElement, false);
                    //current.Use();
                    break;
                case KeyCode.Escape:
                    this.Close();
                    current.Use();
                    break;
            }
        }

        private void SelectElement(SearchElement el)
        {
            this.Close();
            if (this == _window && el != null)
            {
                if (CallbackInfo.instance != null)
                {
                    CallbackInfo.instance.SignalChange(el.Type);
                }

                _window = null;
            }
        }











        private void RebuildSearch()
        {
            _lastSearch = _search;
            _searchIsDirty = false;
            _scrollPosition = Vector2.zero;
            _selectedIndex = -1;
            EditorPrefs.SetString(PREF_SEARCHSTRING, _search);

            if (string.IsNullOrEmpty(_search))
            {
                _searchElements.Clear();
                if (_defaultType == null)
                    _searchElements.Add(new SearchElement()
                    {
                        Content = GetTypeLabel(null)
                    });
                _searchElements.AddRange(from tp in TypeUtil.GetTypes(this.TestIfValidType)
                                         select new SearchElement()
                                         {
                                             Name = tp.Name,
                                             Type = tp,
                                             Content = GetTypeLabel(tp)
                                         });



                _header = (_baseType == typeof(object)) ? "All Types" : _baseType.Name + "/s";
            }
            else
            {
                var match = _search.ToLower();
                _searchElements.Clear();
                if (_defaultType == null)
                    _searchElements.Add(new SearchElement()
                    {
                        Content = GetTypeLabel(null)
                    });
                _searchElements.AddRange(from tp in TypeUtil.GetTypes(this.TestIfValidType)
                                         where tp.Name.ToLower().Contains(match)
                                         select new SearchElement()
                                         {
                                             Name = tp.Name,
                                             Type = tp,
                                             Content = GetTypeLabel(tp)
                                         });

                _header = (_baseType == typeof(object)) ? "Search All Types" : "Search " + _baseType.Name + "/s";
            }
        }

        private bool TestIfValidType(System.Type tp)
        {
            if (tp == null) return false;

            if (!_baseType.IsAssignableFrom(tp)) return false;
            if (tp.IsInterface && !_allowInterfaces) return false;
            if (tp.IsAbstract && !_allowAbstractTypes) return false;
            if (_excludedTypes != null && _excludedTypes.IndexOf(tp) >= 0) return false;

            return true;
        }

        #endregion

        #region Static Utils

        private static GUIContent GetTypeLabel(System.Type tp)
        {
            if (tp == null)
                return new GUIContent("Nothing...");
            else
                return new GUIContent(tp.Name, tp.FullName);
        }

        #endregion

        #region Special Types

        private class SearchElement
        {

            public string Name;
            public System.Type Type;
            public GUIContent Content;

        }

        private class Styles
        {
            public GUIStyle header;
            public GUIStyle componentButton = new GUIStyle((GUIStyle)"PR Label");
            public GUIStyle background = (GUIStyle)"grey_border";
            public GUIStyle previewBackground = (GUIStyle)"PopupCurveSwatchBackground";
            public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);
            public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);
            public GUIStyle rightArrow = (GUIStyle)"AC RightArrow";
            public GUIStyle leftArrow = (GUIStyle)"AC LeftArrow";
            public GUIStyle groupButton;

            public Styles()
            {
                header = SPEditorStyles.GetStyle("In BigTitle");
                this.header.font = EditorStyles.boldLabel.font;
                this.componentButton.alignment = TextAnchor.MiddleLeft;
                this.componentButton.padding.left -= 15;
                this.componentButton.fixedHeight = 20f;
                this.groupButton = new GUIStyle(this.componentButton);
                this.groupButton.padding.left += 17;
                this.previewText.padding.left += 3;
                this.previewText.padding.right += 3;
                ++this.previewHeader.padding.left;
                this.previewHeader.padding.right += 3;
                this.previewHeader.padding.top += 3;
                this.previewHeader.padding.bottom += 2;
            }
        }

        #endregion

        #region Static Inspector Show Methods

        private static TypeSelectionDropDownWindow _window;

        public static System.Type Popup(Rect position, GUIContent label,
                                               System.Type baseType, System.Type selectedType,
                                               bool allowAbstractTypes = false, bool allowInterfaces = false,
                                               System.Type defaultType = null, System.Type[] excludedTypes = null,
                                               TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            int controlID = GUIUtility.GetControlID(TypeSelectionDropDownWindow.s_TypePopupHash, EditorGUIUtility.native, position);
            position = EditorGUI.PrefixLabel(position, controlID, label);

            selectedType = CallbackInfo.GetSelectedValueForControl(controlID, selectedType);

            var content = GetTypeLabel(selectedType);
            var current = Event.current;
            var type = current.type;
            switch(type)
            {
                case EventType.KeyDown:
                    {
                        //TODO?
                        //EditorStyles.popup.Draw(position, content, controlID);
                    }
                    break;
                case EventType.Repaint:
                    {
                        EditorStyles.popup.Draw(position, content, controlID);
                    }
                    break;
                case EventType.MouseDown:
                    {
                        if(current.button == 0 && position.Contains(current.mousePosition))
                        {
                            CallbackInfo.instance = new CallbackInfo(controlID, selectedType);
                            TypeSelectionDropDownWindow.DisplayCustomMenu(position, label, baseType, selectedType, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, listType);
                            GUIUtility.keyboardControl = controlID;
                            current.Use();
                        }
                    }
                    break;
            }

            return selectedType;
        }

        public static void ShowAndCallbackOnSelect(Rect positionUnder, System.Type baseType,
                                           System.Action<System.Type> callback,
                                           bool allowAbstractTypes = false, bool allowInterfaces = false,
                                        System.Type defaultType = null, System.Type[] excludedTypes = null,
                                        TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            int controlID = GUIUtility.GetControlID(TypeSelectionDropDownWindow.s_TypeCustomHash, EditorGUIUtility.native, positionUnder);
            CallbackInfo.instance = new CallbackInfo(controlID, null, callback);
            TypeSelectionDropDownWindow.DisplayCustomMenu(positionUnder, GUIContent.none, baseType, null, allowAbstractTypes, allowInterfaces, defaultType, excludedTypes, listType);
        }

        private static void DisplayCustomMenu(Rect position, GUIContent label,
                                               System.Type baseType, System.Type selectedType,
                                               bool allowAbstractTypes = false, bool allowInterfaces = false,
                                               System.Type defaultType = null, System.Type[] excludedTypes = null,
                                               TypeDropDownListingStyle listType = TypeDropDownListingStyle.Namespace)
        {
            if (_window != null)
            {
                _window.Close();
            }

            var pos = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = pos.x;
            position.y = pos.y;

            _window = EditorWindow.CreateInstance<TypeSelectionDropDownWindow>();
            _window.BaseType = baseType;
            _window.AllowAbstractTypes = allowAbstractTypes;
            _window.AllowInterface = allowInterfaces;
            _window.DefaultType = defaultType;
            _window.ExcludedTypes = excludedTypes;
            _window.ListingStyle = listType;

            _window.ShowAsDropDown(position, new Vector2(position.width, 320f));
            _window.Focus();
        }


        private class CallbackInfo
        {
            public const string CMND_MENUCHANGED = "TypeSelectionPopupMenuChanged";
            public static CallbackInfo instance;

            private int _controlID;
            private System.Type _selectedType;
            private com.spacepuppyeditor.Internal.GUIViewProxy _sourceView;
            private System.Action<System.Type> _callback;

            public CallbackInfo(int controlId, System.Type tp)
            {
                _controlID = controlId;
                _selectedType = tp;
                _sourceView = com.spacepuppyeditor.Internal.GUIViewProxy.GetCurrent();
            }

            public CallbackInfo(int controlId, System.Type tp, System.Action<System.Type> callback)
            {
                _controlID = controlId;
                _selectedType = tp;
                _sourceView = com.spacepuppyeditor.Internal.GUIViewProxy.GetCurrent();
                _callback = callback;
            }

            public void SignalChange(System.Type tp)
            {
                if (tp != this._selectedType)
                {
                    _selectedType = tp;
                    if(_sourceView != null) _sourceView.SendEvent(EditorGUIUtility.CommandEvent(CMND_MENUCHANGED));
                    if (_callback != null) _callback(_selectedType);
                }
            }

            public static System.Type GetSelectedValueForControl(int controlID, System.Type selected)
            {
                Event current = Event.current;
                if(current.type == EventType.ExecuteCommand && current.commandName == CMND_MENUCHANGED)
                {
                    if(instance != null && instance._controlID == controlID)
                    {
                        selected = instance._selectedType;
                        GUI.changed = true;
                        current.Use();
                    }
                }
                return selected;
            }

        }

        #endregion

    }
}
