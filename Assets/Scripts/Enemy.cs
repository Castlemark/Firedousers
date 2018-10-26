using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//De la mateixa manere que player Nemy també hereta de MovingObject
public class Enemy : MovingObject
{
    public int playerDamage;                            //Punts de food que treu
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;                          
    private Transform target;                           //per poder intentar moure l'enemic cada torn
    private bool skipMove;                              //per saber si sha de moure o no en aquest torn



    protected override void Start()
    {
        
        GameManager.instance.AddEnemyToList(this);  //afegim l'enemic a la llista del GameManager

        animator = GetComponent<Animator>();

        //El player és el target del moviment
        target = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
    }


    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;

        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }


    public void MoveEnemy()
    {

        int xDir = 0;
        int yDir = 0;

        //si es troben el mateix eix X intentarem moure en vertical
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)

            //Mourem amunt o avall segons la posició y
            yDir = target.position.y > transform.position.y ? 1 : -1;

        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        //El parametre generic es Player ja que és el que l'enemic espera trobar-se
        AttemptMove<Player>(xDir, yDir);
    }


    //s'executa quan l'enemic xoca contra el jugador
    protected override void OnCantMove<T>(T component)
    {
        
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");
        
        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

    }
}