using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetScore : MonoBehaviour
{
    public GameObject player;
    public GameObject score;

    public void setScore(string playerName, string playerScore)
    {
        player.GetComponent<TextMeshProUGUI>().SetText(playerName);
        score.GetComponent<TextMeshProUGUI>().SetText(playerScore);
    }
}
