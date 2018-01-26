using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput.Unity
{

    public class InputProfileLookupTable<T, TButton, TAxis> 
           : ICollection<InputProfileLookupTable<T, TButton, TAxis>.InputProfileLookupEntry>,
           ISPDisposable
           where T : class, IInputProfile<TButton, TAxis> 
           where TButton : struct, System.IConvertible 
           where TAxis : struct, System.IConvertible
    {

        #region Fields

        private List<InputProfileLookupEntry> _table = new List<InputProfileLookupEntry>();

        #endregion

        #region Methods

        /// <summary>
        /// Loads all profiles of type (T) into the list with their appropriate descriptions.
        /// 
        /// Load clears the table before loading. Make sure you only add custom entries AFTER calling Load.
        /// </summary>
        public void Load(bool includeIfNoDescription = false, bool includeIfNotThisPlatform = false)
        {
            _table.Clear();

            var currentPlatform = UnityEngine.Application.platform.ToTargetPlatform();

            foreach(var tp in com.spacepuppy.Utils.TypeUtil.GetTypesAssignableFrom(typeof(T)))
            {
                if (tp.IsAbstract || tp.IsInterface || tp.GetConstructor(Type.EmptyTypes) == null) continue;

                var description = tp.GetCustomAttributes(typeof(InputProfileDescriptionAttribute), false).FirstOrDefault() as InputProfileDescriptionAttribute;
                if (!includeIfNoDescription && description == null) continue; //if no description was attached, skip
                if (!includeIfNotThisPlatform && (description.Platform & currentPlatform) == 0) continue; //If we don't match the expected platform, skip

                var entry = new InputProfileLookupEntry()
                {
                    ProfileType = tp,
                    Platform = description != null ? description.Platform : TargetPlatform.Unknown,
                    DisplayName = description != null ? description.DisplayName ?? string.Empty : string.Empty,
                    Description = description != null ? description.Description ?? string.Empty : string.Empty
                };
                
                var joysticks = tp.GetCustomAttributes(typeof(InputProfileJoystickNameAttribute), false) as InputProfileJoystickNameAttribute[];
                if(joysticks != null && joysticks.Length > 0)
                {
                    entry.JoystickNames = (from a in joysticks where !string.IsNullOrEmpty(a.JoystickName) select a.JoystickName).ToArray();
                }
                else
                {
                    entry.JoystickNames = com.spacepuppy.Utils.ArrayUtil.Empty<string>();
                }

                _table.Add(entry);
            }
        }

        public InputProfileLookupEntry Find(System.Predicate<InputProfileLookupEntry> predicate)
        {
            if (predicate == null) throw new System.ArgumentNullException("predicate");

            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                if (predicate(e.Current)) return e.Current;
            }

            return default(InputProfileLookupEntry);
        }

        public InputProfileLookupEntry Find(string joystickName)
        {
            var e = _table.GetEnumerator();
            while(e.MoveNext())
            {
                if (Array.IndexOf(e.Current.JoystickNames, joystickName) >= 0) return e.Current;
            }

            return default(InputProfileLookupEntry);
        }

        public T Create(string joystickName)
        {
            return this.Find(joystickName).CreateProfile();
        }

        #endregion

        #region ICollection Interface

        public int Count { get { return _table.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(InputProfileLookupEntry item)
        {
            if (item.JoystickNames == null) item.JoystickNames = com.spacepuppy.Utils.ArrayUtil.Empty<string>();
            _table.Add(item);
        }

        public bool Contains(InputProfileLookupEntry item)
        {
            if (item.JoystickNames == null) item.JoystickNames = com.spacepuppy.Utils.ArrayUtil.Empty<string>();
            return _table.Contains(item);
        }

        public void CopyTo(InputProfileLookupEntry[] array, int arrayIndex)
        {
            _table.CopyTo(array, arrayIndex);
        }

        public bool Remove(InputProfileLookupEntry item)
        {
            if (item.JoystickNames == null) item.JoystickNames = com.spacepuppy.Utils.ArrayUtil.Empty<string>();
            return _table.Remove(item);
        }

        public void Clear()
        {
            _table.Clear();
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<InputProfileLookupEntry> IEnumerable<InputProfileLookupEntry>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable Interface

        private bool _isDisposed;

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Unloads all profiles and forces GC to purge them out. This should be called after a lengthy input creation process. 
        /// Otherwise the garbage created here in may be collected further down the line and impact framerate.
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            _table.Clear();
            _table = null;
            //force collect all those loaded profiles
            GC.Collect();
        }

        #endregion

        #region Special Types

        public struct InputProfileLookupEntry
        {
            public Type ProfileType;
            public TargetPlatform Platform;
            public string DisplayName;
            public string Description;
            public string[] JoystickNames;

            /// <summary>
            /// The constructor of the profile type exists and has 0-parameters.
            /// </summary>
            public bool ValidConstructor
            {
                get { return ProfileType != null && !ProfileType.IsAbstract && !ProfileType.IsInterface && ProfileType.GetConstructor(Type.EmptyTypes) != null; }
            }

            public T CreateProfile()
            {
                if(this.ValidConstructor)
                {
                    return Activator.CreateInstance(ProfileType) as T;
                }

                return null;
            }
        }

        public struct Enumerator : IEnumerator<InputProfileLookupEntry>
        {

            private List<InputProfileLookupEntry>.Enumerator _e;

            public Enumerator(InputProfileLookupTable<T, TButton, TAxis> table)
            {
                if (table == null) throw new System.ArgumentNullException("table");

                _e = table._table.GetEnumerator();
            }

            public InputProfileLookupEntry Current
            {
                get { return _e.Current; }
            }

            object IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                (_e as IEnumerator).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

        }

        #endregion

    }

}
