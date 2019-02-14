using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TileEnums;

public class Tile : MonoBehaviour
{
    public TYPE type;
    public CONTAINED contained;

    public int[] position;

    private Fire fireScript;

    private bool canPass;
    private bool isConsumed = false;
    private bool isCollapsed = false;
    private GameObject typeObject;
    public GameObject containedObject;
    private GameObject fireObject;
    private IBehaviour behaviour;
    private Sprite[] room_images;

    public Sprite[] floor_images;
    public Sprite[] wall_images;
    public Sprite[] breakable_wall_images;
    public Sprite[] stair_up_images;
    public Sprite[] stair_down_images;
    public Sprite[] safepoint_images;

    public void SetUpTile(TYPE typeSetUp, CONTAINED containedSetup, int state, int room_tileset, int[] position)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        typeObject = Instantiate(Resources.Load("Prefabs/Type"), transform.position, Quaternion.identity, this.transform) as GameObject;
        typeObject.name = "Type";
        this.position = position;

        Transform typeSprite = typeObject.transform;

        type = typeSetUp;
        contained = containedSetup;
        canPass = true;

        switch (typeSetUp)
        {
            case TYPE.floor:
                fireObject = Instantiate(Resources.Load("Prefabs/Fire"), transform.position, Quaternion.identity, this.transform) as GameObject;
                fireScript = fireObject.GetComponent<Fire>();
                fireObject.name = "Fire";
                room_images = getRoomImages(room_tileset, floor_images);
                break;

            case TYPE.wall:
                room_images = getRoomImages(room_tileset, wall_images);
                canPass = false;
                break;

            case TYPE.breakable_wall:
                room_images = getRoomImages(room_tileset, breakable_wall_images);
                canPass = false;
                break;

            case TYPE.stair_up:
                room_images = getRoomImages(room_tileset, stair_up_images);
                break;

            case TYPE.stair_down:
                room_images = getRoomImages(room_tileset, stair_down_images);
                break;

            case TYPE.safepoint:
                room_images = getRoomImages(room_tileset, safepoint_images);
                break;

            default:
                Debug.Log("Tile type " + typeSetUp + " entered default state (Floor)");
                type = typeSetUp;
                room_images = getRoomImages(room_tileset, floor_images);
                break;
        }

        typeSprite.GetComponent<SpriteRenderer>().sprite = room_images[Random.Range(0, room_images.Length - 1)];

        CheckTileIntegrity();

        containedObject = Instantiate(contained.GetPrefab(), transform.position, Quaternion.identity, this.transform) as GameObject;
        containedObject.name = containedSetup.ToString();
        behaviour = containedObject.GetComponent<IBehaviour>();

        behaviour.Initialize(state);
        behaviour.SetSprite(room_tileset);

        this.name = contained.ContainsNone() ? type.ToString() : contained.ToString();
    }

    //Returns images of specific room tileset
    public Sprite[] getRoomImages(int room, Sprite[] images)
    {
        Sprite[] room_images = new Sprite[1];

        for (int i = 0; i < images.Length; i++)
        {
            String prefix = images[i].name.Substring(0, 6);
            if (string.Equals(prefix, "room_" + room))
            {
                room_images[room_images.Length - 1] = images[i];
                Array.Resize(ref room_images, room_images.Length + 1);
            }
        }

        return room_images;
    }

    //Throws error if tile combination is invalid
    private void CheckTileIntegrity()
    {
        if ((type.IsWall() || type.IsStair() || type.IsSafePoint()) && !contained.ContainsNone())
        {
            throw new Exception("Tile of type " + type + " can't have any contained object ");
        }
    }

    public void ExecutePreBehaviour()
    {
        behaviour.ExecuteBehaviour();
        if (fireObject != null)
        {
            fireScript.StepOnFire();
        }
    }

    public void ExecutePostBehaviour()
    {
        CollapseFloorIfNecessary();
    }

    private void CollapseFloorIfNecessary()
    {
        if (isConsumed && !isCollapsed)
        {
            canPass = false;
            isCollapsed = true;
            fireScript.ChangeState(6);
        }
    }

    public bool CanPass()
    {
        return canPass && behaviour.CanPass();
    }

    public void PrintInfo()
    {
        Debug.Log("type: " + type + " ; contains: " + contained);
    }

    public Tile[] GetAdjoiningTiles()
    {
        GameObject[,] grid = GameManager.instance.boardScript.grid;
        Tile[] tiles = new Tile[4];

        int x = this.position[1];
        int y = this.position[0];
        
        if (y != 0 && grid[x, y - 1] != null) tiles[2] = grid[x, y - 1].GetComponent<Tile>();//S
        if (y != 0 && grid[x, y + 1] != null) tiles[0] = grid[x, y + 1].GetComponent<Tile>();//N
        if (x != 0 && grid[x + 1, y] != null) tiles[1] = grid[x + 1, y].GetComponent<Tile>();//E
        if (x != 0 && grid[x - 1, y] != null) tiles[3] = grid[x - 1, y].GetComponent<Tile>();//W

        return tiles;
    }

    public void ChangeTypeSpriteTo(int index)
    {
        Transform typeSprite = typeObject.transform;

        switch (type)
        {
            case TYPE.floor:
                typeSprite.GetComponent<SpriteRenderer>().sprite = floor_images[index];
                break;

            case TYPE.wall:
                typeSprite.GetComponent<SpriteRenderer>().sprite = wall_images[index];
                canPass = false;
                break;

            case TYPE.breakable_wall:
                typeSprite.GetComponent<SpriteRenderer>().sprite = breakable_wall_images[index];
                canPass = false;
                break;

            case TYPE.stair_up:
                typeSprite.GetComponent<SpriteRenderer>().sprite = stair_up_images[index];
                break;

            case TYPE.stair_down:
                typeSprite.GetComponent<SpriteRenderer>().sprite = stair_down_images[index];
                break;

            case TYPE.safepoint:
                typeSprite.GetComponent<SpriteRenderer>().sprite = safepoint_images[index];
                break;

            default:
                Debug.Log("Tile type " + type + " entered default state (Floor)");
                typeSprite.GetComponent<SpriteRenderer>().sprite = floor_images[index];
                break;
        }
    }

    public void IncreaseFire()
    {
        if (fireObject != null) isConsumed = fireScript.EvolveFire();
    }

    public void StartFire()
    {
        if (fireObject != null) fireScript.StartFire();
    }
}
