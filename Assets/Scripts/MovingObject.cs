using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

    public float moveTime = 50f;
    public LayerMask blockingLayer;
    public LayerMask visibilityLayer;
    public LayerMask fireLayer;
    // Use this for initialization

    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }



    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > 0.0021)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null; //s'espera un frame abans de tornar a avaluar la condició del WHILE
        }

        GameManager.instance.boardScript.BMExecutePreBehaviour(((Player)this).position.x, ((Player)this).position.y);
        ((Player)this).CheckIfGameOver();
        ((Player)this).playerMovingCoroutine = false;


    }


}
