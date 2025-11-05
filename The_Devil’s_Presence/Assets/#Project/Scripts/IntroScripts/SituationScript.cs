
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SituationScript : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject startButton;
    public GameObject quitButton;
    public TextMeshProUGUI introText;

    [Header("Paramètres")]
    [TextArea(3, 10)]
    public string texteIntro = 
        "Vous êtes une présence invisible.\n" +
        "Un esprit tapi dans l’ombre d’un nouvel arrivant.\n" +
        "Vos murmures influenceront son destin...";

    public float delayBeforeGame = 10f; // délai avant de lancer le jeu

    // Appelé quand on clique sur Start
    public void OnStartClicked()
    {
        // Cache les boutons
        startButton.SetActive(false);
        quitButton.SetActive(false);

        // Affiche le texte
        introText.text = texteIntro;
        introText.gameObject.SetActive(true);

        // Lance la coroutine qui charge le jeu après un délai
        StartCoroutine(LaunchGameAfterDelay());
    }

    IEnumerator LaunchGameAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGame);
        SceneManager.LoadScene("GameScene"); // nom exact de ta scène de jeu
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
