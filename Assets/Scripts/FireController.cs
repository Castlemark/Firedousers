using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    public LayerMask fireLayer;
    public Sprite[] fireStates;
    public int state;

    public int state_counter = 0;
    public bool max_state;
    public bool min_state;

    public int[] state_increase_steps;

    private readonly Vector2[] directions = { new Vector2(0,1),
                                              new Vector2(1,0),
                                              new Vector2(0,-1),
                                              new Vector2(-1,0)
                                            };
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeState(int new_state)
    {
        state = new_state;
        state_counter = 0;
        GetComponent<SpriteRenderer>().sprite = fireStates[state];

        if (state == 6) { max_state = true; }
        else { max_state = false; }

        if (state == 0) { min_state = true; }
        else { min_state = false; }

        if (state == 5) { ExpandFire(); }
    }

    public void EvolveState()
    {
        if (!max_state && !min_state && state_counter >= state_increase_steps[state - 1])
        {
            bool will_change = !max_state && !min_state;
            Debug.Log("fire state: " + state + " ; state counter:" + state_counter + "threshold: " + state_increase_steps[state - 1] + " ; min state:" + min_state + " ; will change: " + will_change);
            ChangeState(state + 1);
        }
        else
        {
            if (min_state || max_state)
            {
                state_counter = 0;
            }
            else
            {
                state_counter++;
            }
        }
    }

    void ExpandFire()
    {

        RaycastHit2D hit;

        GetComponent<BoxCollider2D>().enabled = false;

        foreach (Vector2 vector in directions)
        {
            Vector2 start = transform.position;
            Vector2 end = start + vector;

            hit = Physics2D.Linecast(start, end, fireLayer);

            if (hit.collider != null && hit.collider.name.Contains("fire") && hit.collider.GetComponent<FireController>().state == 0)
            {
                hit.collider.GetComponent<FireController>().ChangeState(1);
            }
            else if (hit.collider != null && hit.collider.name.Contains("burnable"))
            {
                Debug.Log("destroyed furniture");
                Vector3 position = hit.collider.transform.parent.transform.position;
                Destroy(hit.transform.parent.gameObject);
                GameObject.Find("GameManager(Clone)").GetComponent<BoardManager>().InstantiateFloor(position);
            }
        }
    }
}
