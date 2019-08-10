using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScore : MonoBehaviour
{
    private string NuevaPuntuacionURL = "https://firedousers.000webhostapp.com/RankingScripts/newScore.php?";
    //private string NuevaPuntuacionURL = "http://roguelike.dx.am/newScore.php?";
    private string claveSecreta = "dcbkx12fjea59";

    // Start is called before the first frame update
    void Start()
    {
        string nick = this.GetComponent<RankingManager>().nick;
        StartCoroutine(EnviarJugadores(nick, GameManager.instance.peopleSaved));  
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

    IEnumerator EnviarJugadores(string nombreJugador, int puntuacionJugador)
    {
        string hash = Md5Sum(nombreJugador + puntuacionJugador + claveSecreta);
        string PostURL = NuevaPuntuacionURL + "player=" + WWW.EscapeURL(nombreJugador) + "&score=" + puntuacionJugador + "&hash=" + hash;
        WWW DataPost = new WWW(PostURL);
        yield return DataPost;

        if(DataPost.error != null)
        {
            print("Problema al enviar la puntuació a la bbdd   " + DataPost.error);
        }
        else
        {
            this.GetComponent<LoadRanking>().enabled = true;
            Debug.Log((System.Text.Encoding.UTF8.GetString(DataPost.bytes)));
        }
    }
}
