using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafepointBehaviourScript : MonoBehaviour, IBehaviour
{
    public int state { get; set; }
    public void Initialize(int state) { }

    public Sprite[] sprites;

    public void ExecuteBehaviour()
    {
        GameObject.Find("Player").GetComponent<Player>().saveVictim();
    }

    public bool CanPass()
    {
        return true;
    }

    public bool IsFlammable()
    {
        return false;
    }

    public void SetSprite(int room_tileset = 0)
    {
        Tile parent = this.transform.parent.GetComponent<Tile>();
        BoardManager boardManager = GameManager.instance.boardScript;
        int columns = boardManager.columns;
        int rows = boardManager.rows;
        int x = parent.position[0];
        int y = parent.position[1];

        if (x == 1) this.GetComponent<SpriteRenderer>().sprite = sprites[3];
        else if (x == 18) this.GetComponent<SpriteRenderer>().sprite = sprites[1];
        else if (y == 1) this.GetComponent<SpriteRenderer>().sprite = sprites[2];
        else this.GetComponent<SpriteRenderer>().sprite = sprites[0];
    }
}
