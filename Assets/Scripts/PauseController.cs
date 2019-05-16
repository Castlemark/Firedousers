using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject player;
    private bool canPause;
    // Start is called before the first frame update
    void Start()
    {
        canPause = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (canPause)
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                GameManager.instance.pause = !GameManager.instance.pause;
                canPause = false;
            }
            
        }
        else
        {
            canPause = true;
        }
        
    }

    public void restart()
    {
        pauseMenu.SetActive(false);
        GameManager.instance.pause = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MENU");
    }

    public void Forfeit()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        GameManager.instance.pause = !GameManager.instance.pause;
        canPause = false;
        player.GetComponent<Player>().EndGame();
    }
}
