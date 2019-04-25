using System.Collections;
using System;
using System.Collections.Generic;
using TileEnums;
using Random = UnityEngine.Random;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int[,] board;
    public int[,] board_rooms;

    private GameObject genericTile;

    public int hallwayWidth = 5;
    private float hallwaysIterations;

    public float hallwaysProbability = 0.5f;
    public float survivorsProbability = 0.05f;
    public int hammersQuantity = 10;
    public int healthQuantity = 5;
    public int hosesQuantity = 5;
    public int safepointsQuantity = 7;
    public int fireQuantity = 5;

    public int minAreaRoom = 50;
    public int numDoors = 1;

    public int rows, columns;

    public List<Room> rooms;

    private Transform boardHolder;

    private char direction = 'N'; //Des d'on llancem la fila/columna -> N, E, S, W 

    private GameObject cube; //el que fa les ombres
    private GameObject smallCube; //el que fa les ombres de les portes de perfil
    private GameObject bigCube; //el que fa les ombres de portes frontals

    public void Initiate(int columns, int rows)
    {
        this.rows = rows;
        this.columns = columns;
        rooms = new List<Room>();
        board = new int[columns, rows];
        board_rooms = new int[columns, rows];
        cube = (GameObject)Resources.Load("Prefabs/cube", typeof(GameObject));
        smallCube = (GameObject)Resources.Load("Prefabs/smallCube", typeof(GameObject));
        bigCube = (GameObject)Resources.Load("Prefabs/bigCube", typeof(GameObject));

        CreateHallways();
        CreateExternalWalls();
        CreateRooms();
        CreateSafePoints();
        CreateCivils();
        CreateFire();
        CreateStairs();
        CreateItems();
    }

    private bool cellIsEmpty(int x, int y, float probability = 1)
    {
        if (Random.Range(0, 1f) <= probability && (board[x, y] == 0 || board[x, y] == -1) && board[x, y] != 3) return true;
        return false;
    }

    private void CreateCivils()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (cellIsEmpty(x, y, survivorsProbability) && board[x, y - 1] != 2 && board[x, y - 1] != 3) board[x, y] = 6;
            }
        }
    }

    private void CreateItems()
    {
        for (int type = 10; type <= 12; type++)
        {
            int quantity = 0;

            switch (type)
            {
                case 10:
                    quantity = hammersQuantity;
                    break;

                case 11:
                    quantity = healthQuantity;
                    break;

                case 12:
                    quantity = hosesQuantity;
                    break;
            }

            List<Vector3> empty_cells = new List<Vector3>();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (cellIsEmpty(x, y) && board[x, y - 1] != 2 && board[x, y - 1] != 3) empty_cells.Add(new Vector3(x, y, 0));
                }
            }

            if (empty_cells.Count > 0)
            {
                int size = empty_cells.Count;
                if (quantity < size) size = quantity;

                for (int i = 0; i < size; i++)
                {
                    int randomNumber = Random.Range(0, empty_cells.Count);
                    board[(int)empty_cells[randomNumber].x, (int)empty_cells[randomNumber].y] = type;
                    empty_cells.RemoveAt(randomNumber);
                }
            }
        }
    }

    private void CreateFire()
    {
        int num_fire_focus = 0;

        while (num_fire_focus < fireQuantity)
        {
            int x, y = 0;
            do
            {
                x = Random.Range(0, columns - 1);
                y = Random.Range(0, rows - 1);
            } while (!(cellIsEmpty(x, y, 1) && board[x, y - 1] != 2 && board[x, y - 1] != 3 && board_rooms[x, y] != 0));

            board[x, y] = 7;
            num_fire_focus++;
        }
    }

    private void CreateStairs()
    {
        List<Vector3> stairs = GameManager.instance.stairsUpPositions;

        if (GameManager.instance.level > 1)
        {
            Vector3 last_stairs_up = stairs[stairs.Count - 1];
            board[(int)last_stairs_up.x, (int)last_stairs_up.y] = 9;
        }

        int x = 0, y = 0;

        do
        {
            int edge = Random.Range(0, 3);

            switch (edge)
            {
                case 0:
                    y = rows - 3;
                    x = Random.Range(1, columns - 2);
                    break;

                case 1:
                    x = columns - 2;
                    y = Random.Range(1, rows - 3);
                    break;

                case 2:
                    y = 1;
                    x = Random.Range(1, columns - 2);
                    break;

                case 3:
                    x = 1;
                    y = Random.Range(1, rows - 3);
                    break;
            }
        } while (board[x, y] != -1);

        board[x, y] = 8;
        stairs.Add(new Vector3(x, y, 0));
    }

    private void CreateSafePoints()
    {
        int edge = 0;
        bool possible;

        for (int s = 0; s < safepointsQuantity; s++)
        {
            do
            {
                possible = PlaceSafePointInEdge(edge);
            } while (!possible);

            edge++;
            if (edge == 4) edge = 0;
        }
    }

    private bool PlaceSafePointInEdge(int edge)
    {
        int x = 0, y = 0;

        switch (edge)
        {
            case 0:
                x = Random.Range(1, columns - 2);
                y = rows - 3;
                if (board[x, y] != -1) return false;
                break;

            case 1:
                y = Random.Range(1, rows - 3);
                x = columns - 2;
                if (board[x, y] != -1) return false;
                break;

            case 2:
                x = Random.Range(1, columns - 2);
                y = 1;
                if (board[x, y] != -1) return false;
                break;

            case 3:
                y = Random.Range(1, rows - 3);
                x = 1;
                if (board[x, y] != -1) return false;
                break;

        }

        board[x, y] = 5;
        return true;
    }


    private String GetSpriteSuffix(int[] around)
    {
        bool b = false, t = false, r = false, l = false; //Obstacles al voltant

        if (around[0] == 1) t = true;
        if (around[1] == 1) r = true;
        if (around[2] == 1) b = true;
        if (around[3] == 1) l = true;

        if (b && around[0] == 2 || around[1] == 0 && around[3] == 0 && b && !t || around[1] == 0 && b && !t && !l) return "top";
        if (l && around[1] == 0) return "right";
        if (t && around[2] == 0) return "bottom";
        if (r && around[3] == 0 && !b) return "left";

        if (b && l && !t && !r || l && !t && !r && around[2] == 0 || b && !t && !r && around[3] == 0) return "tr";
        if (b && r && !l && !t || b && !t && !l && around[1] == 0) return "tl";
        if (t && l && !b && !r || l && !r && !b && around[0] == 2 || around[3] == 2 && t && !b && !r) return "br";
        if (t && r && !b && !l || r && !l && !b && around[0] == 2 || around[1] == 2 && t && !b && !l) return "bl";

        if (b && around[0] == 2) return "top";
        if (l && around[1] == 0) return "right";
        if (t && around[2] == 2) return "none";
        if (r && around[3] == 0 && !b) return "left";

        if (t && b && r) return "rc";
        if (t && b && l) return "lc";
        if (r && l && t) return "tc";
        if (r && l && b) return "bc";

        if (t && b) return "v";
        if (r && l) return "h";

        return "none";
    }

    private int SetTileToWall(int[] around, Sprite[] sprites)
    {
        int size = sprites.Length;
        for (int i = 0; i < size; i++)
        {
            String prefix = sprites[i].name.Substring(0, 7);
            String suffix = sprites[i].name.Replace(prefix, "");
            if (suffix == GetSpriteSuffix(around)) return i;
        }

        return 0;
    }

    GameObject IntItemToTile(int item, String state, int[] pos, int[] around, int room_tileset)
    {
        int level = GameManager.instance.level - 1;
        GameObject aux = Instantiate(genericTile, new Vector3(pos[0] + level * columns, pos[1], -1), Quaternion.identity, GameObject.Find("Board").transform);

        bool possible = true;
        switch (item)
        {
            case 1: //wall
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall, CONTAINED.none, 0, room_tileset, pos);
                Sprite[] sprites = aux.GetComponent<Tile>().getRoomImages(0, aux.GetComponent<Tile>().wall_images);
                int sprite = SetTileToWall(around, sprites);
                aux.GetComponent<Tile>().ChangeTypeSpriteTo(sprite);
                Instantiate(cube, new Vector3(pos[0] + level * columns, pos[1], 0.5f), Quaternion.identity, GameObject.Find("Shadow").transform);
                break;

            case 2: //door
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.door, 0, room_tileset, pos);

                if (pos[1] > 1 && board[pos[0], pos[1] - 2] == 1 && around[2] == 1)
                {
                    Instantiate(smallCube, new Vector3(pos[0] + level * columns - 0.4f, pos[1], 0.5f), Quaternion.identity, GameObject.Find("Shadow").transform);
                }

                if (around[1] == 4 && around[3] == 4)
                {
                    Instantiate(bigCube, new Vector3(pos[0] + level * columns, pos[1] + 0.55f, 0.5f), Quaternion.identity, GameObject.Find("Shadow").transform);
                }

                break;

            case 3: //furniture
                possible &= !(around[0] == 2 || around[1] == 2 || around[2] == 2 || around[3] == 2 || pos[1] > 1 && board[pos[0], pos[1] - 2] == 2);

                if (possible)
                    aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.furniture, 0, room_tileset, pos);
                else
                    aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0, room_tileset, pos);
                break;

            case 4: //front_wall
                aux.GetComponent<Tile>().SetUpTile(TYPE.front_wall, CONTAINED.none, 0, room_tileset, pos);
                break;

            case 5: //safepoints
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.safepoint, 0, room_tileset, pos);
                break;

            case 6: //survivors
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.survivor, Random.Range(0, 4), room_tileset, pos);
                break;

            case 7: //fire
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0, room_tileset, pos);
                aux.GetComponent<Tile>().StartFire();
                break;

            case 8: //stairs up
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_up, CONTAINED.none, 0, room_tileset, pos);
                break;

            case 9: //stairs down
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_down, CONTAINED.none, 0, room_tileset, pos);
                break;

            case 10: //hammers
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 0, room_tileset, pos);
                break;

            case 11: //health
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 1, room_tileset, pos);
                break;

            case 12: //hoses
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 2, room_tileset, pos);
                break;

            default: //floor
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0, room_tileset, pos);
                break;
        }

        return aux;
    }

    private int[] SetupPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        int[] player_pos = new int[2];

        if (GameManager.instance.boardScript.levels.Count == 0)
        {
            do
            {
                player_pos[0] = Random.Range(1, columns);
                player_pos[1] = Random.Range(1, rows);
            } while (board[player_pos[0], player_pos[1]] != 0);

            GameObject.Find("Player").GetComponent<Player>().SetPosition(player_pos[0], player_pos[1]);
        }
        else
        {
            int level = GameManager.instance.level - 1;
            player_pos[0] = (int)player.transform.position[0] + columns;
            player_pos[1] = (int)player.transform.position[1];
        }

        player.transform.position = new Vector3(player_pos[0], player_pos[1], 0);
        return player_pos;
    }

    private int[] getAroundObjects(GameObject[,] grid, int[] position)
    {
        int col = position[0];
        int row = position[1];

        int[] around = new int[8];

        if (row == grid.GetLength(1) - 1) around[0] = -5;
        else around[0] = board[col, row + 1];

        if (col == grid.GetLength(0) - 1) around[1] = -5;
        else around[1] = board[col + 1, row];

        if (row == 0) around[2] = -5;
        else around[2] = board[col, row - 1];

        if (col == 0) around[3] = -5;
        else around[3] = board[col - 1, row];

        if (col == grid.GetLength(0) - 1 || row == grid.GetLength(1) - 1) around[4] = -5;
        else around[4] = board[col + 1, row + 1];

        if (row == 0 || col == grid.GetLength(0) - 1) around[5] = -5;
        else around[5] = board[col + 1, row - 1];

        if (row == 0 || col == 0) around[6] = -5;
        else around[6] = board[col - 1, row - 1];

        if (row == grid.GetLength(1) - 1 || col == 0) around[7] = -5;
        else around[7] = board[col - 1, row + 1];

        return around;
    }

    public int[] BoardSetup(GameObject[,] grid, GameObject genericTile)
    {
        this.genericTile = genericTile;

        int level = GameManager.instance.level;

        for (int col = 0; col < grid.GetLength(0); col++)
        {
            for (int row = 0; row < grid.GetLength(1); row++)
            {
                int[] position = { col, row };
                GameObject tile = IntItemToTile(board[col, row], "0", position, getAroundObjects(grid, position), board_rooms[col, row]);
                grid[col, row] = tile;
            }
        }

        return SetupPlayerPosition();
    }

    /****************************** ROOMS ***********************************/
    private void PaintRoom(Room room, int num)
    {
        for (int x = room.x1; x <= room.x2; x++)
        {
            for (int y = room.y1; y <= room.y2; y++)
            {
                board[x, y] = num;
                board_rooms[x, y] = room.numTile;
                if (y > 0) board_rooms[x, y - 1] = room.numTile;
                if (x > 0) board_rooms[x - 1, y] = room.numTile;
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
                    if (newRoom.h > 2)
                    {
                        rooms.Add(newRoom);
                        PaintRoom(newRoom, 5);
                    }
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
    private void CreateExternalWalls()
    {
        //Vertical walls
        for (int x = 0; x < columns; x++)
        {
            board[x, 0] = 1;
            board[x, rows - 1] = 1;
        }

        //Horizontal walls
        for (int y = 0; y < rows; y++)
        {
            board[0, y] = 1;
            board[columns - 1, y] = 1;
        }
    }

    private void PaintBoard()
    {
        //Floor 
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (board[x, y] == 2) board[x, y] = 0;
            }
        }

        //walls
        foreach (Room room in rooms)
        {
            for (int x = room.x1 - 1; x <= room.x2 + 1; x++)
            {
                board[x, room.y1 - 1] = 1;
                board[x, room.y2 + 1] = 1;
                board[x, room.y2] = 4;
            }

            for (int y = room.y1; y <= room.y2; y++)
            {
                board[room.x1 - 1, y] = 1;
                board[room.x2 + 1, y] = 1;
            }

            for (int x = room.x1 - 1; x <= room.x2 + 1; x++)
            {
                if (room.y1 > 1 && board[x, room.y1 - 2] != 1) board[x, room.y1 - 2] = 4;
            }
        }

        int level = GameManager.instance.level - 1;

        //Paint room objects
        foreach (Room room in rooms)
        {
            foreach (RoomObject door in room.doors)
            {
                if (door.x > 0 && door.x < columns - 1 && door.y > 0 && door.y < rows - 2)
                {
                    bool nextToDoor = false;

                    if (door.y > 0) nextToDoor |= board[door.x, door.y - 1] == 2;
                    if (door.y < rows - 1) nextToDoor |= board[door.x, door.y + 1] == 2;
                    if (door.x < columns - 2) nextToDoor |= board[door.x + 2, door.y] == 2 || board[door.x + 1, door.y] == 2;
                    if (door.x > 1) nextToDoor |= board[door.x - 2, door.y] == 2 || board[door.x - 1, door.y] == 2;

                    if (!(nextToDoor || board[door.x, door.y] == 2))
                    {
                        if (board[door.x - 1, door.y] == 4 && board[door.x + 1, door.y] == 4)
                        {
                            board[door.x, door.y + 1] = 0;
                        }
                        else
                        {
                            board[door.x, door.y - 1] = 1;
                        }

                        board[door.x, door.y] = 2;
                    }
                }
            }

            for (int r = 0; r < room.furniture.Count; r++)
            {
                if (room.furniture[r].x > 1 && board[room.furniture[r].x - 2, room.furniture[r].y] != 2)
                {
                    board[room.furniture[r].x, room.furniture[r].y] = 3;
                    board_rooms[room.furniture[r].x, room.furniture[r].y] = room.numTile;
                }
            }
        }

        for (int x = 0; x < columns; x++)
        {
            if (board[x, rows - 2] != 1) board[x, rows - 2] = 4;
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
        int level = GameManager.instance.level;
        bool avoid_stairs = true;
        if (level == 1) avoid_stairs = false;
        List<Vector3> stairs = GameManager.instance.stairsUpPositions;

        bool completed = true;
        int tries = 0;
        switch (direction)
        {
            case 'N':
                int column = Random.Range(x1, x2 - hallwayWidth);
                if (level > 1)
                {
                    while (column == stairs[level - 2].x)
                    {
                        column = Random.Range(x1, x2 - hallwayWidth);
                        tries++;
                        if (tries > columns) break;
                    }
                }
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
                int row = Random.Range(y1, y2 - hallwayWidth - 1);
                if (level > 1)
                {
                    while (row == stairs[level - 2].y || row == stairs[level - 2].y - 1)
                    {
                        row = Random.Range(y1, y2 - hallwayWidth - 1);
                        tries++;
                        if (tries > rows) break;
                    }
                }
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
                if (level > 1)
                {
                    while (c == stairs[level - 2].x - 1)
                    {
                        c = Random.Range(x1, x2 - hallwayWidth);
                        tries++;
                        if (tries > columns) break;
                    }
                }
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
                int r = Random.Range(y1, y2 - hallwayWidth - 1);
                if (level > 1)
                {
                    while (r == stairs[level - 2].y)
                    {
                        r = Random.Range(y1, y2 - hallwayWidth - 1);
                        tries++;
                        if (tries > rows) break;
                    }
                }
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
}