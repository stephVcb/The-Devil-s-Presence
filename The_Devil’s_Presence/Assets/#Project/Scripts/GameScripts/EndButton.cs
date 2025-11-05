using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButtons : MonoBehaviour
{

    // Retourne à la scène d’intro
    public void BackToIntro()
    {
        // Remettre le jeu à 0
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("SceneIntro");
    }
}
