using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartScript : MonoBehaviour
{

    [SerializeField] private TMP_Text label;
    [SerializeField] private float delay = 3f;
    private string nomScene = "GameScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void ClickStart()
    {
        label.SetText("Ceci est votre choix, commençons ");
        Invoke("CommencerJeux", delay);

    }
    void CommencerJeux()
    {
        // Charge la scène spécifiée.
        SceneManager.LoadScene(nomScene, LoadSceneMode.Single);
    }

}