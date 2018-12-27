using System.Collections;
using System.Collections.Generic;
using TileEnums;
using UnityEngine;

public class TestController : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		GameObject aux = Instantiate(TileUtils.GetTile(), Vector3.right, Quaternion.identity) as GameObject;
		aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.survivor,0);
		aux.GetComponent<Tile>().SetUpTile(TYPE.floor, CONTAINED.item,0);
		Instantiate(TileUtils.GetTile(), Vector3.down, Quaternion.identity).GetComponent<Tile>().SetUpTile(TYPE.wall, CONTAINED.none,0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
