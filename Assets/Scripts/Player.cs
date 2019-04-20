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
    public Animator temperatureHUD;

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
    public GameObject hoseHUD;
    public GameObject peopleHUD;
    private bool hasKey;
    public bool hasAxe;
    private List<string> path = new List<string>();
    
    public Vector2Int position;
    private Vector2Int endHose; 

    private bool pickingUpHose;
    private bool playerTurnInCourse;

    private List<GameObject> hoseList;
    private List<int> hoseAnim;
    private bool holdingHose; // false quan has deixat anar la manguera
    private GameObject manguera_end_reference;

    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
    bool damaged;

   
    


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

    public bool carryVictim(int state)
    {
        if (victims == 0)
        {
            victims++;
            peopleHUD.GetComponent<HudPeople>().ChangeSprite(state);

            return true;
        }
        return false;
    }

    public void saveVictim()
    {
        victims_total += victims;
        victims = 0;
        peopleHUD.GetComponent<HudPeople>().ChangeSprite(-1);
        peopleText.text = victims_total.ToString();
    }

    private void Update()
    {
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;

        //Si no es el torn sortim de la funcio
        if (!GameManager.instance.playersTurn) return;

        if (pickingUpHose || playerTurnInCourse) return;
        if (GameManager.instance.pause) return;

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
                if(GameManager.instance.waterRecharges > 0)
                {
                    if (horizontal == 1)
                    {
                        animatorWater.SetTrigger("right");
                        animator.SetTrigger("waterRight");
                        animatorHoseItem.SetTrigger("waterRight");

                    }
                    else if (horizontal == -1)
                    {
                        animatorWater.SetTrigger("left");
                        animator.SetTrigger("waterLeft");
                        animatorHoseItem.SetTrigger("waterLeft");


                    }
                    else if (vertical == -1)
                    {
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
                    GameManager.instance.waterRecharges -= 1;
                    GameManager.instance.turndeposit = 0;
                }
                
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
                hoseHUD.GetComponent<HoseHUD>().changeSprite(metersHose, GameManager.instance.totalHoseMeters);
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
        else
        {
            GameManager.instance.boardScript.BMExecutePreBehaviour(position.x + xDir, position.y + yDir);
            if(!GameManager.instance.boardScript.IsNotStucked(position.x, position.y))
            {
                temperature = 100;
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
            hoseHUD.GetComponent<HoseHUD>().changeSprite(metersHose, GameManager.instance.totalHoseMeters);
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

    public void CheckIfGameOver()
    {
        if (temperature >= 100)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.muscicSource.Stop();
            GameManager.instance.peopleSaved = victims_total;
            pickingUpHose = true;
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
                damaged = true;
                temperature += 5;
                break;
            
            case 4:
                damaged = true;
                temperature += 10;
                break;
            default:
                break;
        }
        temperatureText.text = temperature.ToString();

        if (temperature < 11)
        {
            temperatureHUD.SetTrigger("temperature10");
            return;
        }
        if (temperature < 21)
        {
            temperatureHUD.SetTrigger("temperature20");
            return;
        }
        if (temperature < 31)
        {
            temperatureHUD.SetTrigger("temperature30");
            return;
        }
        if (temperature < 41)
        {
            temperatureHUD.SetTrigger("temperature40");
            return;
        }
        if (temperature < 51)
        {
            temperatureHUD.SetTrigger("temperature50");
            return;
        }
        if (temperature < 61)
        {
            temperatureHUD.SetTrigger("temperature60");
            return;
        }
        if (temperature < 71)
        {
            temperatureHUD.SetTrigger("temperature70");
            return;
        }
        if (temperature < 81)
        {
            temperatureHUD.SetTrigger("temperature80");
            return;
        }
        if (temperature < 91)
        {
            temperatureHUD.SetTrigger("temperature90");
            return;
        }
        if (temperature < 101)
        {
            temperatureHUD.SetTrigger("temperature100");
            return;
        }

    }

    public void SetPosition(int x, int y)
    {
        position.x = x;
        position.y = y;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
