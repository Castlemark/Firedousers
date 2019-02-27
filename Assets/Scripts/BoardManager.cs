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

    private int[] player_position;
    public Board heatmap;
    public int rows = 32;
    public int columns = 32;
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
        levelGenerator.Initiate(columns, rows);
        player_position = levelGenerator.BoardSetup(grid, genericTile);

        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }

        int[] aux_pos = grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().position;
        int tileset = grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().tileset;
        grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0, tileset, aux_pos);
        grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().StartFire();

        int[] aux_pos_2 = grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().position;
        int tileset_2 = grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().tileset;
        grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.survivor, 0, tileset_2, aux_pos_2);

        int[] aux_pos_3 = grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().position;
        int tileset_3 = grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().tileset;
        grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.safepoint, 0, 3, aux_pos_3);
    }

    public void updateFire()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid[i, j].GetComponent<Tile>().IncreaseFire();
            }
        }
    }

    public bool CanMoveTo(int x, int y, int ox, int oy)
    {
        Tile ctile = grid[ox, oy].GetComponent<Tile>();
        Tile gtile = grid[x, y].GetComponent<Tile>();

        bool canMoveTo = gtile.CanPass();
        ctile.ExecutePostBehaviour();
        gtile.ExecutePreBehaviour();

        if (canMoveTo) updateFire();

        return canMoveTo;
    }
}
