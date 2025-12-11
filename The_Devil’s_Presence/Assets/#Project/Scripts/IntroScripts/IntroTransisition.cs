using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroTransition : MonoBehaviour
{
    // [Header("Son")]
    // public AudioFader introAudioFader;

    [Header("UI Références")]
    [SerializeField] private TextMeshProUGUI introText;    // zone texte utilisée pour le warning + transition
    [SerializeField] private GameObject[] objectsToHide;   // warningBubble, etc
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject skipButton;        // bouton skip (pour sauter la transition)

    [Header("Texte de transition")]
    [TextArea] public string transitionText;               // texte affiché après le fade démon

    [Header("Référence du fade démon")]
    [SerializeField] private FadeImage fadeImage;

    [Header("Timings du démon")]
    [SerializeField] private float demonFadeDuration = 4f; // vitesse apparition/disparition
    [SerializeField] private float demonHoldDuration = 2f; // temps où le démon reste visible

    [Header("Fin de transition")]
    [SerializeField] private float delayBeforeGame = 1f;   // petit délai avant d’aller dans la scène du jeu
    [SerializeField] private string nextScene = "GameScene";
    

    void Start()
    {
        //jouer la musique
        AudioManager.Instance.PlayAmbience("Intro");
    }
    private void Awake()
    {
        // bouton skip caché au lancement
        if (skipButton != null)
            skipButton.SetActive(false);
    }



    // quand on clique sur "Start"
    public void OnStartClicked()
    {
        // on désactive l'intro animator pour éviter que le warning se relance
        var animator = GetComponent<IntroAnimator>();
        if (animator != null)
            animator.enabled = false;

        // on cache start et quit
        if (startButton != null) startButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        // on vide le warning (surtout pas désactiver la zone sinon le TextEffect ne pourra pas écrire dedans)
        if (introText != null)
            introText.text = "";

        // lancement de la transition complète
        StartCoroutine(PlayTransition());
    }



    // quitter le jeu
    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }



    // bouton skip → sauter toute la transition
    public void OnSkipClicked()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(nextScene);
    }



    private IEnumerator PlayTransition()
    {
        // on cache tous les objets définis (warning bubble, etc)
        foreach (var obj in objectsToHide)
            if (obj != null)
                obj.SetActive(false);

        // animation démon
        if (fadeImage != null)
        {
            fadeImage.fadeDuration = demonFadeDuration;
            fadeImage.FadeIn();
            yield return new WaitForSeconds(demonFadeDuration);

            yield return new WaitForSeconds(demonHoldDuration);

            fadeImage.FadeOut();
            yield return new WaitForSeconds(demonFadeDuration);
        }

        // maintenant que la partie démon est finie, on affiche skip
        if (skipButton != null)
            skipButton.SetActive(true);

        // affichage du texte de transition avec effet
        if (introText != null)
        {
            var effect = introText.GetComponent<TextEffect>();

            if (effect != null)
            {
                bool finished = false;

                effect.OnFinished = null;            // reset sécurité
                effect.OnFinished += () => finished = true;

                effect.Run(transitionText);

                yield return new WaitUntil(() => finished);
            }
            else
            {
                introText.text = transitionText;
            }
        }

        // petit délai avant entrée dans la scène du jeu
        yield return new WaitForSeconds(delayBeforeGame);

        SceneManager.LoadScene(nextScene);
    }
}
