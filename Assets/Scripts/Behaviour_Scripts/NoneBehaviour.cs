using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneBehaviour : MonoBehaviour, IBehaviour {


	public void Initialize(int state) {}

	public void ExecuteBehaviour() {}

	public bool CanPass() { return true; }

    public bool IsFlammable()
    {
        return true;
    }

    public void SetSprite(int room_tileset = 0) { }
}
