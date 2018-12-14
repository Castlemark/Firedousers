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

    public bool broken = false;

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

        if (state == 0 || state == 7) { min_state = true; }
        else { min_state = false; }

        if (state == 5) { ExpandFire(); }
        if (state == 6) { MakeFloorCollapsable(); }
    }

    public void EvolveState()
    {

        if (broken) { BreakFloor(); }

        if (!max_state && !min_state && state_counter >= state_increase_steps[state - 1])
        {
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

    private void BreakFloor()
    {
        gameObject.layer = LayerMask.NameToLayer("VisibilityLayer");
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        Debug.Log(gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>());
        this.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = null;
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
            else if (hit.collider != null && hit.collider.name.Contains("fire") && hit.collider.GetComponent<FireController>().state == 7)
            {
                hit.collider.GetComponent<FireController>().ChangeState(0);
            }
            else if (hit.collider != null && hit.collider.name.Contains("burnable"))
            {
                Vector3 position = hit.collider.transform.parent.transform.position;
                Destroy(hit.transform.parent.gameObject);
            }
        }

        GetComponent<BoxCollider2D>().enabled = true;
    }

    void MakeFloorCollapsable()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void SteppedOnFire()
    {
        if (state == 1) { ChangeState(0); }
        if (state >= 3 && state < 7)
        {
            GameObject.Find("Player").GetComponent<Player>().IncreaseTemperature();
        }
    }
}
