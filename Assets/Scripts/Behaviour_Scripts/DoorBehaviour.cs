﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour, IBehaviour
{
    public Sprite h_locked;
    public Sprite h_open;
    public Sprite v_locked;
    public Sprite v_open;

    /*
	 * OPEN = 0
	 * LOCKED = 1
	 */
    public int state { get; set; }

    public void Initialize(int state)
    {
        ChangeState(state);
    }

    private void ChangeState(int state)
    {
        this.state = state;

        Tile parent = this.transform.parent.GetComponent<Tile>();

        Tile[] tiles = parent.GetAdjoiningTiles();

        char type = 'h';
        if (tiles[2] != null && tiles[2].name == "wall") type = 'v';

        switch (state)
        {
            case 0:
                if (type == 'h') GetComponent<SpriteRenderer>().sprite = h_locked;
                else GetComponent<SpriteRenderer>().sprite = v_locked;
                break;

            case 1:
                if (type == 'h') GetComponent<SpriteRenderer>().sprite = h_open;
                else GetComponent<SpriteRenderer>().sprite = v_open;
                RemoveCube(type);
                break;

            default:
                this.state = 0;
                Debug.Log("Invalid state (" + state + ") for Door, reset state to 0 : Locked");
                break;
        }
    }

    private void RemoveCube(char type)
    {
        Vector3 pos = this.transform.position;
        GameObject shadow = GameObject.Find("Shadow");

        foreach (Transform child in shadow.transform)
        {
            if (child.gameObject.tag == "DoorCube")
            {
                Vector3 child_pos = child.position;

                if (type == 'v')
                {
                    if ((pos.y == child_pos.y || pos.y == child_pos.y - 1) && Mathf.Round(pos.x - 0.4f - child_pos.x) == 0)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }

                if (type == 'h')
                {
                    if (pos.x == child_pos.x && Mathf.Round(child_pos.y - 0.4f - pos.y) == 0)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }
    }

    public void ExecuteBehaviour()
    {
        if (state != 1) { ChangeState(1); }
    }

    public bool CanPass()
    {
        return state == 1;
    }

    public bool IsFlammable()
    {
        throw new System.NotImplementedException();
    }

    public void SetSprite(int room_tileset = 0) { }
}
