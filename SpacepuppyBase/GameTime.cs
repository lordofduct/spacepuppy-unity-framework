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




        //public static float ActualTimeSinceStartup { get { return UnityEngine.Time.realtimeSinceStartup; } }

        //public static float Time { get { return UnityEngine.Time.time; } }

        //public static float RealTime { get { return UnityEngine.Time.unscaledTime; } }

        //public static float FixedTime { get { return UnityEngine.Time.fixedTime; } }

        //public static float DeltaTime { get { return UnityEngine.Time.deltaTime; } }

        //public static float RealDeltaTime { get { return UnityEngine.Time.unscaledDeltaTime; } }

        //public static float SmoothDeltaTime { get { return UnityEngine.Time.smoothDeltaTime; } }


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
                return new GameTime(UnityEngine.Time.unscaledTime, UnityEngine.Time.unscaledDeltaTime);
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
            }
        }

    }

}