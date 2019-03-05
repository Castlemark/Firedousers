using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileEnums;

public class SurvivorBehaviour : MonoBehaviour, IBehaviour {

    bool canPass = false;
	
	/*
	 * Doesn't have states
	 */
	public void Initialize(int state) {
        Animator anim = GetComponent<Animator>();
        anim.SetInteger("selector", Random.Range(0, 4));
    }

	public void ExecuteBehaviour()
	{
        GameObject.Find("Player").GetComponent<Player>().carryVictim();
        transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
    }

	public bool CanPass()
    {
        return canPass;
    }

    public bool IsFlammable()
    {
        return true;
    }

    public void SetSprite(int room_tileset = 0) { }
}
