﻿using com.spacepuppy.Hooks;

namespace com.spacepuppy
{

    /// <summary>
    /// This class is really only for internal use by com.spacepuppy, avoid using it outside of it.
    /// </summary>
    [Singleton.Config(SingletonLifeCycleRule.LivesForever, ExcludeFromSingletonManager = true, LifeCycleReadOnly = true)]
    public sealed class GameLoopEntry : Singleton
    {

        #region Events

        public static event System.EventHandler BeforeApplicationQuit;
        public static event System.EventHandler ApplicatinQuit;

        public static event System.EventHandler EarlyUpdate;
        public static event System.EventHandler OnUpdate;
        public static event System.EventHandler TardyUpdate;
        public static event System.EventHandler EarlyFixedUpdate;
        public static event System.EventHandler OnFixedUpdate;
        public static event System.EventHandler TardyFixedUpdate;
        public static event System.EventHandler EarlyLateUpdate;
        public static event System.EventHandler OnLateUpdate;
        public static event System.EventHandler TardyLateUpdate;

        #endregion

        #region Fields

        private static GameLoopEntry _instance;
        private static UpdateSequence _currentSequence;
        private static QuitState _quitState;
        private static System.Action<bool> _internalEarlyUpdate;

        private UpdateEventHooks _updateHook;
        private TardyExecutionUpdateEventHooks _tardyUpdateHook;

        [System.NonSerialized]
        private static UpdatePump _updatePump = new UpdatePump();
        [System.NonSerialized]
        private static UpdatePump _fixedUpdatePump = new UpdatePump();
        [System.NonSerialized]
        private static UpdatePump _lateUpdatePump = new UpdatePump();

        [System.NonSerialized]
        private static com.spacepuppy.Async.InvokePump _updateInvokeHandle = new com.spacepuppy.Async.InvokePump();
        [System.NonSerialized]
        private static com.spacepuppy.Async.InvokePump _lateUpdateInvokeHandle = new com.spacepuppy.Async.InvokePump();
        [System.NonSerialized]
        private static com.spacepuppy.Async.InvokePump _fixedUpdateInvokeHandle = new com.spacepuppy.Async.InvokePump();

        private static int _currentFrame;
        private static int _currentLateFrame;

        #endregion

        #region CONSTRUCTOR

        public static void Init()
        {
            if (_instance != null) return;

            _instance = Singleton.CreateSpecialInstance<GameLoopEntry>("SpacePuppy.GameLoopEntry", SingletonLifeCycleRule.LivesForever);
            //_instance = Singleton.GetInstance<GameLoopEntry>();
        }

        protected override void OnValidAwake()
        {
            _updateHook = this.gameObject.AddComponent<UpdateEventHooks>();
            _tardyUpdateHook = this.gameObject.AddComponent<TardyExecutionUpdateEventHooks>();

            _updateHook.UpdateHook += _updateHook_Update;
            _tardyUpdateHook.UpdateHook += _tardyUpdateHook_Update;

            _updateHook.FixedUpdateHook += _updateHook_FixedUpdate;
            _tardyUpdateHook.FixedUpdateHook += _tardyUpdateHook_FixedUpdate;

            _updateHook.LateUpdateHook += _updateHook_LateUpdate;
            _tardyUpdateHook.LateUpdateHook += _tardyUpdateHook_LateUpdate;
        }

        /// <summary>
        /// A special static, register once, earlyupdate event hook that preceeds ALL other events. 
        /// This is used internally by some special static classes (namely SPTime) that needs extra 
        /// high precedence early access.
        /// </summary>
        /// <param name="d"></param>
        internal static void RegisterInternalEarlyUpdate(System.Action<bool> d)
        {
            _internalEarlyUpdate -= d;
            _internalEarlyUpdate += d;
        }

        internal static void UnregisterInternalEarlyUpdate(System.Action<bool> d)
        {
            _internalEarlyUpdate -= d;
        }

        #endregion

        #region Properties

        public override SingletonLifeCycleRule LifeCycle
        {
            get { return SingletonLifeCycleRule.LivesForever; }
            set { }
        }

        public static bool Initialized { get { return _instance != null; } }

        public static GameLoopEntry Hook
        {
            get
            {
                if (_instance == null) GameLoopEntry.Init();
                return _instance;
            }
        }

        public static UpdateEventHooks TardyHook
        {
            get
            {
                if (_instance == null) GameLoopEntry.Init();
                return _instance._tardyUpdateHook;
            }
        }

        /// <summary>
        /// Returns which event sequence that code is currently operating as. 
        /// WARNING - during 'OnMouseXXX' messages this will report that we're in the FixedUpdate sequence. 
        /// This is because there's no end of FixedUpdate available to hook into, so it reports FixedUpdate 
        /// until Update starts, and 'OnMouseXXX' occurs in between those 2.
        /// </summary>
        public static UpdateSequence CurrentSequence { get { return _currentSequence; } }

        public static QuitState QuitState { get { return _quitState; } }

        /// <summary>
        /// Returns true if the OnApplicationQuit message has been received.
        /// </summary>
        public static bool ApplicationClosing { get { return _quitState == QuitState.Quit; } }

        public static UpdatePump UpdatePump { get { return _updatePump; } }

        public static UpdatePump FixedUpdatePump { get { return _fixedUpdatePump; } }

        public static UpdatePump LateUpdatePump { get { return _lateUpdatePump; } }

        public static com.spacepuppy.Async.InvokePump UpdateHandle { get { return _updateInvokeHandle; } }

        public static com.spacepuppy.Async.InvokePump LateUpdateHandle { get {return _lateUpdateInvokeHandle; } }

        public static com.spacepuppy.Async.InvokePump FixedUpdateHandle { get { return _fixedUpdateInvokeHandle; } }

        /// <summary>
        /// Returns true if the UpdatePump and Update event were ran.
        /// </summary>
        public static bool UpdateWasCalled
        {
            get { return _currentFrame == UnityEngine.Time.frameCount; }
        }

        /// <summary>
        /// Returns true if the LateUpdatePump and LateUpdate event were ran.
        /// </summary>
        public static bool LateUpdateWasCalled
        {
            get { return _currentFrame == UnityEngine.Time.frameCount; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Preferred method of closing application.
        /// </summary>
        public static void QuitApplication()
        {
            if(_quitState == QuitState.None)
            {
                _quitState = QuitState.BeforeQuit;
                if (BeforeApplicationQuit != null) BeforeApplicationQuit(_instance, System.EventArgs.Empty);

                if(_quitState == QuitState.BeforeQuit)
                {
                    //wasn't cancelled, or force quit
                    if (UnityEngine.Application.isEditor)
                    {
                        try
                        {
                            var tp = com.spacepuppy.Utils.TypeUtil.FindType("UnityEditor.EditorApplication");
                            tp.GetProperty("isPlaying").SetValue(null, false, null);
                        }
                        catch
                        {
                            UnityEngine.Debug.Log("Failed to stop play in editor.");
                        }
                    }
                    else
                    {
                        UnityEngine.Application.Quit();
                    }
                    
                }
            }
        }

        public static void CancelQuit()
        {
            if(_quitState == QuitState.BeforeQuit)
            {
                _quitState = QuitState.None;
            }
        }

        public static void RegisterNextUpdate(IUpdateable obj)
        {
            if (UpdateWasCalled) _updatePump.Add(obj);
            else _updatePump.DelayedAdd(obj);
        }

        public static void RegisterNextLateUpdate(IUpdateable obj)
        {
            if (LateUpdateWasCalled) _lateUpdatePump.Add(obj);
            else _lateUpdatePump.DelayedAdd(obj);
        }

        #endregion

        #region Event Handlers

        private void OnApplicationQuit()
        {
            _quitState = QuitState.Quit;
            if (ApplicatinQuit != null) ApplicatinQuit(this, System.EventArgs.Empty);
        }
        
        //Update

        private void Update()
        {
            //Track entry into update loop
            _currentSequence = UpdateSequence.Update;

            if (_internalEarlyUpdate != null) _internalEarlyUpdate(false);

            _updateInvokeHandle.Update();

            if (EarlyUpdate != null) EarlyUpdate(this, System.EventArgs.Empty);
        }

        private void _updateHook_Update(object sender, System.EventArgs e)
        {
            if (OnUpdate != null) OnUpdate(this, e);
            _updatePump.Update();
            _currentFrame = UnityEngine.Time.frameCount;
        }

        private void _tardyUpdateHook_Update(object sender, System.EventArgs e)
        {
            if (TardyUpdate != null) TardyUpdate(this, e);
        }

        //Fixed Update

        private void FixedUpdate()
        {
            //Track entry into fixedupdate loop
            _currentSequence = UpdateSequence.FixedUpdate;

            if (_internalEarlyUpdate != null) _internalEarlyUpdate(true);

            _fixedUpdateInvokeHandle.Update();

            if (EarlyFixedUpdate != null) EarlyFixedUpdate(this, System.EventArgs.Empty);
        }

        private void _updateHook_FixedUpdate(object sender, System.EventArgs e)
        {
            if (OnFixedUpdate != null) OnFixedUpdate(this, e);
            _fixedUpdatePump.Update();
        }

        private void _tardyUpdateHook_FixedUpdate(object sender, System.EventArgs e)
        {
            if (TardyFixedUpdate != null) TardyFixedUpdate(this, e);

            ////Track exit of fixedupdate loop
            //_currentSequence = UpdateSequence.None;
        }

        //LateUpdate

        private void LateUpdate()
        {
            _currentSequence = UpdateSequence.LateUpdate;
            if (EarlyLateUpdate != null) EarlyLateUpdate(this, System.EventArgs.Empty);
        }

        private void _updateHook_LateUpdate(object sender, System.EventArgs e)
        {
            if (OnLateUpdate != null) OnLateUpdate(this, e);
            _lateUpdatePump.Update();
            _currentLateFrame = UnityEngine.Time.frameCount;
        }

        private void _tardyUpdateHook_LateUpdate(object sender, System.EventArgs e)
        {
            _lateUpdateInvokeHandle.Update();
            if (TardyLateUpdate != null) TardyLateUpdate(this, e);

            //Track exit of update loop
            _currentSequence = UpdateSequence.None;
        }

        #endregion
        
    }
}
