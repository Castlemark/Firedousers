using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject
{
    public int x, y;

    public RoomObject(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Room
{
    private int minArea;
    private int min_height_width = 5;
    public int x1, x2, y1, y2, w, h;

    public List<RoomObject> doors = new List<RoomObject>();
    public List<RoomObject> safePoints = new List<RoomObject>();
    public List<RoomObject> furniture = new List<RoomObject>();
    public int numDoors;
    public int numTile;
    private int area;

    private float furnitureProbability = 0.5f;
    private float safePointsProbability = 0.3f;
    private float divisibleProbability = 0.5f;

    public Room(int x1, int x2, int y1, int y2, int minArea, int numDoors)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.minArea = minArea;
        this.numDoors = numDoors;
        this.numTile = Random.Range(1, 4);

        w = x2 - x1 + 1;
        h = y2 - y1 + 1;
        area = w * h;
        AssignFurniture();
        AssignDoors();
    }

    public void AssignFurniture()
    {
        if (w > 2 && h > 3)
        {
            for (int x = x1; x <= x2; x++)
            {
                if (Random.Range(0f, 1f) < furnitureProbability) furniture.Add(new RoomObject(x, y1));
                if (Random.Range(0f, 1f) < furnitureProbability) furniture.Add(new RoomObject(x, y2 - 1));
            }

            for (int y = y1; y < y2; y++)
            {
                if (Random.Range(0f, 1f) < furnitureProbability) furniture.Add(new RoomObject(x2, y));
                if (Random.Range(0f, 1f) < furnitureProbability) furniture.Add(new RoomObject(x1, y));
            }
        }
    }

    public void AssignDoors()
    {
        //In vertical walls
        if (h > 2)
        {
            doors.Add(new RoomObject(x2 + 1, Random.Range(y1 + 1, y2 - 3))); //right
            doors.Add(new RoomObject(x1 - 1, Random.Range(y1 + 1, y2 - 3))); //left
        }

        //In horizontal walls
        if (w > 2)
        {
            doors.Add(new RoomObject(Random.Range(x1 + 1, x2 - 1), y1 - 2)); //bottom
            doors.Add(new RoomObject(Random.Range(x1 + 1, x2 - 1), y2)); //top
        }
    }

    public bool isDivisible()
    {
        float probability = Random.Range(0f, 1f);
        return (probability < divisibleProbability && area > minArea && (w > 4 && h > 1 || h > 4 && w > 1) && w > min_height_width && h > min_height_width);
    }

    public List<Room> Subdivide()
    {
        List<Room> subrooms = new List<Room>();

        if (isDivisible())
        {
            if (w > h && w > 2)
            {
                int col = Random.Range(x1 + 1, x2 - 1);
                subrooms.Add(new Room(x1, col - 1, y1, y2, minArea, numDoors));
                subrooms.Add(new Room(col + 1, x2, y1, y2, minArea, numDoors));
            }
            else if (h > 2)
            {
                int row = Random.Range(y1 + 1, y2 - 1);
                subrooms.Add(new Room(x1, x2, y1, row - 1, minArea, numDoors));
                subrooms.Add(new Room(x1, x2, row + 1, y2, minArea, numDoors));
            }
        }

        return subrooms;
    }
}
