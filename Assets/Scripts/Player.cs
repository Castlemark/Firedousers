using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public Text peopleText;

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

    public LayerMask mangueraLayer;

    private SpriteRenderer spriteRenderer;
    private int victims; //Víctimes que portes a sobre
    private int victims_total;
    private int maxVictims = 1;
    public Sprite spriteWithVictim;
    public Sprite spriteWithoutVictim;

    private Animator animator;
    private int food;
    private bool hasKey;
    private List<string> path = new List<string>();

    private List<GameObject> visibilityTiles;

    // Use this for initialization
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = food.ToString();
        victims = GameManager.instance.playerVictims;
        victims_total = GameManager.instance.playerVictimsTotal;
        peopleText.text = victims_total.ToString();
        hasKey = GameManager.instance.playerHasKey;

        foodText.text = food.ToString();
        path.Add("r");
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (victims > 0) spriteRenderer.sprite = spriteWithVictim;

        visibilityTiles = GetLosObjects();
    }

    // S'ecexuta quan es deshabilita el game object quan es canvia de nivell
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
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
        UpdateVisibility(visibilityTiles, this.gameObject);

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
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        if ((food <= 0) && (xDir == 1 && path[path.Count - 1] != "l" || xDir == -1 && path[path.Count - 1] != "r" || yDir == 1 && path[path.Count - 1] != "d" || yDir == -1 && path[path.Count - 1] != "u")) return;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            food--;
            foodText.text = food.ToString();
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
                path.Add("r"); //right
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
                path.Add("l"); //left
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
                    path.Add("u"); //up
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
                    path.Add("d"); //down
                }
            }

            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);
            RaycastHit2D hitManguera = Physics2D.Linecast(start, end, mangueraLayer);
            Debug.DrawLine(start, end, Color.white, 2.5f, false);

            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);

            Instantiate(toInstantiate, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            if (hitManguera.transform != null)
            {
                hitManguera.transform.localScale += new Vector3(1.0F, 0, 0);
                hitManguera.collider.gameObject.SetActive(false);
                RecullManguera(end, end);
            }
        }
        else
        {
            if (hit.collider.tag == "Door" || (hit.collider.tag == "LockedDoor" && hasKey))
            {
                Door door = hit.collider.gameObject.GetComponent<Door>();
                door.openDoor();
            }
        }
        //CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void RecullManguera(Vector2 end, Vector2 pos)
    {
        food++;
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
            hitManguera.collider.gameObject.SetActive(false);
            RecullManguera(end, to);
        }
        else
        {
            foodText.text = food.ToString();
        }
    }

    //Al haver posat els colliders a Trigger aquesta funcio de la APi de Unity s'executa quan colisiona cotra food, soda o exit
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "StairsUp")
        {
            Invoke("Restart", restartLevelDelay);
            GameManager.instance.level++;
            GameManager.instance.lastStairs = "up";
            enabled = false;
        }
        else if (other.tag == "StairsDown")
        {
            Invoke("Restart", restartLevelDelay);
            GameManager.instance.level--;
            GameManager.instance.lastStairs = "down";
            enabled = false;
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Victim" && victims < maxVictims)
        {
            carryVictim();
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "SafePoint" && victims != 0)
        {
            saveVictim();
        }
        else if (other.tag == "Key")
        {
            hasKey = true;
            other.gameObject.SetActive(false);
            //Destroy(other.gameObject);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }


    private void Restart()
    {
        //Recarreguem l'escena
        SceneManager.LoadScene(0);
    }

    //s'executa quan un enemic et pega
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;

        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.muscicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    public void UpdateVisibility(List<GameObject> losObjects, GameObject player)
    {
        foreach (GameObject losObject in losObjects)
        {
            Vector2 origin = (Vector2)losObject.transform.position;
            Vector2 destination = (Vector2)player.transform.position;

            bool hasVisibility = losObject.GetComponent<tileSeen>() != null;

            RaycastHit2D hit;
            if (losObject.GetComponent<BoxCollider2D>() != null)
            {
                losObject.GetComponent<BoxCollider2D>().enabled = false;
                hit = Physics2D.Raycast(origin, (destination - origin), 5.0f);
                losObject.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                hit = Physics2D.Raycast(origin, (destination - origin), 5.0f);
            }

            //if the ray hit nothing (died)
            if (hit.collider == null)
            {

                if (hasVisibility && losObject.GetComponent<tileSeen>().alreadySeen == true)
                {
                    losObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
                    losObject.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    losObject.GetComponent<SpriteRenderer>().enabled = false;
                }

            }
            //if the ray hit anything else
            else if (hit.collider.gameObject.name != "Player")
            {
                if (hasVisibility && losObject.GetComponent<tileSeen>().alreadySeen == true)
                {
                    losObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .5f);
                    losObject.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    losObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            //if the ray hit the player 
            else
            {
                losObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                losObject.GetComponent<SpriteRenderer>().enabled = true;

                if (hasVisibility)
                {
                    losObject.GetComponent<tileSeen>().alreadySeen = true;
                }
            }
        }
    }

    // All LOS objects should be children of Board GameObject
    public List<GameObject> GetLosObjects()
    {
        Transform[] allChildren = GameObject.Find("Board").GetComponentsInChildren<Transform>();
        List<GameObject> childObjects = new List<GameObject>();

        bool first = true;

        foreach (Transform child in allChildren)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                childObjects.Add(child.gameObject);
            }
        }

        return childObjects;
    }
}
