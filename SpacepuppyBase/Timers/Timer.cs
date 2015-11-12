namespace com.spacepuppy.Timers
{

    public class Timer : ITimer
    {

        private static long _instance = 0;

        #region Events

        public event System.Action<Timer> TimerCount;
        public event System.Action<Timer> TimerComplete;

        #endregion

        #region Fields

        private string _name;

        private float _interval;
        private int _repeatCount;

        private float _currentTime;
        private int _elapsedCount;

        #endregion

        #region CONSTRUCTOR
        
        public Timer()
        {
            _instance++;
        }

        public Timer(float interval) : this()
        {
            _interval = interval;
            _repeatCount = 0;
        }

        /// <summary>
        /// Create a timer with a delay in seconds and a certain repeat count. A repeat count of 0 continues forever.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="repeatCount"></param>
        public Timer(float interval, int repeatCount) : this()
        {
            _interval = interval;
            _repeatCount = repeatCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Interval of the timer.
        /// </summary>
        public float Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        /// <summary>
        /// Number of times the interval should repeat before completing. Negative values repeat forever.
        /// </summary>
        public int RepeatCount
        {
            get { return _repeatCount; }
            set { _repeatCount = value; }
        }

        /// <summary>
        /// Amount of time into the current interval has passed.
        /// </summary>
        public float CurrentTime
        {
            get { return _currentTime; }
        }

        /// <summary>
        /// Number of times the timer has elapsed.
        /// </summary>
        public int ElapsedCount
        {
            get { return _elapsedCount; }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _elapsedCount = 0;
            _currentTime = 0;
        }

        #endregion

        #region ITimer Interface

        public bool Update(float dt)
        {
            _currentTime += dt;

            while (_currentTime > _interval)
            {
                _currentTime -= _interval;
                _elapsedCount++;

                if (this.TimerCount != null) this.TimerCount(this);

                if (_repeatCount >= 0 && _elapsedCount > _repeatCount)
                {
                    if (this.TimerComplete != null)
                    {
                        this.TimerComplete(this);
                    }
                    return false;
                }
            }

            return true;
        }

        #endregion

    }

}
