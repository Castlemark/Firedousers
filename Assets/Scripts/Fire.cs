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
    
    public void IncreaseCount() { state_counter++; }
}
