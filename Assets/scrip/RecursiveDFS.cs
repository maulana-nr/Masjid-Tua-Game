using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveDFS : MazeLogic
{
    public List<MapLocation> directions = new List<MapLocation>() {
        new MapLocation(1,0),
        new MapLocation(0,1),
        new MapLocation(-1,0),
        new MapLocation(0,-1)
    };

    public override void GenerateMaps()
    {
        int sx, sz;

        // cari titik random yang bukan room (map != 2)
        do
        {
            sx = Random.Range(1, width - 1);
            sz = Random.Range(1, depth - 1);
        }
        while (map[sx, sz] == 2); // jangan start dari room

        Generate(sx, sz);
    }


    void Generate(int x, int z)
    {
        if (map[x, z] == 2) return; // jangan jalanin DFS di room
        if (CountSquareNeighbours(x, z) >= 2) return;

        map[x, z] = 0;

        directions.Shuffle();

        Generate(x + directions[0].x, z + directions[0].z);
        Generate(x + directions[1].x, z + directions[1].z);
        Generate(x + directions[2].x, z + directions[2].z);
        Generate(x + directions[3].x, z + directions[3].z);
    }

}
