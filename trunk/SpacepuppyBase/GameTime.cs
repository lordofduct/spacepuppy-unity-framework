using UnityEngine;

namespace com.spacepuppy
{

    public struct GameTime
    {

        public float Total;
        public float Delta;

        public GameTime(float t, float dt)
        {
            this.Total = t;
            this.Delta = dt;
        }



        #region Static Interface

        /// <summary>
        /// SP Time, ignores moments when game was paused.
        /// </summary>
        public static float Time { get { return UnityEngine.Time.time; } }


        /// <summary>
        /// SP deltaTime, ignores moments when game is paused.
        /// </summary>
        public static float DeltaTime { get { return UnityEngine.Time.deltaTime; } }

        public static float RealDeltaTime { get { return UnityEngine.Time.unscaledDeltaTime; } }


        public static GameTime Current
        {
            get
            {
                return new GameTime(UnityEngine.Time.time, UnityEngine.Time.deltaTime);
            }
        }

        public static GameTime CurrentReal
        {
            get
            {
                return new GameTime(UnityEngine.Time.time, UnityEngine.Time.deltaTime);
            }
        }

        public static GameTime CurrentFixed
        {
            get
            {
                return new GameTime(UnityEngine.Time.fixedTime, UnityEngine.Time.fixedDeltaTime);
            }
        }

        public static GameTime CurrentSmooth
        {
            get
            {
                return new GameTime(UnityEngine.Time.time, UnityEngine.Time.smoothDeltaTime);
            }
        }

        public static float TimeScale
        {
            get
            {
                return UnityEngine.Time.timeScale;
            }
            set
            {
                UnityEngine.Time.timeScale = value;
                UnityEngine.Time.fixedDeltaTime = UnityEngine.Time.timeScale * 0.02f;
            }
        }

        #endregion

    }

}