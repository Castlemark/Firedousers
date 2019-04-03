﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public float levelStartDelay = 2f;  //segons que dura la transició entre un nivell i el seguent

    public float turnDelay = 50f;                          //Delay entre cada torn del jugador

    public Text levelText;
    private GameObject levelImage;
    public static GameManager instance = null; //singleton
    public BoardManager boardScript;
    public int level = 1;
    private bool doingSetup;    //prevent a l'usuari de moure's quan estem establint el tauler
    private bool enemiesMoving;
    private bool firstRun = true;

    public List<Vector3> stairsUpPositions = new List<Vector3>();

    public int playerHoseMeters = 100;
    public int peopleSaved = 0;
    public int playerVictims;
    public int playerVictimsTotal;
    public bool playerHasKey;
    [HideInInspector] public bool playersTurn = true;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    //S'executa cada cop que s'ha carregat una escena
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode
    mode)
    {
        if (firstRun)
        {
            firstRun = false;
            return;
        }

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

    public void ChangeLevel(int increment)
    {
        doingSetup = true;
        level += increment;
        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay); // executa la funció despres del Delay que li hem dit: 2 segons
        boardScript.SetupScene(level, increment);
        GameObject.Find("CamerasParent").GetComponent<CameraFollow>().ChangeLevel(increment, boardScript.columns);
    }

    public void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay); // executa la funció despres del Delay que li hem dit: 2 segons
        boardScript.SetupScene(level, 0);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "Game Over!";
        levelImage.SetActive(true);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }
        else
        {
            StartCoroutine(MoveEnemies());
        }
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        boardScript.updateFire();
        playersTurn = true;
        enemiesMoving = false;
    }
}
