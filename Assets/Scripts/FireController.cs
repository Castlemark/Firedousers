using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    public Sprite[] fireStates;
    public int state;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeState(int new_state)
    {
        state = new_state;
        GetComponent<SpriteRenderer>().sprite = fireStates[state];
    }
}
