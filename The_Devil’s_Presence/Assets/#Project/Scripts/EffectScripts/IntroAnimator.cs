using UnityEngine;

public class IntroAnimator : MonoBehaviour
{
    [Header("Effets")]
    [SerializeField] private TextEffect warningEffect;

    [Header("Texte du warning")]
    [TextArea] public string warningText;

    [Header("Boutons")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject skipButton;

    private bool buttonsShown = false;

    private void Start()
    {
        // Cache les boutons au début
        if (startButton != null) startButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false);

        // Quand le warning est fini → on montre les boutons
        if (warningEffect != null)
        {
            warningEffect.OnFinished -= ShowButtons;
            warningEffect.OnFinished += ShowButtons;

            // Lance l'affichage du warning
            warningEffect.Run(warningText);
        }
    }

    private void ShowButtons()
    {
        if (buttonsShown) return;
        buttonsShown = true;

        if (startButton != null) startButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);
        if (skipButton != null) skipButton.SetActive(true);
    }
}
