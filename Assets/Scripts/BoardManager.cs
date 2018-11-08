using System.Collections;
using System;
using System.Collections.Generic;
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
        public int[] player_pos;
        public int columns;
        public Row[] rows;
    }

    public Board board;
    public Board heatmap;
    public int rows = 21;

    public GameObject[] wallTiles; //H, V, RT, RB, LB, LT
    public GameObject[] weakWallTiles; //H, V, RT, RB, LB, LT
    public GameObject[] stairTiles; //H, V

    public GameObject[] survivorTiles;
    public GameObject[] itemTiles;
    public GameObject[] floorTiles;
    public GameObject[] furnitureTiles;
    public GameObject[] doorTiles;
    public GameObject[] safePointTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

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
        Debug.Log("change to: " + fileName);
        heatmap = JsonUtility.FromJson<Board>(jsonHeatmap.text);
    }

    void InitialiseList()
    {
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
        for (int x = -1; x < board.columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    GameObject StringItemToTile(String item, String state)
    {
        switch (item)
        {
            case "w_h":
                return wallTiles[0];
            case "w_v":
                return wallTiles[1];
            case "w_rt":
                return wallTiles[2];
            case "w_rb":
                return wallTiles[3];
            case "w_lb":
                return wallTiles[4];
            case "w_lt":
                return wallTiles[5];

            case "ww_h":
                return weakWallTiles[0];
            case "ww_v":
                return weakWallTiles[1];
            case "ww_rt":
                return weakWallTiles[2];
            case "ww_rb":
                return weakWallTiles[3];
            case "ww_lb":
                return weakWallTiles[4];
            case "ww_lt":
                return weakWallTiles[5];

            case "s_h":
                GameObject s_h = stairTiles[0];
                s_h.tag = "Untagged";
                return s_h;

            case "s_v":
                GameObject s_v = stairTiles[1];
                s_v.tag = "Untagged";
                return s_v;

            case "s_h_u":
                GameObject s_h_u = stairTiles[0];
                s_h_u.tag = "StairsUp";
                return s_h_u;

            case "s_h_d":
                GameObject s_h_d = stairTiles[0];
                s_h_d.tag = "StairsDown";
                return s_h_d;

            case "s_v_u":
                GameObject s_v_u = stairTiles[1];
                s_v_u.tag = "StairsUp";
                return s_v_u;

            case "s_v_d":
                GameObject s_v_d = stairTiles[1];
                s_v_d.tag = "StairsDown";
                return s_v_d;

            case "f":
                return furnitureTiles[Random.Range(0, furnitureTiles.Length)];

            case "d":
                GameObject door = doorTiles[0];
                door.tag = "Door";
                return doorTiles[0];

            case "d_l":
                GameObject locked_door = doorTiles[0];
                locked_door.tag = "LockedDoor";
                return doorTiles[0];

            case "v":
                return survivorTiles[Random.Range(0, survivorTiles.Length)];
            case "s":
                return safePointTiles[Random.Range(0, safePointTiles.Length)];
            case "i":
                return itemTiles[Random.Range(0, itemTiles.Length)];
            case "#":
                GameObject floor = floorTiles[Random.Range(0, floorTiles.Length)];
                floor.transform.GetChild(0).GetComponent<FireController>().ChangeState(Int32.Parse(state));
                return floor;

            default:
                return null;
        }
    }

    void LayoutObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3 (board.player_pos[0], board.player_pos[1], 0);

        foreach (Row row in board.rows)
        {
            int column = 0;
            foreach (String item in row.items)
            {
                String state = heatmap.rows[row.row].items[column];
                int[] position = { row.row, column };
                //Debug.Log( item + " ; " + state  + " : " + position[0] + ";" + position[1]);
                GameObject tile = StringItemToTile(item, state);
                LayoutObjectAtPosition(tile, position);
                column++;
            }
        }

    }

    void LayoutObjectAtPosition(GameObject tileChoice, int[] pos)
    {
        Instantiate(tileChoice, new Vector3(pos[1], rows - pos[0], -1), Quaternion.identity).transform.SetParent(GameObject.Find("Board").transform);
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); // escollim random quants objectes hi hauran daquell tipus a l'escena
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            instance.transform.SetParent(GameObject.Find("Board").transform);
        }
    }

    public void SetupScene(int level)
    {
        LoadJSON(level);
        BoardSetup();
        InitialiseList();
        LayoutObjects();
    }
}
