using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Gère l'affichage de la scène de fin (fond + texte selon la fin obtenue)
public class EndSceneController : MonoBehaviour
{
    [Header("UI Visuelle")]
    [SerializeField] private Image backgroundImage;      // Image de fond à changer selon la fin
    [SerializeField] private Sprite badSprite;           // Sprite à afficher pour la mauvaise fin
    [SerializeField] private Sprite neutralSprite;       // Sprite pour la fin neutre
    [SerializeField] private Sprite goodSprite;          // Sprite pour la bonne fin

    [Header("Texte de fin")]
    [SerializeField] private TextEffect endTextEffect;   // Effet lettre par lettre pour le texte
    [SerializeField] private TextMeshProUGUI endTextTMP; // Plan B : texte brut si pas d’effet

    [Header("Bouton de retour")]
    [SerializeField] private GameObject menuButton;      // Le bouton "Menu" à afficher à la fin
    [SerializeField] private string menuSceneName = "SceneIntro"; // Nom de la scène du menu

    [Header("Musique de fin")]
    [SerializeField] private AudioClip badEndingClip;     // son pour la mauvaise fin
    [SerializeField] private AudioClip neutralEndingClip; // son pour la fin neutre
    [SerializeField] private AudioClip goodEndingClip;    // son pour la bonne fin
    [SerializeField] private AudioSource audioSource;     // source pour jouer la musique de fin

    private void Start()
    {
        string message = "";
        Sprite spriteToUse = null;

        // Choix du message + visuel selon la fin atteinte
        switch (GameResult.lastEnding)
        {
            case EndingType.Bad:
                message = "Misérable vermine...\nVoilà qu'un prêtre débarque!!!!! je me sens partir et retourner dans l'entre des Enfer...";
                spriteToUse = badSprite;
                break;

            case EndingType.Good:
                message = "te voilà pendouillant.. Il ne t'a pas fallu grand chose pour te détruire...\nHâte de rencontrer la prochaine victime.";
                spriteToUse = goodSprite;
                break;

            default: // Neutral
                message = "Il survivra.\nMais il ne sera plus jamais le même.";
                spriteToUse = neutralSprite;
                break;
        }

        // On cache le bouton Menu pendant que le texte s'écrit
        if (menuButton != null)
            menuButton.SetActive(false);

        // Application de l’image de fond
        if (backgroundImage != null && spriteToUse != null)
            backgroundImage.sprite = spriteToUse;

        // Affichage du texte avec effet
        if (endTextEffect != null)
        {
            // Quand l'effet a fini d'écrire → on affiche enfin le bouton Menu
            if (menuButton != null)
            {
                endTextEffect.OnFinished += () =>
                {
                    menuButton.SetActive(true);
                };
            }

            endTextEffect.Run(message);
        }
        else if (endTextTMP != null)
        {
            // Plan B : pas d'effet, on balance tout le texte d'un coup
            endTextTMP.text = message;

            // Du coup, on peut afficher le bouton tout de suite
            if (menuButton != null)
                menuButton.SetActive(true);
        }

        // choisir la bonne musique de fin (bad / neutral / good)
        // on passe par l’AudioManager pour avoir le fade-out + fade-in propre
        switch (GameResult.lastEnding)
        {
            case EndingType.Bad:
                AudioManager.Instance.PlayEndingMusic("Bad");
                break;

            case EndingType.Good:
                AudioManager.Instance.PlayEndingMusic("Good");
                break;

            default: // Neutral
                AudioManager.Instance.PlayEndingMusic("Neutral");
                break;
        }


    }

    // Action du bouton “Retour Accueil”
    public void OnClickReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
