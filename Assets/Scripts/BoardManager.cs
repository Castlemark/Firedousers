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
    public int rows = 64;
    public int columns = 64;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public GameObject[,] grid;
    public LevelGenerator levelGenerator;

    public void SetupScene(int level)
    {
        levelGenerator = gameObject.AddComponent(typeof(LevelGenerator)) as LevelGenerator;
        boardHolder = new GameObject("Board").transform;
        levelGenerator.Initiate(64, 64);
        levelGenerator.BoardSetup(grid);

        grid = new GameObject[columns, rows];
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
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

    public bool CanMoveTo(int x, int y)
    {
        Tile tile = grid[x, y].GetComponent<Tile>();

        bool canMoveTo = tile.CanPass();
        tile.ExecuteBehaviour();

        return canMoveTo;
    }

    /*void LoadBoard(int level)
    {
        /*String fileName = "first_floor";

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

        //GameObject.Find("Player").GetComponent<Player>().SetPosition(player_pos_up[0], player_pos_up[1]);
    }*/

    /*
    void LayoutObjects()
    {
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

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                //String state = heatmap.rows[row].items[column];
                int[] position = { row, col };
                String state = "0";
                Debug.Log(grid[col, row]);
                //GameObject tile = StringItemToTile(grid[row, col], state, position);
                grid[col, row] = tile;
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
    */
}
