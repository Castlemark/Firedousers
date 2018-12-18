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
    private int rows = 32;
    private int columns = 32;
    public GameObject genericTile;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public GameObject[,] grid;
    public LevelGenerator levelGenerator;

    public void SetupScene(int level)
    {
        grid = new GameObject[columns, rows];
        levelGenerator = gameObject.AddComponent(typeof(LevelGenerator)) as LevelGenerator;
        boardHolder = new GameObject("Board").transform;
        levelGenerator.Initiate(64, 64);
        levelGenerator.BoardSetup(grid, genericTile);

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
        Debug.Log(grid[0, 0]);
        Debug.Log(grid.GetLength(1));
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
}
