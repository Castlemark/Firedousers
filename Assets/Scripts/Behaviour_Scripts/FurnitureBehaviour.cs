using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBehaviour : MonoBehaviour, IBehaviour {

	/*
	 * INTACT = 0
	 * BURNED = 1
	 */
	private int state;
	
	public void Initialize(int state)
	{
		this.state = 0;
	}

	public void ExecuteBehaviour()
	{

    }

	public bool CanPass()
	{
		return state == 1;
	}

    public bool IsFlammable()
    {
        return true;
    }
}
