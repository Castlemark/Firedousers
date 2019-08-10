using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileEnums;

public class SurvivorBehaviour : MonoBehaviour, IBehaviour {

    bool canPass = false;
    Vector2Int position;
    public RuntimeAnimatorController[] animators;

    public int state { get; set; }


    public void Initialize(int state) {
        this.state = state;

        position.x = this.transform.parent.GetComponent<Tile>().position[0];
        position.y = this.transform.parent.GetComponent<Tile>().position[1];

        Animator anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = animators[state];
        anim.SetFloat("offset", Random.Range(0.0f, 1.0f));
        //anim.SetInteger("selector", state);
    }

	public void ExecuteBehaviour()
	{
        if (GameObject.Find("Player").GetComponent<Player>().carryVictim(state))
        {
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
        }
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
