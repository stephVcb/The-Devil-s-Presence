using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTransition : MonoBehaviour
{
    [Header("UI Références")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private TMP_Text introText; // ton texte de contexte (désactivé par défaut)
    [SerializeField] private GameObject[] objectsToHide;
        [SerializeField] private GameObject skipButton;

    [Header("Paramètres")]
    [TextArea(3, 10)]
    public string texteIntro = 
        "Vous êtes une présence invisible.\n" +
        "Un esprit tapi dans l’ombre d’un nouvel arrivant.\n" +
        "Vos murmures influenceront son destin…";

    [SerializeField] private float delayBeforeGame = 10f; // durée avant lancement de la GameScene/ du jeu 
    [SerializeField] private string nextScene = "GameScene"; 

    // Appelé par le bouton Start
    public void OnStartClicked()
    {
        // Cache les boutons
        startButton.SetActive(false);
        quitButton.SetActive(false);
        // Cache les objets au démarrage du texte (comme le WARNING, les boutons, etc.)
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Active le texte narratif
        introText.text = texteIntro;
        introText.gameObject.SetActive(true);

        //Afficher le bouton skip, pour passer l'intro du jeux 
        if (skipButton != null)
        skipButton.SetActive(true);

        // Lance la coroutine de transition
        StartCoroutine(LaunchGameAfterDelay());
    }

    private IEnumerator LaunchGameAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeGame);
//Bloc de débug pendant le dev du jeu
//ne s'execute que dans l'éditeur unity donc pas dans la compilation
#if UNITY_EDITOR
        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError($"Scène '{nextScene}' non trouvée dans la Build List !");
            yield break;
        }
#endif

        SceneManager.LoadScene(nextScene);
    }

    // Appelé par le bouton Quit
    public void OnQuitClicked()
    {
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        yield return new WaitForSeconds(1f);
        //idem que plus haut 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnSkipClicked()
    {
        SceneManager.LoadScene(nextScene);
    }

}
