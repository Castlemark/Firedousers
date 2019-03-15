using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour, IBehaviour
{

    public Sprite[] items;

    /*
	 * AXE = 0
	 * KEY = 1
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

            default:
                GetComponent<SpriteRenderer>().sprite = items[0];
                this.state = 0;
                Debug.Log("Invalid state (" + state + ")");
                break;
        }
    }

    public void ExecuteBehaviour()
    {
        Debug.Log("Behaviour not yet implemented");
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
