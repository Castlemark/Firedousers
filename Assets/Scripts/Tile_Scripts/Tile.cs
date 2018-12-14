using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using TileEnums;

public class Tile : MonoBehaviour {

	public TYPE type;
	public CONTAINED contained;

	private bool canPass;
    private GameObject typeObject;
    private GameObject containedObject;
	private IBehaviour behaviour;

	public Sprite[] floor_images;
	public Sprite[] wall_images;
	public Sprite[] breakable_wall_images;
	public Sprite[] stair_up_images;
	public Sprite[] stair_down_images;
	public Sprite[] safepoint_images;

	public void SetUpTile(TYPE typeSetUp, CONTAINED containedSetup, int state)
	{	
		typeObject = Instantiate(Resources.Load("Prefabs/Type"), transform.position, Quaternion.identity, this.transform) as GameObject;
		typeObject.name = "Type";
		
		Random rnd = new Random();
		Transform typeSprite = typeObject.transform;
		
		type = typeSetUp;
		contained = containedSetup;
		canPass = true;
		
		switch (typeSetUp)
		{
			case TYPE.floor:
				typeSprite.GetComponent<SpriteRenderer>().sprite = floor_images[rnd.Next(0, floor_images.Length)];
				break;
			
			case TYPE.wall:
				typeSprite.GetComponent<SpriteRenderer>().sprite = wall_images[rnd.Next(0, wall_images.Length)];
				canPass = false;
				break;
			
			case TYPE.breakable_wall:
				typeSprite.GetComponent<SpriteRenderer>().sprite = breakable_wall_images[rnd.Next(0, breakable_wall_images.Length)];
				canPass = false;
				break;
			
			case TYPE.stair_up:
				typeSprite.GetComponent<SpriteRenderer>().sprite = stair_up_images[rnd.Next(0, stair_up_images.Length)];
				break;
			
			case TYPE.stair_down:
				typeSprite.GetComponent<SpriteRenderer>().sprite = stair_down_images[rnd.Next(0, stair_down_images.Length)];
				break;
			
			case TYPE.safepoint:
				typeSprite.GetComponent<SpriteRenderer>().sprite = safepoint_images[rnd.Next(0, safepoint_images.Length)];
				break;
			
			default:
				Debug.Log("Tile type "+ typeSetUp +" entered default state (Floor)");
				type = typeSetUp;
				typeSprite.GetComponent<SpriteRenderer>().sprite = floor_images[rnd.Next(0, floor_images.Length)];
				break;
		}

		CheckTileIntegrity();

		containedObject = Instantiate(contained.GetPrefab(), transform.position, Quaternion.identity, this.transform) as GameObject;
		containedObject.name = containedSetup.ToString();
		behaviour = containedObject.GetComponent<IBehaviour>();
		behaviour.Initialize(state);

		this.name = contained.ContainsNone() ? type.ToString() : contained.ToString();
	}

	//Throws error if tile combination is invalid
	private void CheckTileIntegrity()
	{
		if ((type.IsWall() || type.IsStair() || type.IsSafePoint()) && !contained.ContainsNone())
		{
			throw new Exception("Tile of type " + type + " can't have any contained object ");
		}
	} 

	public void ExecuteBehaviour()
	{
		behaviour.ExecuteBehaviour();
	}

	public bool CanPass()
	{
		return canPass && behaviour.CanPass();
	}

    public void PrintInfo()
    {
        Debug.Log("type: " + type + " ; contains: " + contained);
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
	
}
