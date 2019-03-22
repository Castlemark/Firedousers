using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafepointBehaviourScript : MonoBehaviour, IBehaviour
{
    public int state { get; set; }
    public void Initialize(int state) { }

    public void ExecuteBehaviour()
    {
        GameObject.Find("Player").GetComponent<Player>().saveVictim();
    }

    public bool CanPass()
    {
        return true;
    }

    public bool IsFlammable()
    {
        return false;
    }

    public void SetSprite(int room_tileset = 0)
    {

    }
}
