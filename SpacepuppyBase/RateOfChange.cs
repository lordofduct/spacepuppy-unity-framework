using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents a rate of change in units per period with some timesupplier.
    /// </summary>
    [System.Serializable()]
    public struct RateOfChange
    {

        public float Units;
        public SPTimePeriod TimePeriod;


        public RateOfChange(float units, SPTimePeriod period)
        {
            this.Units = units;
            this.TimePeriod = period;
        }
        

        public double GetDelta()
        {
            var dt = this.TimePeriod.TimeSupplier.Delta / this.TimePeriod.Seconds;
            return (dt * this.Units);
        }

        public double GetDelta(float deltaSeconds)
        {
            var dt = deltaSeconds / this.TimePeriod.Seconds;
            return (dt * this.Units);
        }

    }
}
