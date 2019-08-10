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
            GameObject hoseAux = GameObject.Find("/Player/PowerUpHose");
            GameObject hoseTextAux = GameObject.Find("/Player/PowerUpHose/Canvas/PowerUpHoseText");
            hoseTextAux.GetComponent<TMPro.TextMeshProUGUI>().SetText("+" + 5 * GameManager.instance.level);
            hoseAux.SetActive(false);
            hoseAux.SetActive(true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose += 5 * GameManager.instance.level;
            GameManager.instance.totalHoseMeters += 5 * GameManager.instance.level;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hoseText.text = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose.ToString();
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hoseHUD.GetComponent<HoseHUD>().changeSprite(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().metersHose, GameManager.instance.totalHoseMeters);
            SoundManager.instance.RandomizeSfx(hose);

        }
        else if (state == 0)
        {
            GameObject axeAux = GameObject.Find("/Player/PowerUpAxe");
            axeAux.SetActive(false);
            axeAux.SetActive(true);
            GameManager.instance.playerHasAxe = true;
            GameManager.instance.axe.SetActive(true);
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
            SoundManager.instance.RandomizeSfx(axe);

        }
        else
        {
            GameObject healthAux = GameObject.Find("/Player/PowerUpHealth");
            healthAux.SetActive(false);
            healthAux.SetActive(true);
            Debug.Log(GameObject.FindGameObjectWithTag("Player"));
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().IncreaseTemperature(0);
            transform.parent.GetComponent<Tile>().ReplaceContained(CONTAINED.none, 0);
            SoundManager.instance.RandomizeSfx(health);

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
