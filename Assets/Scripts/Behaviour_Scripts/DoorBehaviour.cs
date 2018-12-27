﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour, IBehaviour
{

	public Sprite locked;
	public Sprite open;

	/*
	 * OPEN = 0
	 * LOCKED = 1
	 */
	private int state;

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
				GetComponent<SpriteRenderer>().sprite = locked;
				break;
			
			case 1:
				GetComponent<SpriteRenderer>().sprite = open;
				break;
			
			default:
				GetComponent<SpriteRenderer>().sprite = locked;
				this.state = 0;
				Debug.Log("Invalid state (" + state + ") for Door, reset state to 0 : Locked");
				break;
		}
	}

	public void ExecuteBehaviour()
	{
		if (state != 1){ ChangeState(1); }
	}

	public bool CanPass()
	{
		return state == 1;
	}

    public bool IsFlammable()
    {
        throw new System.NotImplementedException();
    }
}
