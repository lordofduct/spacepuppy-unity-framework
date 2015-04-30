using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{

    [InitializeOnLoad()]
    public static class EditorGame
    {

        public enum State
        {
            Stopped = 0,
            Playing = 1,
            Paused = 2
        }

        #region Events

        /// <summary>
        /// Signal a change in playmode. Parameter is the state we've just left.
        /// </summary>
        public static event System.Action PlayModeChanged;

        #endregion

        #region Fields

        private static State _last = State.Stopped;
        private static State _current = State.Stopped;

        #endregion

        #region STATIC CONSTRUCTOR

        static EditorGame()
        {
            EditorApplication.playmodeStateChanged += OnPlayModeChanged;
        }

        #endregion

        #region Properties

        public static State LastState { get { return _last; } }

        public static State CurrentState { get { return _current; } }

        #endregion

        #region Methods

        public static bool Play()
        {
            if (EditorApplication.isPlaying) return false;
            EditorApplication.isPlaying = true;
            return true;
        }

        public static bool Stop()
        {
            if (!EditorApplication.isPlaying) return false;
            EditorApplication.isPlaying = false;
            return true;
        }

        public static bool TogglePause()
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
            return EditorApplication.isPaused;
        }

        #endregion

        #region Event Handlers

        private static void OnPlayModeChanged()
        {
            var nextState = State.Stopped;
            switch(_current)
            {
                case State.Stopped:
                    if (EditorApplication.isPlayingOrWillChangePlaymode) nextState = State.Playing;
                    break;
                case State.Playing:
                    nextState = (EditorApplication.isPaused) ? State.Paused : State.Stopped;
                    break;
                case State.Paused:
                    nextState = (EditorApplication.isPlayingOrWillChangePlaymode) ? State.Playing : State.Stopped;
                    break;
            }

            _last = _current;
            _current = nextState;
            if (PlayModeChanged != null) PlayModeChanged();
        }

        #endregion

    }
}
