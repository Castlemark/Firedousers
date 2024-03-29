﻿using System;
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

    public int tileset;

    private Fire fireScript;

    public bool canPass;
    public bool reserved;
    private bool isConsumed = false;
    private bool isCollapsed = false;
    private GameObject typeObject;
    public GameObject containedObject;
    private GameObject fireObject;
    private IBehaviour behaviour;

    public Sprite stair_up_top_image;
    public Sprite stair_up_bottom_image;
    public Sprite stair_up_right_image;
    public Sprite stair_up_left_image;
    public Sprite stair_down_top_image;
    public Sprite stair_down_bottom_image;
    public Sprite stair_down_right_image;
    public Sprite stair_down_left_image;

    private Sprite[] room_images;
    public Sprite[] floor_images;
    public Sprite[] wall_images;
    public Sprite[] front_wall_images;
    public Sprite[] breakable_wall_images;

    private bool CR_running = false;

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
        reserved = false;

        tileset = room_tileset;

        switch (typeSetUp)
        {
            case TYPE.floor:
                fireObject = Instantiate(Resources.Load("Prefabs/Fire"), transform.position, Quaternion.identity, this.transform) as GameObject;
                fireScript = fireObject.GetComponent<Fire>();
                fireObject.name = "Fire";
                room_images = getRoomImages(room_tileset, floor_images);
                typeSprite.GetComponent<SpriteRenderer>().sprite = room_images[Random.Range(0, room_images.Length - 1)];
                break;

            case TYPE.wall:
                typeSprite.GetComponent<SpriteRenderer>().sprite = wall_images[Random.Range(0, wall_images.Length - 1)];
                canPass = false;
                break;

            case TYPE.front_wall:
                room_images = getRoomImages(room_tileset, front_wall_images);
                typeSprite.GetComponent<SpriteRenderer>().sprite = room_images[Random.Range(0, room_images.Length - 1)];
                canPass = false;
                break;

            case TYPE.breakable_wall:
                //room_images = getRoomImages(room_tileset, breakable_wall_images);
                canPass = false;
                break;

            case TYPE.stair_up:
                Sprite orientation;

                if (position[1] == 1) orientation = stair_up_bottom_image;
                else if (position[1] >= GameManager.instance.GetComponent<LevelGenerator>().columns - 3) orientation = stair_up_top_image;
                else if (position[0] == (GameManager.instance.level - 1) * GameManager.instance.boardScript.rows + 1) orientation = stair_up_left_image;
                else orientation = stair_up_right_image;

                typeSprite.GetComponent<SpriteRenderer>().sprite = orientation;
                break;

            case TYPE.stair_down:
                Sprite orientationD;

                if (position[1] == 1) orientationD = stair_down_bottom_image;
                else if (position[1] >= GameManager.instance.GetComponent<LevelGenerator>().columns - 3) orientationD = stair_down_top_image;
                else if (position[0] == (GameManager.instance.level - 1) * GameManager.instance.boardScript.rows + 1) orientationD = stair_down_left_image;
                else orientationD = stair_down_right_image;

                typeSprite.GetComponent<SpriteRenderer>().sprite = orientationD;
                break;

            default:
                Debug.Log("Tile type " + typeSetUp + " entered default state (Floor)");
                type = typeSetUp;
                break;
        }

        CheckTileIntegrity();

        containedObject = Instantiate(contained.GetPrefab(), transform.position, Quaternion.identity, this.transform) as GameObject;
        containedObject.name = containedSetup.ToString();
        behaviour = containedObject.GetComponent<IBehaviour>();

        behaviour.Initialize(state);
        behaviour.SetSprite(room_tileset);

        this.name = contained.ContainsNone() ? type.ToString() : contained.ToString();
    }

    public void ReplaceContained(CONTAINED newContained, int state)
    {
        Destroy(this.transform.Find(contained.ToString()).gameObject);
        contained = newContained;

        containedObject = Instantiate(contained.GetPrefab(), transform.position, Quaternion.identity, this.transform) as GameObject;
        containedObject.name = newContained.ToString();
        behaviour = containedObject.GetComponent<IBehaviour>();

        behaviour.Initialize(state);
        behaviour.SetSprite(tileset);

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

        Array.Resize(ref room_images, room_images.Length - 1);
        return room_images;
    }

    //Throws error if tile combination is invalid
    private void CheckTileIntegrity()
    {
        if ((type.IsWall() || type.IsStair()) && !contained.ContainsNone())
        {
            throw new Exception("Tile of type " + type + " can't have any contained object ");
        }
    }

    public void ExecutePreBehaviour()
    {
        behaviour.ExecuteBehaviour();

        if (type.IsStairUp()) GameManager.instance.ChangeLevel(1);
        else if (type.IsStairDown()) GameManager.instance.ChangeLevel(-1);
        else if (HasFire()) fireScript.StepOnFire();
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

        int x = this.position[0];
        int y = this.position[1];

        if (y < grid.GetLength(1) - 1 && grid[x, y + 1] != null) tiles[0] = grid[x, y + 1].GetComponent<Tile>(); //N
        if (y != 0 && grid[x, y - 1] != null) tiles[2] = grid[x, y - 1].GetComponent<Tile>();//S
        if (x < grid.GetLength(0) - 1 && grid[x + 1, y] != null) tiles[1] = grid[x + 1, y].GetComponent<Tile>();//E
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

            case TYPE.front_wall:
                typeSprite.GetComponent<SpriteRenderer>().sprite = front_wall_images[index];
                canPass = false;
                break;

            case TYPE.breakable_wall:
                typeSprite.GetComponent<SpriteRenderer>().sprite = breakable_wall_images[index];
                canPass = false;
                break;

            case TYPE.stair_up:
                typeSprite.GetComponent<SpriteRenderer>().sprite = stair_up_right_image;
                break;

            case TYPE.stair_down:
                typeSprite.GetComponent<SpriteRenderer>().sprite = stair_down_right_image;
                break;

            default:
                Debug.Log("Tile type " + type + " entered default state (Floor)");
                typeSprite.GetComponent<SpriteRenderer>().sprite = floor_images[index];
                break;
        }
    }

    public void IfSurvivorThenAttemptMove()
    {
        if (contained.IsSurvivor())
        {
            Vector2Int fleeDirection = CalculateFleeDirection();
            if (fleeDirection.x + fleeDirection.y != 0)
            {
                //if the survivor can't move in the desired direction, it will try to move to both ortogonal directions
                if (!MoveContainedInDirIfPossible(fleeDirection))
                {
                    Vector2Int aux = new Vector2Int();
                    aux.x = fleeDirection.y;
                    aux.y = fleeDirection.x;
                    fleeDirection = aux;

                    if (!MoveContainedInDirIfPossible(fleeDirection))
                    {
                        fleeDirection.x = -fleeDirection.x;
                        fleeDirection.y = -fleeDirection.y;

                        MoveContainedInDirIfPossible(fleeDirection);
                    }
                }
            }
        }
    }

    private bool MoveContainedInDirIfPossible(Vector2Int dir)
    {
        Tile movTile = GameManager.instance.boardScript.grid[position[0] + dir.x, position[1] + dir.y].GetComponent<Tile>();
        if (!CR_running &&
            movTile.type.IsFloor() &&
            movTile.contained != CONTAINED.item &&
            movTile.contained != CONTAINED.furniture &&
            movTile.contained != CONTAINED.survivor &&
            !movTile.HasBurningFire() &&
            !movTile.isCollapsed &&
            !movTile.reserved)
        {
            bool disappears = false;

            if (movTile.contained == CONTAINED.safepoint)
            {
                disappears = true;
            }
            else
            {
                movTile.behaviour.ExecuteBehaviour();
            }

            Vector3 newPos = new Vector3(containedObject.transform.position.x + dir.x, containedObject.transform.position.y + dir.y, containedObject.transform.position.z);
            StartCoroutine(SmoothMovement(dir, newPos, movTile, disappears));
            CollapseFloorIfNecessary();

            return true;
        }
        return false;
    }

    private IEnumerator SmoothMovement(Vector2Int dir, Vector3 end, Tile movTile, bool disappears)
    {

        CR_running = true;
        movTile.reserved = true;
        Animator anim = containedObject.GetComponent<Animator>();
        if (dir.x > 0)
        {
            anim.SetTrigger("playerRight");
        }
        else if (dir.x < 0)
        {
            anim.SetTrigger("playerLeft");
        }
        else if (dir.y > 0)
        {
            anim.SetTrigger("playerBack");
        }
        else
        {
            anim.SetTrigger("playerFront");
        }
        Rigidbody2D rb2D = containedObject.GetComponent<Rigidbody2D>();
        if (rb2D)
        {
            float sqrRemainingDistance = (containedObject.transform.position - end).sqrMagnitude;
            while (sqrRemainingDistance > 0.0021)
            {
                if (rb2D == null) break;
                Vector3 newPosition = Vector3.MoveTowards(new Vector3(rb2D.position.x, rb2D.position.y, containedObject.transform.position.z), end, (2f) * Time.deltaTime);
                rb2D.MovePosition(newPosition);
                sqrRemainingDistance = (containedObject.transform.position - end).sqrMagnitude;
                yield return null; //s'espera un frame abans de tornar a avaluar la condició del WHILE
            }
            if (!disappears) movTile.ReplaceContained(CONTAINED.survivor, containedObject.GetComponent<IBehaviour>().state);
            this.ReplaceContained(CONTAINED.none, 0);
        }
        CR_running = false;
        movTile.reserved = false;


    }

    private Vector2Int CalculateFleeDirection()
    {
        Vector2 fleeDir = new Vector2(0, 0);

        List<Tile> nearFire = GameManager.instance.boardScript.GetFireTilesWithinRange(position[0], position[1], 2);

        foreach (Tile tile in nearFire)
        {
            fleeDir.x += position[0] - tile.position[0];
            fleeDir.y += position[1] - tile.position[1];
        }
        // We get the normalized vector of the opposite direction where the "most" fire is
        fleeDir.Normalize();

        return ToGridVector(fleeDir);
    }

    private Vector2Int ToGridVector(Vector2 dir)
    {
        Vector2Int returnDir = new Vector2Int(0, 0);
        if (System.Math.Abs(dir.x) > System.Math.Abs(dir.y))
        {
            if (dir.x > 0) returnDir = new Vector2Int(1, 0);
            else returnDir = new Vector2Int(-1, 0);
        }
        else
        {
            if (dir.y > 0) returnDir = new Vector2Int(0, 1);
            else if (dir.y < 0) returnDir = new Vector2Int(0, -1);
        }

        return returnDir;
    }

    public bool HasFire()
    {
        return (fireObject != null);
    }

    public bool HasBurningFire()
    {
        if (HasFire()) return fireScript.IsIgnited();
        else return false;
    }

    public void IncreaseFire()
    {
        if (HasFire()) isConsumed = fireScript.EvolveFire();
    }

    public void StartFire()
    {
        if (HasFire()) fireScript.StartFire();
    }

    public void DrownTile()
    {
        if (HasFire()) fireScript.DrownFire();
    }
}
