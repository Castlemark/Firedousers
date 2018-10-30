using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;  //segons que dura la transició entre un nivell i el seguent

    public float turnDelay = 0.1f;                          //Delay entre cada torn del jugador

    private Text levelText;
    private GameObject levelImage;
    public static GameManager instance = null; //singleton
    public BoardManager boardScript;
    private int level = 1;
    private bool doingSetup;    //prevent a l'usuari de moure's quan estem establint el tauler
    private List<Enemy> enemies;        //llista d'enemics a l'escena 
    private bool enemiesMoving;
    private bool firstRun = true;

    public int playerFoodPoints = 3;
    public int peopleSaved = 0;
    [HideInInspector] public bool playersTurn = true;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}
    //S'execura cada cop que s'ha carregat una escena
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode
    mode)
    {
        if (firstRun)
        {
            firstRun = false;
            return;
        }

        level++;
        InitGame();
    }
    void OnEnable()
    {
        //Activem el listener perque s'executi onLevelFinishedLoading quan hi hagi un canvi en le'escena
        SceneManager.sceneLoaded += OnLevelFinishedLoading; 
    }

    void OnDisable()
    {
        //Desactivem el listener quan deshabilitem l'escena
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay); // executa la funció despres del Delay que li hem dit: 2 segons
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "¡Has durao " + level + " días güey!";
        levelImage.SetActive(true);
        enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }
        else
        {
            StartCoroutine(MoveEnemies());
        }
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        //al primer nivell no hi haurà enemics i per això hem de controlar el temps de torn especialment.
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
