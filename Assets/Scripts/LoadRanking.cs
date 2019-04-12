using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadRanking : MonoBehaviour
{
    private string URLLoadRanking = "https://firedousers.000webhostapp.com/RankingScripts/score.php";
    //private string URLLoadRanking = "http://roguelike.dx.am/score.php";
    private List<Jugador> rankingPlayers = new List<Jugador>();
    private string[] CurrentArray = null;
    public Transform tfPanelData;
    public GameObject textLoading;
    private TextMeshProUGUI txtCargando;
    public GameObject PanelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        txtCargando = textLoading.GetComponent<TextMeshProUGUI>();
        StartCoroutine(ObtenerJugadores());
    }

    IEnumerator ObtenerJugadores()
    {
        txtCargando.SetText("CARGANDO \n ...");
        WWW DataServer = new WWW(URLLoadRanking);
        yield return DataServer;

        if(DataServer.error != null)
        {
            Debug.Log("Problema al intentar obtenir els jugadors de la bbdd:  " + DataServer.error);
            txtCargando.SetText(DataServer.error);
        }
        else
        {
            txtCargando.SetText("");
            ObtenerRegistros(DataServer);
            VerRegistros();
        }
    }

    void ObtenerRegistros(WWW DataServer)
    {
        Debug.Log(System.Text.Encoding.UTF8.GetString(DataServer.bytes));
        CurrentArray = System.Text.Encoding.UTF8.GetString(DataServer.bytes).Split(";"[0]);

        for(int i = 0; i<= CurrentArray.Length - 3; i = i + 2)
        {
            rankingPlayers.Add(new Jugador(CurrentArray[i], CurrentArray[i + 1]));
        }
    }

    void VerRegistros()
    {
        for(int i = 0; i < rankingPlayers.Count; i++)
        {
            GameObject obj = Instantiate(PanelPrefab);
            Jugador jg = rankingPlayers[i];
            obj.GetComponent<SetScore>().setScore(jg.nombre, jg.puntuacion);
            obj.transform.SetParent(tfPanelData);
            obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        }
    }

}

public class Jugador
{
    public string puntuacion;
    public string nombre;

    public Jugador(string nombreJugador, string puntuacionJugador)
    {
        puntuacion = puntuacionJugador;
        nombre = nombreJugador;
    }
}
