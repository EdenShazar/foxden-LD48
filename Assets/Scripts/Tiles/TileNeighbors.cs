using System;
using System.Collections.Generic;

public static class TileNeighbors
{
    [Flags]
    public enum Neighbors
    {
        NONE = 0,
        Z = 1,
        X = 2,
        C = 4,
        A = 8,
        D = 16,
        Q = 32,
        W = 64,
        E = 128
    }

    private static Dictionary<int, int> tileIndices = new Dictionary<int, int>() {
            { (int)(Neighbors.NONE), 0 },
            { (int)(Neighbors.X), 1 },
            { (int)(Neighbors.A), 2 },
            { (int)(Neighbors.D), 3 },
            { (int)(Neighbors.W), 4 },
            { (int)(Neighbors.X | Neighbors.W), 5 },
            { (int)(Neighbors.A | Neighbors.D), 6 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A), 7 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.D), 8 },
            { (int)(Neighbors.X | Neighbors.A | Neighbors.D), 9 },
            { (int)(Neighbors.X | Neighbors.A | Neighbors.W), 10 },
            { (int)(Neighbors.X | Neighbors.D | Neighbors.W), 11 },
            { (int)(Neighbors.A | Neighbors.D | Neighbors.W), 12 },
            { (int)(Neighbors.A | Neighbors.Q | Neighbors.W), 13 },
            { (int)(Neighbors.D | Neighbors.W | Neighbors.E), 14 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.D), 15 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.W), 16 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D), 17 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.D | Neighbors.W), 18 },
            { (int)(Neighbors.X | Neighbors.A | Neighbors.Q | Neighbors.W), 19 },
            { (int)(Neighbors.X | Neighbors.D | Neighbors.W | Neighbors.E), 20 },
            { (int)(Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W), 21 },
            { (int)(Neighbors.A | Neighbors.D | Neighbors.W | Neighbors.E), 22 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D), 23 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.Q | Neighbors.W), 24 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.D | Neighbors.W | Neighbors.E), 25 },
            { (int)(Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W | Neighbors.E), 26 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.W), 27 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W), 28 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.D | Neighbors.W | Neighbors.E), 29 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W), 30 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.W | Neighbors.E), 31 },
            { (int)(Neighbors.X | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W | Neighbors.E), 32 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W), 33 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.W | Neighbors.E), 34 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W | Neighbors.E), 35 },
            { (int)(Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W | Neighbors.E), 36 },
            { (int)(Neighbors.Z | Neighbors.X | Neighbors.C | Neighbors.A | Neighbors.D | Neighbors.Q | Neighbors.W | Neighbors.E), 37 }
        };

    public static int GetTileIndex(Neighbors neighbors)
    {
        int neighborsInt = (int)neighbors;

        if (!tileIndices.ContainsKey(neighborsInt))
            return tileIndices.Count - 1;
        
        return tileIndices[neighborsInt];
    }
}
