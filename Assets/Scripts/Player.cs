using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int temperature = 0;
    public float restartLevelDelay = 1f;
    public Text hoseText;
    public Text peopleText;
    private Text temperatureText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    public GameObject manguera_v;
    public GameObject manguera_h;
    public GameObject manguera_lb;
    public GameObject manguera_lt;
    public GameObject manguera_rb;
    public GameObject manguera_rt;
    public GameObject manguera_empty;
    public GameObject manguera_end;

    public GameObject water;

    public GameObject hoseItem;

    public LayerMask mangueraLayer;

    private SpriteRenderer spriteRenderer;
    private int victims; //Víctimes que portes a sobre
    private int victims_total;
    private int maxVictims = 1;
    
    public Sprite spriteWithVictim;
    public Sprite spriteWithoutVictim;

    private Animator animator;
    private Animator animatorWater;
    private Animator animatorHoseItem;

    public int metersHose;
    private bool hasKey;
    public bool hasAxe;
    private List<string> path = new List<string>();
    
    private Vector2Int position;
    private Vector2Int endHose; 

    private bool pickingUpHose;
    private bool playerTurnInCourse;

    private List<GameObject> hoseList;
    private List<int> hoseAnim;
    private bool holdingHose; // false quan has deixat anar la manguera
    private GameObject manguera_end_reference;



    // Use this for initialization
    protected override void Start()
    {
        holdingHose = true;
        pickingUpHose = false;
        playerTurnInCourse = false;
        hoseList = new List<GameObject>();
        hoseAnim = new List<int>();

        temperatureText = GameObject.Find("TemperatureText").GetComponent<Text>();
        temperatureText.text = temperature.ToString();
        animator = GetComponent<Animator>();
        animatorWater = water.GetComponent<Animator>();
        animatorHoseItem = hoseItem.GetComponent<Animator>();


        metersHose = GameManager.instance.playerHoseMeters;
        hoseText.text = metersHose.ToString();
        victims = GameManager.instance.playerVictims;
        victims_total = GameManager.instance.playerVictimsTotal;
        peopleText.text = victims_total.ToString();
        hasKey = GameManager.instance.playerHasKey;

        hoseText.text = metersHose.ToString();
        path.Add("r");
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (victims > 0) spriteRenderer.sprite = spriteWithVictim;

    }

    // S'ecexuta quan es deshabilita el game object quan es canvia de nivell
    private void OnDisable()
    {
        GameManager.instance.playerHoseMeters = metersHose;
        GameManager.instance.playerHasKey = hasKey;
        GameManager.instance.playerVictims = victims;
        GameManager.instance.playerVictimsTotal = victims_total;
    }

    public void carryVictim()
    {
        victims++;
        spriteRenderer.sprite = spriteWithVictim;
    }

    public void saveVictim()
    {
        victims_total += victims;
        victims = 0;
        spriteRenderer.sprite = spriteWithoutVictim;
        peopleText.text = victims_total.ToString();
    }

    private void Update()
    {
        //Si no es el torn sortim de la funcio
        if (!GameManager.instance.playersTurn) return;

        if (pickingUpHose || playerTurnInCourse) return;        

        int horizontal = 0;
        int vertical = 0;

       
        //Obtenim Input de Unity segons el teclat i ho arrodonim a un enter
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Prevé que només es pugui moure en una direcció
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            //Passem el paràmetre Wall ja que es contra el que pot interactuar el jugador  
            if (!Input.GetKey(KeyCode.Space))
            {
                AttemptMove(horizontal, vertical);
            }
            else
            {
                if(horizontal == 1)
                {
                    animatorWater.SetTrigger("right");
                    animator.SetTrigger("waterRight");
                    animatorHoseItem.SetTrigger("waterRight");

                }
                else if(horizontal == -1)
                {
                    animatorWater.SetTrigger("left");
                    animator.SetTrigger("waterLeft");
                    animatorHoseItem.SetTrigger("waterLeft");


                }
                else if(vertical == -1){
                    animatorWater.SetTrigger("bottom");
                    animator.SetTrigger("waterFront");
                    animatorHoseItem.SetTrigger("waterFront");

                }
                else
                {
                    animator.SetTrigger("waterBack");
                    animatorHoseItem.SetTrigger("waterBack");


                }
                ShootWater(horizontal, vertical);
                //GameManager.instance.playersTurn = false;

            }
        }
        else if (Input.GetKey(KeyCode.J))
        {
            if (holdingHose)
            {
                endHose.x = position.x;
                endHose.y = position.y;
                holdingHose = false;
                manguera_end_reference = (GameObject) Instantiate(manguera_end, new Vector2(transform.position.x, transform.position.y), Quaternion.identity); //.transform.SetParent(GameObject.Find("Board").transform);
                Animator anim = manguera_end_reference.GetComponent<Animator>();

                int direction = 1;
                if (path[path.Count - 1] == "u")
                {
                    direction = 4;
                }
                else if (path[path.Count - 1] == "d")
                {
                    direction = 3;
                }
                else if (path[path.Count - 1] == "l")
                {
                    direction = 2;
                }
                anim.SetInteger("direction", direction);

                hoseItem.SetActive(false);

            }
            
        }
    }

    protected void AttemptMove(int xDir, int yDir)
    {

        if ((metersHose <= 0) && (xDir == 1 && path[path.Count - 1] != "l" || xDir == -1 && path[path.Count - 1] != "r" || yDir == 1 && path[path.Count - 1] != "d" || yDir == -1 && path[path.Count - 1] != "u")) return;

        if (GameManager.instance.boardScript.CanMoveTo(position.x + xDir, position.y + yDir, position.x, position.y))
        {
            playerTurnInCourse = true;
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);
            StartCoroutine(SmoothMovement(end));
            if (holdingHose)
            {
                metersHose--;
                hoseText.text = metersHose.ToString();
                GameObject toInstantiate = manguera_h;
                if (xDir == 1)
                {

                    if (path[path.Count - 1] == "u")
                    {
                        toInstantiate = manguera_rb;
                    }
                    else if (path[path.Count - 1] == "d")
                    {
                        toInstantiate = manguera_rt;
                    }
                    else if (path[path.Count - 1] == "l")
                    {
                        toInstantiate = manguera_empty;
                    }
                    path.Add("r"); //right
                    animator.SetTrigger("playerRight");
                    animatorHoseItem.SetTrigger("playerRight");

                }
                else if (xDir == -1)
                {
                    if (path[path.Count - 1] == "u")
                    {
                        toInstantiate = manguera_lb;
                    }
                    else if (path[path.Count - 1] == "d")
                    {
                        toInstantiate = manguera_lt;
                    }
                    else if (path[path.Count - 1] == "r")
                    {
                        toInstantiate = manguera_empty;
                    }
                    path.Add("l"); //left
                    animator.SetTrigger("playerLeft");
                    animatorHoseItem.SetTrigger("playerLeft");

                }
                else
                {
                    if (yDir == 1)
                    {
                        if (path[path.Count - 1] == "l")
                        {
                            toInstantiate = manguera_rt;
                        }
                        else if (path[path.Count - 1] == "r")
                        {
                            toInstantiate = manguera_lt;
                        }
                        else if (path[path.Count - 1] == "u")
                        {
                            toInstantiate = manguera_v;
                        }
                        else if (path[path.Count - 1] == "d")
                        {
                            toInstantiate = manguera_empty;
                        }
                        path.Add("u"); //up
                        animatorHoseItem.SetTrigger("playerBack");
                        animator.SetTrigger("playerBack");

                    }
                    else
                    {
                        if (path[path.Count - 1] == "l")
                        {
                            toInstantiate = manguera_rb;
                        }
                        else if (path[path.Count - 1] == "r")
                        {
                            toInstantiate = manguera_lb;
                        }
                        else if (path[path.Count - 1] == "d")
                        {
                            toInstantiate = manguera_v;
                        }
                        else if (path[path.Count - 1] == "u")
                        {
                            toInstantiate = manguera_empty;
                        }
                        path.Add("d"); //down
                        animator.SetTrigger("playerFront");
                        animatorHoseItem.SetTrigger("playerFront");

                    }
                }

                //Vector2 start = transform.position;
                //Vector2 end = start + new Vector2(xDir, yDir);
                RaycastHit2D hitManguera = Physics2D.Linecast(start, end, mangueraLayer);
                //Debug.DrawLine(start, end, Color.white, 2.5f, false);

                position.x += xDir;
                position.y += yDir;

                


                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);

                Instantiate(toInstantiate, new Vector2(transform.position.x, transform.position.y), Quaternion.identity).transform.SetParent(GameObject.Find("Board").transform);


                if (hitManguera.transform != null)
                {
                    pickingUpHose = true;
                    hitManguera.transform.localScale += new Vector3(1.0F, 0, 0);
                    Destroy(hitManguera.collider.gameObject);
                    RecullManguera(end, end);
                }
            }
            else
            {
                if (xDir == 1)
                {

                    animator.SetTrigger("playerRight");
                }
                else if (xDir == -1)
                {
                   
                    animator.SetTrigger("playerLeft");
                }
                else
                {
                    if (yDir == 1)
                    {
                      
                        animator.SetTrigger("playerBack");
                    }
                    else
                    {
                        animator.SetTrigger("playerFront");
                    }
                }
                position.x += xDir;
                position.y += yDir;
                if (endHose == position)
                {
                    holdingHose = true;
                    Destroy(manguera_end_reference);
                    hoseItem.SetActive(true);

                }
            }
        }

        CheckIfGameOver();
        //GameManager.instance.playersTurn = false;
    }

    public void endTurn()
    {
        GameManager.instance.playersTurn = false;
        playerTurnInCourse = false;
    }

    public void RecullManguera(Vector2 end, Vector2 pos)
    {
        metersHose++;
        string dir = path[path.Count - 1];
        path.RemoveAt(path.Count - 1);
        Vector2 to = pos;
        if (dir == "u")
        {
            to += new Vector2(0, -1);
        }
        else if (dir == "d")
        {
            to += new Vector2(0, 1);
        }
        else if (dir == "r")
        {
            to += new Vector2(-1, 0);
        }
        else if (dir == "l")
        {
            to += new Vector2(1, 0);
        }

        RaycastHit2D hitManguera = Physics2D.Linecast(pos, to, mangueraLayer);
        if (hitManguera.transform != null)
        {
            hitManguera.collider.gameObject.layer = 0;
            hoseList.Add(hitManguera.collider.gameObject);
            hoseAnim.Add(ChooseAnimation(dir, path[path.Count - 1]));
            RecullManguera(end, to);
            
           

        }
        else
        {
            hoseText.text = metersHose.ToString();
            StartCoroutine(AnimationHose());
            /*pickingUpHose = false;*/

        }
    }

    public IEnumerator AnimationHose()
    {
        for(int i = 0; i < hoseList.Count; i++)
        {
            Animator animatoraux = hoseList[i].GetComponent<Animator>();
            if(animatoraux != null)
            {
                animatoraux.SetInteger("grab", hoseAnim[i]);
                yield return new WaitForSeconds(0.1f);
            }
           
            Destroy(hoseList[i]);
        }
        hoseList.Clear();
        hoseAnim.Clear();
        pickingUpHose = false;
    
    }

    private int ChooseAnimation(string dir1, string dir2)
    {
        int anim = 2;

        if(dir1 == "r" && dir2 == "r" || dir1 == "u" && dir2 == "l" || dir1 == "d" && dir2 == "l" || dir1 == "u" && dir2 == "r" || dir1 == "l" && dir2 == "u" || dir1 == "u" && dir2 == "u")
        {
            anim = 1;
        }else if(dir1 == "r" && dir2 == "l" || dir1 == "l" && dir2 == "r" || dir1 == "u" && dir2 == "d" || dir1 == "d" && dir2 == "u")
        {
            anim = 3;
        }
        return anim;

    }


    private void Restart()
    {
        //Recarreguem l'escena
        SceneManager.LoadScene(0);
    }

    private void CheckIfGameOver()
    {
        if (temperature >= 100)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.muscicSource.Stop();
            GameManager.instance.peopleSaved = victims_total;
            GameManager.instance.GameOver();
        }
    }

    void ShootWater(int horizontal, int vertical)
    {
        GameObject[,] grid = GameManager.instance.boardScript.grid;

        grid[position.x + horizontal, position.y + vertical].GetComponent<Tile>().DrownTile();

        playerTurnInCourse = true;

    }

    public void IncreaseTemperature(int state)
    {
        switch (state)
        {
            case 0:
                temperature -= 20;
                if (temperature < 0) temperature = 0;
                break;
            case 3:
                temperature += 5;
                break;
            
            case 4:
                temperature += 10;
                break;
            default:
                break;
        }
        temperatureText.text = temperature.ToString();
    }

    public void SetPosition(int x, int y)
    {
        position.x = x;
        position.y = y;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
