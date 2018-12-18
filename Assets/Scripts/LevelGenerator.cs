using System.Collections;
using System;
using System.Collections.Generic;
using TileEnums;
using Random = UnityEngine.Random;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /***** PROVISIONAL ****************************/
    public GameObject floor; //0
    public GameObject hallway; //- 1
    public GameObject wall; //1 
    public GameObject door; //2
    /*********************************/

    public int[,] board;
    private GameObject genericTile;

    public int hallwayWidth = 2;
    private float hallwaysIterations;
    public float hallwaysProbability = 0.1f;
    public int minAreaRoom = 100;
    public int numDoors = 2;

    private int rows, columns;
    public int[] player_pos_up = { 1, 1 };
    public int[] player_pos_down = { 5, 5 };

    public List<Room> rooms;

    private Transform boardHolder;

    private char direction = 'N'; //Des d'on llancem la fila/columna -> N, E, S, W 

    public void Initiate(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        rooms = new List<Room>();
        board = new int[rows, columns];
        floor = GameObject.Find("Floor");
        wall = GameObject.Find("Wall");

        CreateHallways();
        CreateWalls();
        CreateRooms();
    }

    GameObject IntItemToTile(int item, String state, int[] pos)
    {
        GameObject aux = Instantiate(genericTile, new Vector3(pos[1], pos[0], -1), Quaternion.identity, GameObject.Find("Board").transform);
       
        switch (item)
        {
            case 0:
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0);
                break; 

            case 1:
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall, CONTAINED.none, 0);
                break;

            case 2:
                //aux.GetComponent<Tile>().SetUpTile(TYPE.wall, CONTAINED.none, 0); DOOR
                break;

            default:
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0);
                break;
        }

        return aux;
    }

    public void BoardSetup(GameObject[,]  grid, GameObject genericTile)
    {
        this.genericTile = genericTile;
        GameObject.Find("Player").GetComponent<Player>().SetPosition(player_pos_up[0], player_pos_up[1]);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        switch (GameManager.instance.lastStairs)
        {
            case "up":
                player.transform.position = new Vector3(player_pos_up[0], player_pos_up[1], 0);

                break;
            case "down":
                player.transform.position = new Vector3(player_pos_down[0], player_pos_down[1], 0);
                break;
        }
        
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                //String state = heatmap.rows[row].items[column];
                int[] position = { row, col };
                String state = "0";
                GameObject tile = IntItemToTile(board[row, col], state, position);
                grid[col, row] = tile;
            }
        }
    }

    /****************************** ROOMS ***********************************/
    private void PaintRoom(Room room, int num)
    {
        for (int x = room.x1; x <= room.x2; x++)
        {
            for (int y = room.y1; y <= room.y2; y++)
            {
                board[x, y] = num;
            }
        }
    }

    private void CreateRooms()
    {
        DefineRooms();
        SubdivideRooms();
        PaintBoard();
    }

    private void DefineRooms()
    {
        int x1, y1, x2 = 0, y2 = 0;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == 0)
                {
                    x1 = x; y1 = y;

                    for (int col = x1; col < columns; col++)
                    {
                        if (board[col, y] != 0)
                        {
                            x2 = col - 1;
                            break;
                        }

                        if (col == columns - 1)
                        {
                            x2 = col;
                            break;
                        }
                    }

                    for (int row = y1; row < rows; row++)
                    {
                        if (board[x2, row] != 0)
                        {
                            y2 = row - 1;
                            break;
                        }
                        if (row == rows - 1)
                        {
                            y2 = row;
                        }
                    }

                    Room newRoom = new Room(x1, x2, y1, y2, minAreaRoom, numDoors);
                    rooms.Add(newRoom);
                    PaintRoom(newRoom, 5);
                }
            }
        }
    }

    private void SubdivideRooms()
    {
        List<Room> subdivisions = new List<Room>();

        while (roomsAreDivisibles())
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                List<Room> subrooms = rooms[i].Subdivide();

                if (subrooms.Count > 0) subdivisions.AddRange(subrooms);
                else subdivisions.Add(rooms[i]);
            }

            rooms = subdivisions;
            subdivisions = new List<Room>();
        }

        foreach (Room room in rooms) PaintRoom(room, 2);
    }

    private bool roomsAreDivisibles()
    {
        for (int i = 0; i < rooms.Count; i++) if (rooms[i].isDivisible()) return true;

        return false;
    }
    /***************************************************************************/




    /****************************** WALLS ***********************************/
    public void CreateWalls()
    {
        CreateExternalWalls();
        CreateVerticalWalls();
        CreateHorizontalWalls();
    }

    private void CreateExternalWalls()
    {
        //Horizontal walls
        for (int x = 0; x < columns; x++)
        {
            board[x, 0] = 1;
            board[x, rows - 1] = 1;
        }

        //Vertical walls
        for (int y = 0; y < rows; y++)
        {
            board[0, y] = 1;
            board[columns - 1, y] = 1;
        }
    }

    private void CreateVerticalWalls()
    {
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == 0 & board[x + 1, y] == -1) board[x, y] = 1;
            }
        }

        for (int x = columns - 1; x > 0; x--)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == 0 & board[x - 1, y] == -1) board[x, y] = 1;
            }
        }
    }

    private void CreateHorizontalWalls()
    {
        for (int y = 1; y < rows - 1; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (board[x, y] == 0 & board[x, y + 1] == -1) board[x, y] = 1;
            }
        }

        for (int y = rows - 1; y > 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                if (board[x, y] == 0 & board[x, y - 1] == -1) board[x, y] = 1;
            }
        }
    }

    private void PaintBoard()
    {
        //Paint internal walls
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == 5) board[x, y] = 1;
                if (board[x, y] == 2) board[x, y] = 0;
            }
        }

        //Paint doors
        foreach (Room room in rooms)
        {
            for (int i = 0; i < room.numDoors; i++)
            {
                RoomDoor door = room.AssignDoor();

                while (door.x == 0 || door.x == columns - 1 || door.y == 0 || door.y == columns - 1)
                {
                    door = room.AssignDoor();
                }

                room.doors.Add(door);
                board[door.x, door.y] = 2;
            }
        }
    }
    /***************************************************************************/



    /****************************** HALLWAYS ***********************************/
    public void CreateHallways()
    {
        hallwaysIterations = (columns + rows) / (hallwayWidth + 2) * hallwaysProbability;
        SetRowOrColumn(0, columns, 0, rows);
        direction = 'W';

        for (int i = 1; i < hallwaysIterations; i++)
        {
            SetRowOrColumn(0, columns, 0, rows);
        }
    }

    // Returns true if completes without colliding
    private bool SetCell(int x, int y)
    {
        if (board[x, y] == -1) return false;
        else board[x, y] = -1;
        return true;
    }

    //"Throws" hallways from North or West
    private void SetRowOrColumn(int x1, int x2, int y1, int y2)
    {
        bool completed = true;
        switch (direction)
        {
            case 'N':
                int column = Random.Range(x1, x2 - hallwayWidth);
                for (int y = y2 - 1; y >= y1; y--)
                {
                    for (int w = 0; w < hallwayWidth; w++)
                    {
                        completed &= SetCell(column + w, y);
                    }

                    if (!completed)
                    {
                        direction = 'S';
                        SetRowOrColumn(x1, x2, y1, y2);
                        break;
                    }
                }
                break;

            case 'W':
                int row = Random.Range(x1, y2 - hallwayWidth);
                for (int x = x1; x < x2; x++)
                {
                    for (int w = 0; w < hallwayWidth; w++)
                    {
                        completed &= SetCell(x, row + w);
                    }

                    if (!completed)
                    {
                        direction = 'E';
                        SetRowOrColumn(x1, x2, y1, y2);
                        break;
                    }
                }
                break;

            case 'S':
                int c = Random.Range(x1, x2 - hallwayWidth);
                for (int y = y1; y < y2; y++)
                {
                    for (int w = 0; w < hallwayWidth; w++)
                    {
                        completed &= SetCell(c + w, y);
                    }
                    if (!completed)
                    {
                        direction = 'W';
                        SetRowOrColumn(x1, x2, y1, y2);
                        break;
                    }
                }
                break;

            case 'E':
                int r = Random.Range(y1, y2 - hallwayWidth);
                for (int x = x2 - 1; x >= x1; x--)
                {
                    for (int w = 0; w < hallwayWidth; w++)
                    {
                        completed &= SetCell(x, r + w);
                    }
                    if (!completed)
                    {
                        direction = 'N';
                        break;
                    }
                }
                break;
        }
    }
    /***************************************************************************/


    //Sets up the outer walls and floor (background) of the game board.
    /*public void BoardSetup()
    {
        
    }*/
}