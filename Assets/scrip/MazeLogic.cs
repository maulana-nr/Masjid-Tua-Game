using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapLocation
{
    public int x;
    public int z;

    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

//tambahan
public class Room
{
    public int startX, startZ, width, depth;
    public List<Vector2Int> tiles = new List<Vector2Int>(); // semua tile di room
}

public class MazeLogic : MonoBehaviour
{
    public int width = 40; //x length
    public int depth = 40; //z length
    public int scale = 6;
    public GameObject Character; //Player Character
    public GameObject Enemy; //Enemy Character
    public int EnemyCount = 4;
    public int RoomCount = 4;
    public int RoomMinSize = 6;
    public int RoomMaxSize = 10;
    public NavMeshSurface Surface;
    public List<GameObject> Cube; //Maze Wall
    public byte[,] map;
    public GameObject Item; // Prefab item yang akan diambil player
    public int ItemCount = 4; // jumlah item yang ingin disebar, tambahan



    void Start()
    {
        InitialiseMap();
        AddRooms(RoomCount, RoomMinSize, RoomMaxSize); // room duluan!
        GenerateMaps();    // DFS setelah room
        DrawMaps();
        PlaceCharacter();
        PlaceEnemy();
        PlaceItems(); //tambahan
        // Surface.BuildNavMesh();
    }


    void InitialiseMap() // initialise all Maps with 1
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;        // 1 = wall  
            }
    }

    public virtual void GenerateMaps() // random corridors
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                    map[x, z] = 0;    // 0 = corridor
            }
    }

    void DrawMaps() // Draw All Value With GameObject
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Cube[Random.Range(0, Cube.Count)], pos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(scale, scale, scale);
                    wall.transform.position = pos;
                }
            }
    }



    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public virtual void PlaceCharacter()
    {
        bool PlayerSet = false;
        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int x = Random.Range(0, width);
                int z = Random.Range(0, depth);
                if (map[x, z] == 0 && !PlayerSet)
                {
                    Debug.Log("placing character");
                    PlayerSet = true;
                    Character.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                else if (PlayerSet)
                {
                    Debug.Log("already Placing character");
                    return;
                }
            }
        }
    }

    //tambahan duanya
    private List<Room> rooms = new List<Room>();

    public virtual void AddRooms(int count, int minSize, int maxSize)
    {
        rooms.Clear();
        for (int i = 0; i < count; i++)
        {
            int roomWidth = Random.Range(minSize, maxSize);
            int roomDepth = Random.Range(minSize, maxSize);
            int startX = Random.Range(1, width - roomWidth - 1);
            int startZ = Random.Range(1, depth - roomDepth - 1);

            Room room = new Room();
            room.startX = startX;
            room.startZ = startZ;
            room.width = roomWidth;
            room.depth = roomDepth;

            for (int x = startX; x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < startZ + roomDepth; z++)
                {
                    map[x, z] = 2; // room
                    room.tiles.Add(new Vector2Int(x, z));
                }
            }

            rooms.Add(room);
        }
    }


    //tambahan
    public virtual void PlaceItems()
    {
        int count = 0;
        foreach (Room room in rooms)
        {
            if (count >= ItemCount) break;
            Vector2Int pos = room.tiles[Random.Range(0, room.tiles.Count)];
            GameObject item = Instantiate(Item, new Vector3(pos.x * scale, 1f, pos.y * scale), Quaternion.identity);
            count++;
        }
    }

    //diubah
    public virtual void PlaceEnemy()
    {
        int count = 0;
        foreach (Room room in rooms)
        {
            if (count >= EnemyCount) break;
            Vector2Int pos = room.tiles[Random.Range(0, room.tiles.Count)];
            GameObject e = Instantiate(Enemy, new Vector3(pos.x * scale, 1f, pos.y * scale), Quaternion.identity);
            e.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            count++;
        }
    }

    // public virtual void AddRooms(int count, int minSize, int maxSize)
    // {
    //     for (int c = 0; c < count; c++)
    //     {
    //         int startX = Random.Range(3, width - 3);
    //         int startZ = Random.Range(3, depth - 3);
    //         int roomWidth = Random.Range(minSize, maxSize);
    //         int roomDepth = Random.Range(minSize, maxSize);

    //         for (int x = startX; x < width - 3 && x < startX + roomWidth; x++)
    //         {
    //             for (int z = startZ; z < depth - 3 && z < startZ + roomDepth; z++)
    //             {
    //                 map[x, z] = 2;
    //             }
    //         }
    //     }
    // }

    //public virtual void AddRooms(int count, int minSize, int maxSize)
    //{
    //    int roomsCreated = 0;

    //    int[,] sides = new int[,] {
    //        { 1, 0 },             // KIRI
    //        { width - 3, 0 },     // KANAN
    //        { 0, 1 },             // BAWAH
    //        { 0, depth - 3 }      // ATAS
    //    };

    //    for (int s = 0; s < 4; s++) 
    //    {
    //        int startX = sides[s, 0];
    //        int startZ = sides[s, 1];

    //        if (s == 0) // KIRI
    //        {
    //            startX = Random.Range(1, 3);
    //            startZ = Random.Range(1, depth - maxSize - 1);
    //        }
    //        else if (s == 1) // KANAN
    //        {
    //            startX = Random.Range(width - maxSize - 1, width - 2);
    //            startZ = Random.Range(1, depth - maxSize - 1);
    //        }
    //        else if (s == 2) // BAWAH
    //        {
    //            startX = Random.Range(1, width - maxSize - 1);
    //            startZ = Random.Range(1, 3);
    //        }
    //        else if (s == 3) // ATAS
    //        {
    //            startX = Random.Range(1, width - maxSize - 1);
    //            startZ = Random.Range(depth - maxSize - 1, depth - 2);
    //        }

    //        int roomWidth = Random.Range(minSize, maxSize);
    //        int roomDepth = Random.Range(minSize, maxSize);

    //        for (int x = startX; x < startX + roomWidth; x++)
    //            for (int z = startZ; z < startZ + roomDepth; z++)
    //                if (x > 0 && x < width - 1 && z > 0 && z < depth - 1)
    //                    map[x, z] = 2;

    //        roomsCreated++;
    //    }

    //    Debug.Log("Rooms created: " + roomsCreated);
    //}




}
