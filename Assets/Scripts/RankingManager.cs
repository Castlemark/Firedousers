using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingManager : MonoBehaviour
{

    public GameObject input;
    public GameObject panelInput;
    public GameObject panelRanking;
    public GameObject score;

    public string nick;
    // Start is called before the first frame update
    void Start()
    {
        string scoreText = GameManager.instance.peopleSaved.ToString();
        score.GetComponent<TextMeshProUGUI>().SetText(scoreText);
        input.GetComponent<TMP_InputField>().Select();
        input.GetComponent<TMP_InputField>().ActivateInputField();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nick = input.GetComponent<TMP_InputField>().text;
            panelInput.SetActive(false);
            panelRanking.SetActive(true);
            this.GetComponent<SaveScore>().enabled = true;
        }
        
    }
}
