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


    public void ChangeState(int new_state)
    {
        state = new_state;
        state_counter = 0;
        animator.SetInteger("state", new_state);
    }

    private void IncreaseCount() { state_counter++; }

    public bool EvolveFire()
    {
        if (state != 0 && state < 5)
        {
            IncreaseCount();
            if (state_counter > state_increase_steps[state])
            {
                Debug.Log("Cahnging state");
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
        return state > 4;
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
        animator = GetComponent<Animator>();

        if (state < 1)
        {
            animator.SetInteger("state",1);
            state = 1;
        }
    }

    public void StepOnFire()
    {
        if (state == 1)
        {
            Debug.Log("state to 0");
            this.ChangeState(0);
            Debug.Log(state);
        }
        if (state ==3 || state ==4)
        {
            GameObject.Find("Player").GetComponent<Player>().IncreaseTemperature();
        }
    }
}
