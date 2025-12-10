using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private TextEffect transitionEffect;   // même TextMeshPro que Warning
    [TextArea] public string transitionText;               // texte de transition
    [SerializeField] private GameObject startButton;       // pour les cacher
    [SerializeField] private GameObject quitButton;
    [SerializeField] private string gameSceneName = "GameScene";

    public void OnClickStart()
    {
        // Cache les boutons
        startButton.SetActive(false);
        quitButton.SetActive(false);

        // Quand la transition est finie → on lance le jeu
        transitionEffect.OnFinished += LoadGame;

        // Lance le texte de transition
        transitionEffect.Run(transitionText);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
