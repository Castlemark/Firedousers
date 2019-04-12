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
    public int rows;
    public int columns;
    public GameObject genericTile;

    private Transform boardHolder;
    private Transform shadowHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public GameObject[,] grid;
    public LevelGenerator levelGenerator;

    public List<GameObject[,]> levels = new List<GameObject[,]>();

    public void SetupScene(int level, int increment)
    {
        if (levels.Count < level)
        {
            grid = new GameObject[columns, rows];
            levelGenerator = gameObject.AddComponent(typeof(LevelGenerator)) as LevelGenerator;
            boardHolder = new GameObject("Board").transform;
            shadowHolder = new GameObject("Shadow").transform;
            levelGenerator.Initiate(columns, rows);
            player_position = levelGenerator.BoardSetup(grid, genericTile);
            levels.Add(grid);
        }
        else
        {
            grid = levels[level - 1];
            Vector3 current_pos = GameObject.FindGameObjectWithTag("Player").transform.position;
            player_position[0] = (int)current_pos[0] + columns * increment;
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(player_position[0], current_pos[1], 0);
        }

        int[] aux_pos = grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().position;
        int tileset = grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().tileset;
        grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 2, tileset, aux_pos);
        grid[player_position[0], player_position[1] + 1].GetComponent<Tile>().StartFire();

        
        int[] aux_pos_2 = grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().position;
        int tileset_2 = grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().tileset;
        grid[player_position[0], player_position[1] - 1].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 0, tileset_2, aux_pos_2);
        

        int[] aux_pos_3 = grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().position;
        int tileset_3 = grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().tileset;
        grid[player_position[0] - 1, player_position[1]].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item, 1, tileset_3, aux_pos_3);
        
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    public void ExecuteRoutine()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid[i, j].GetComponent<Tile>().IncreaseFire();
                grid[i, j].GetComponent<Tile>().IfSurvivorThenAttemptMove();
            }
        }
    }

    public List<Tile> GetFireTilesWithinRange(int x, int y, int range)
    {
        List<Tile> fireList = new List<Tile>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Tile tile = grid[i, j].GetComponent<Tile>();
                if (tile.HasBurningFire() &&
                    (Math.Abs(tile.position[0] - x) + Math.Abs(tile.position[1] - y)) <= range)
                {
                    fireList.Add(tile);
                }
            }
        }

        return fireList;
    }

    public bool CanMoveTo(int x, int y, int ox, int oy)
    {
        Tile ctile = grid[ox, oy].GetComponent<Tile>();
        Tile gtile = grid[x, y].GetComponent<Tile>();

        bool canMoveTo = gtile.CanPass();

        if((gtile.type == TYPE.wall || gtile.type == TYPE.front_wall) && GameManager.instance.playerHasAxe && (y!=0 && y< rows-2 && x != (GameManager.instance.level - 1)*columns && x != (GameManager.instance.level - 1) * columns + (columns-1)))
        {
            //gtile.ReplaceContained(CONTAINED.none, 0);
            gtile.SetUpTile(TYPE.floor, CONTAINED.none, 0, gtile.tileset, gtile.position);
            if (y != oy)
            {
                //grid[ox, oy + (y-oy)*2].GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
                grid[ox, oy + (y-oy)*2].GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.none, 0, gtile.tileset, gtile.position);
            }
            canMoveTo = true;
            GameManager.instance.playerHasAxe = false;
        }
        ctile.ExecutePostBehaviour();
        gtile.ExecutePreBehaviour();

        

        //if (canMoveTo) updateFire();
        return canMoveTo;
    }
}
