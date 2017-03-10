using System;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Graphs
{
    [System.Flags]
    public enum GridNeighbour : byte
    {
        None = 0,
        North = 1,
        N = 1,
        NE = 2,
        East = 4,
        E = 4,
        SE = 8,
        South = 16,
        S = 16,
        SW = 32,
        West = 64,
        W = 64,
        NW = 128,

        NESW = 85,
        Diagonals = 170,
        All = 255
    }

    public static class GridNeighbourUtil
    {

        /// <summary>
        /// Returns the side opposite of input.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static GridNeighbour Opposite(this GridNeighbour side)
        {
            int e = (int)side;
            int result = 0;
            for(int i = 0; i < 8; i++)
            {
                int f = 1 << i;
                if ((e & (1 << i)) != 0) result |= (i < 4) ? f << 4 : f >> 4;
            }
            return (GridNeighbour)result;
        }

        /// <summary>
        /// Rotates side x number of 45 degree turns around the cardinal directions clockwise.
        /// North turned twice is East, South turned -1 is SouthEast.
        /// </summary>
        /// <param name="side"></param>
        /// <param name="turns"></param>
        /// <returns></returns>
        public static GridNeighbour Rotate(this GridNeighbour side, int turns)
        {
            turns = turns % 8;
            if (turns == 0) return side;

            if (turns < 0)
            {
                turns = System.Math.Abs(turns);
                int i = (int)side << (8 - turns);
                i = (i & 255) | (i >> 8);
                return (GridNeighbour)i;
            }
            else
            {
                return (GridNeighbour)(((int)side << turns) % 255);
            }
        }
        
        /// <summary>
        /// Gets the first side reached rotating clockwise. 
        /// If All, North is returned.
        /// If None, None is returned.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static GridNeighbour FirstSide(this GridNeighbour side)
        {
            if (side == GridNeighbour.All) return GridNeighbour.North;
            if (side == GridNeighbour.None) return GridNeighbour.None;

            GridNeighbour start = GridNeighbour.North;
            if(side.HasFlag(start))
            {
                if (!side.HasFlag(GridNeighbour.North.Rotate(-1))) return GridNeighbour.North;

                for(int i = 1; i < 8; i++)
                {
                    if(!side.HasFlag(1 << i))
                    {
                        start = (GridNeighbour)(1 << (i + 1));
                    }
                }
            }

            while(!side.HasFlag(start))
            {
                start = (GridNeighbour)((int)start << 1);
            }
            return start;
        }

    }
    
}
