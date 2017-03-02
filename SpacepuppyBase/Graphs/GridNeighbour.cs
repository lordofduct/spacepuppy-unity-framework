using System;

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

    }
    
}
