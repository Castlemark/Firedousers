using System.Collections;
using System.Collections.Generic;
using TileEnums;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour, IBehaviour
{

    public Sprite[] items;

    /*
	 * AXE = 0
	 * HOSE = 0
	 * to be expanded
	 */
    public int state { get; set; }

    public void Initialize(int state)
    {
        ChangeState(state);
    }

    private void ChangeState(int state)
    {
        this.state = state;

        switch (state)
        {
            case 0:
                GetComponent<SpriteRenderer>().sprite = items[state];
                break;

            case 1:
                GetComponent<SpriteRenderer>().sprite = items[state];
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = items[state];
                break;

            default:
                GetComponent<SpriteRenderer>().sprite = items[0];
                this.state = 0;
                Debug.Log("Invalid state (" + state + ")");
                break;
        }
    }

    public void ExecuteBehaviour()
    {
        if(state == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose += 20;
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);

        }
        else if (state == 0)
        {
            GameManager.instance.playerHasAxe = true;
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);

        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().IncreaseTemperature(0);
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
        }
    }

    public bool CanPass()
    {
        return true;
    }

    public bool IsFlammable()
    {
        return true;
    }

    public void SetSprite(int room_tileset = 0) { }
}
