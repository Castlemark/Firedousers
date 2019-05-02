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

    public AudioClip axe;
    public AudioClip hose;
    public AudioClip health;
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
            SoundManager.instance.RandomizeSfx(hose);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose += 20;
            GameManager.instance.totalHoseMeters += 20;
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hoseHUD.GetComponent<HoseHUD>().changeSprite(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose, GameManager.instance.totalHoseMeters);

        }
        else if (state == 0)
        {
            SoundManager.instance.RandomizeSfx(axe);

            GameManager.instance.playerHasAxe = true;
            GameManager.instance.axe.SetActive(true);
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);

        }
        else
        {
            SoundManager.instance.RandomizeSfx(health);

            Debug.Log(GameObject.FindGameObjectWithTag("Player"));
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
