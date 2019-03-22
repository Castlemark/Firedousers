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

    private Animator animator;

    private void Awake()
    {
       animator = GetComponent<Animator>();

    }

    public void ChangeState(int new_state)
    {
        state = new_state;
        state_counter = 0;
        animator.SetInteger("state", new_state);
    }

    private void IncreaseCount() { state_counter++; }

    //if state between 1 and 4, will evolve
    public bool EvolveFire()
    {
        if (state != 0 && state < 5)
        {
            IncreaseCount();
            if (state_counter > state_increase_steps[state])
            {
                ChangeState(state + 1);
            }

            if (state == 4 && state_counter == 0)
            {
                ExpandFire();
            }
        }

        return IsConsumed();
    }

    private bool IsConsumed()
    {
        return state > 4 && state < 7;
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
            ChangeState(1);
        }
        else if (state == 7)
        {
            ChangeState(0);
        }
    }

    public void StepOnFire()
    {
        if (state == 1)
        {
            this.ChangeState(0);
        }
        if (state ==3 || state ==4)
        {
            GameObject.Find("Player").GetComponent<Player>().IncreaseTemperature(state);
        }
    }

    public void DrownFire()
    {
        this.ChangeState(7);
    }

    public bool IsIgnited()
    {
        return state > 0 && state < 5;
    }
}
