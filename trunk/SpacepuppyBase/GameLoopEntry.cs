using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Hooks;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// This class is really only for internal use by com.spacepuppy, avoid using it outside of it.
    /// </summary>
    public static class GameLoopEntry
    {

        #region Events

        public static event System.EventHandler EarlyUpdate;
        public static event System.EventHandler Update;
        public static event System.EventHandler TardyUpdate;
        public static event System.EventHandler EarlyFixedUpdate;
        public static event System.EventHandler FixedUpdate;
        public static event System.EventHandler TardyFixedUpdate;
        public static event System.EventHandler EarlyLateUpdate;
        public static event System.EventHandler LateUpdate;
        public static event System.EventHandler TardyLateUpdate;

        #endregion

        #region Fields

        private static GameObject _gameObject;
        private static EarlyExecutionUpdateEventHooks _earlyUpdateHook;
        private static UpdateEventHooks _updateHook;
        private static TardyExecutionUpdateEventHooks _tardyUpdateHook;

        private static bool _inUpdateLoop;

        #endregion

        #region CONSTRUCTOR

        static GameLoopEntry()
        {
            _gameObject = new GameObject("SpacePuppy.GameLoopEntryObject");
            _earlyUpdateHook = _gameObject.AddComponent<EarlyExecutionUpdateEventHooks>();
            _updateHook = _gameObject.AddComponent<UpdateEventHooks>();
            _tardyUpdateHook = _gameObject.AddComponent<TardyExecutionUpdateEventHooks>();
            GameObject.DontDestroyOnLoad(_gameObject);

            _earlyUpdateHook.UpdateHook += _earlyUpdateHook_Update;
            _updateHook.UpdateHook += _updateHook_Update;
            _tardyUpdateHook.UpdateHook += _tardyUpdateHook_Update;

            _earlyUpdateHook.FixedUpdateHook += _earlyUpdateHook_FixedUpdate;
            _updateHook.FixedUpdateHook += _updateHook_FixedUpdate;
            _tardyUpdateHook.FixedUpdateHook += _tardyUpdateHook_FixedUpdate;

            _earlyUpdateHook.LateUpdateHook += _earlyUpdateHook_LateUpdate;
            _updateHook.LateUpdateHook += _updateHook_LateUpdate;
            _tardyUpdateHook.LateUpdateHook += _tardyUpdateHook_LateUpdate;
        }

        #endregion

        #region Properties

        public static bool InUpdateLoop { get { return _inUpdateLoop; } }

        #endregion

        #region Methods

        //Regular

        public static Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            return _updateHook.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(System.Collections.IEnumerable routine)
        {
            return _updateHook.StartCoroutine(routine);
        }

        public static Coroutine StartCoroutine(System.Delegate routine, params object[] args)
        {
            return _updateHook.StartCoroutine(routine, args);
        }

        //Radical

        public static RadicalCoroutine StartRadicalCoroutine(System.Collections.IEnumerator routine)
        {
            return _updateHook.StartRadicalCoroutine(routine);
        }

        public static RadicalCoroutine StartRadicalCoroutine(System.Collections.IEnumerable routine)
        {
            return _updateHook.StartRadicalCoroutine(routine);
        }

        public static RadicalCoroutine StartRadicalCoroutine(System.Delegate routine, params object[] args)
        {
            return _updateHook.StartRadicalCoroutine(routine, args);
        }

        #endregion

        #region Event Handlers

        //Update

        private static void _earlyUpdateHook_Update(object sender, System.EventArgs e)
        {
            //Track entry into update loop
            _inUpdateLoop = true;

            if (EarlyUpdate != null) EarlyUpdate(sender, e);
        }

        private static void _updateHook_Update(object sender, System.EventArgs e)
        {
            if (Update != null) Update(sender, e);
        }

        private static void _tardyUpdateHook_Update(object sender, System.EventArgs e)
        {
            if (TardyUpdate != null) TardyUpdate(sender, e);
        }

        //Fixed Update

        private static void _earlyUpdateHook_FixedUpdate(object sender, System.EventArgs e)
        {
            if (EarlyFixedUpdate != null) EarlyFixedUpdate(sender, e);
        }

        private static void _updateHook_FixedUpdate(object sender, System.EventArgs e)
        {
            if (FixedUpdate != null) FixedUpdate(sender, e);
        }

        private static void _tardyUpdateHook_FixedUpdate(object sender, System.EventArgs e)
        {
            if (TardyFixedUpdate != null) TardyFixedUpdate(sender, e);
        }

        //LateUpdate

        private static void _earlyUpdateHook_LateUpdate(object sender, System.EventArgs e)
        {
            if (EarlyLateUpdate != null) EarlyLateUpdate(sender, e);
        }

        private static void _updateHook_LateUpdate(object sender, System.EventArgs e)
        {
            if (LateUpdate != null) LateUpdate(sender, e);
        }

        private static void _tardyUpdateHook_LateUpdate(object sender, System.EventArgs e)
        {
            if (TardyLateUpdate != null) TardyLateUpdate(sender, e);

            //Track exit of update loop
            _inUpdateLoop = false;
        }

        #endregion

    }
}
