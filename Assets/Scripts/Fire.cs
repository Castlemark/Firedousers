using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public Sprite[] fireStates;
    public int state = 0;

    public int state_counter = 0;
    public bool max_state;
    public bool min_state;

    public bool broken = false;

    public int[] state_increase_steps;


    public void IncreaseState()
    {
        state++;
        state_counter = 0;
        GetComponent<SpriteRenderer>().sprite = fireStates[state];
    }
    
    private void IncreaseCount() { state_counter++; }

    public void EvolveFire()
    {
        if (state != 0 && state < 5)
        {
            IncreaseCount();
            if (state_counter > state_increase_steps[state])
            {
                IncreaseState();
            }

            if (state == 4)
            {
                ExpandFire();
            }
        }
    }

    private void ExpandFire()
    {
        Tile[] tiles = transform.parent.GetComponent<Tile>().GetAdjoiningTiles();
        for (int i = 0; i < 4; i++)
        {
            if (tiles[i].transform.Find("Fire") != null)
            {
                tiles[i].transform.Find("Fire").GetComponent<Fire>().StartFire();
            }
        }
    }

    public void StartFire()
    {
        if (state < 1)
        {
            state = 1;
            GetComponent<SpriteRenderer>().sprite = fireStates[1];
        }
    }
}
