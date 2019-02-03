using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorBehaviour : MonoBehaviour, IBehaviour {

    bool canPass = false;
	
	/*
	 * Doesn't have states
	 */
	public void Initialize(int state) {}

	public void ExecuteBehaviour()
	{
        Debug.Log("Behaviour not yet implemented");
	}

	public bool CanPass()
    {
        return canPass;
    }

    public bool IsFlammable()
    {
        return true;
    }

    public void SetSprite(int room_tileset = 0) { }
}
