using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tobeDestroyed = GameObject.FindGameObjectsWithTag("ToBeDestroyed");
        foreach (GameObject pum in tobeDestroyed){
            Destroy(pum);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
