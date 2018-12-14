using System.Collections;
using System;
using System.Collections.Generic;
using TileEnums;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;

        }
    }

    [Serializable]
    public class Row
    {
        public int row;
        public String[] items;
    }

    public class Board
    {
        public int[] player_pos_up;
        public int[] player_pos_down;
        public int columns;
        public Row[] rows;
    }

    public Board board;
    public Board heatmap;
    public int rows = 21;

    public GameObject genericTile;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public GameObject[,] grid;

    void LoadJSON(int level)
    {
        String fileName = "first_floor";

        switch (level)
        {
            case 1:
                fileName = "garage";
                break;

            case 2:
                fileName = "first_floor";
                break;

            case 3:
                fileName = "second_floor";
                break;
        }

        TextAsset jsonBoard = (TextAsset)Resources.Load("Boards/" + fileName, typeof(TextAsset));
        TextAsset jsonHeatmap = (TextAsset)Resources.Load("Boards/" + fileName + "_heatmap", typeof(TextAsset));

        board = JsonUtility.FromJson<Board>(jsonBoard.text);
        heatmap = JsonUtility.FromJson<Board>(jsonHeatmap.text);

        GameObject.Find("Player").GetComponent<Player>().SetPosition(board.player_pos_up[0], board.player_pos_up[1]);
    }

    void InitialiseList()
    {
        grid = new GameObject[board.columns, board.rows.Length];
        gridPositions.Clear();

        for (int x = 1; x < board.columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
    }

    GameObject StringItemToTile(String item, String state, int[] pos)
    {
        GameObject aux = Instantiate(genericTile,  new Vector3(pos[1], pos[0], -1),Quaternion.identity,GameObject.Find("Board").transform);
        
        switch (item)
        {
            case "w_h":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;
            case "w_v":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;
            case "w_rt":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;
            case "w_rb":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;
            case "w_lb":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;
            case "w_lt":
                aux.GetComponent<Tile>().SetUpTile(TYPE.wall,CONTAINED.none,0);
                break;

            case "ww_h":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
            case "ww_v":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
            case "ww_rt":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
            case "ww_rb":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
            case "ww_lb":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
            case "ww_lt":
                aux.GetComponent<Tile>().SetUpTile(TYPE.breakable_wall,CONTAINED.none,0);
                break;
                
            case "s_h_u":
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_up,CONTAINED.none,0);
                break;
            case "s_h_d":
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_down,CONTAINED.none,0);
                break;
            case "s_v_u":
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_up,CONTAINED.none,0);
                break;
            case "s_v_d":
                aux.GetComponent<Tile>().SetUpTile(TYPE.stair_down,CONTAINED.none,0);
                break;

            case "f":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.furniture,0);
                break;

            case "d":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.door,0);
                break;

            case "d_l":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.door,0);
                break;

            case "v":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.survivor,0);
                break;
            case "s":
                aux.GetComponent<Tile>().SetUpTile(TYPE.safepoint,CONTAINED.none,0);
                break;
            case "i":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.item,0);
                break;
            case "#":
                aux.GetComponent<Tile>().SetUpTile(TYPE.floor,CONTAINED.none,0);
                break;

            default:
                return null;
        }
        return aux;
    }

    void LayoutObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        switch (GameManager.instance.lastStairs)
        {
            case "up": 
                player.transform.position = new Vector3 (board.player_pos_up[0], board.player_pos_up[1], 0); 
                
                break;
            case "down": 
                player.transform.position = new Vector3 (board.player_pos_down[0], board.player_pos_down[1], 0); 
                break;
        }        

        foreach (Row row in board.rows)
        {
            int column = 0;
            foreach (String item in row.items)
            {
                String state = heatmap.rows[row.row].items[column];
                int[] position = { row.row, column };
                GameObject tile = StringItemToTile(item, state, position);
                grid[column, row.row] = tile;
                column++;
            }
        }
        OrientGrid();

    }

    private void OrientGrid()
    {
        int length = grid.GetLength(0);
        int width = grid.GetLength(1);

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {

            }
        }
    }

    public void SetupScene(int level)
    {
        LoadJSON(level);
        BoardSetup();
        InitialiseList();
        LayoutObjects();
    }

    public bool CanMoveTo(int x, int y)
    {
        Tile tile = grid[x, y].GetComponent<Tile>();

        bool canMoveTo = tile.CanPass();
        tile.ExecuteBehaviour();

        return canMoveTo;
    }
}
