using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;      // Zone de texte pour afficher la question (si ça pète ici, on ne voit plus rien)
    [SerializeField] private List<Button> answerButtons;        // Les 3 boutons de réponses
    [SerializeField] private TextMeshProUGUI gaugeText;         // Affichage de la jauge (optionnel, peut disparaître plus tard)
    [SerializeField] private TextEffect typewriter;             // Effet d'écriture pour la question

    [Header("Données")]
    [SerializeField] private GameData gameData;                 // ScriptableObject qui contient toutes les questions / réponses

    [Header("Options")]
    [SerializeField] private bool randomizeAnswers = true;      // Si vrai, on mélange l’ordre des réponses (histoire de ne pas cliquer toujours sur le même bouton)

    private int currentIndex = 0;                               // Index de la question actuelle
    private int gauge = 0;                                      // Valeur de la jauge (emprise / karma chelou)
    private readonly List<int> displayOrder = new List<int>(3); // Ordre d’affichage des réponses (pour le mélange)

    [Header("Background")]
    [SerializeField] private Image backgroundImage;             // L’image UI qui affiche le fond (salon/chambre/cuisine)
    [SerializeField] private Sprite salonSprite;                // Sprite du salon
    [SerializeField] private Sprite chambreSprite;              // Sprite de la chambre
    [SerializeField] private Sprite cuisineSprite;              // Sprite de la cuisine

    void Start()
    {
        gauge = 0;
        currentIndex = 0;

        Debug.Log($"[GC] GameData utilisé = {gameData?.name} | questions={gameData?.questions?.Count}");

        UpdateGaugeUI();
        RenderCurrentQuestion();
    }

    void RenderCurrentQuestion()
    {
        // Si pas de données -> rideau.
        if (gameData == null || gameData.questions == null)
        {
            Debug.LogError("[GC] GameData ou sa liste de questions est null.");
            return;
        }

        // Si on a dépassé la dernière question -> on enchaîne sur la fin.
        if (currentIndex >= gameData.questions.Count)
        {
            ShowEnding();
            return;
        }

        var q = gameData.questions[currentIndex];

        if (q == null || q.reponses == null || q.reponses.Count == 0)
        {
            Debug.LogWarning($"[GC] Question {currentIndex} invalide, on saute.");
            currentIndex++;
            RenderCurrentQuestion();
            return;
        }

        // On coupe les boutons pendant que la phrase s'écrit (on est pas pressés).
        foreach (var btn in answerButtons)
        {
            if (btn) btn.gameObject.SetActive(false);
        }

        // Décor adapté à la question
        UpdateBackground(q.background);

        // On branche le callback : quand le texte est fini -> on affiche les réponses
        if (typewriter != null)
        {
            typewriter.OnFinished = () =>
            {
                ShowAnswers(q);
            };

            typewriter.Run(q.prompt);
        }
        else
        {
            // Si jamais le typewriter n’est pas branché, on fait un affichage brut + réponses directes.
            if (questionText != null)
                questionText.text = q.prompt;

            ShowAnswers(q);
        }
    }

    /// <summary>
    /// Affiche les réponses une fois que la question est complètement écrite.
    /// </summary>
    private void ShowAnswers(Questions q)
    {
        // Préparation de l’ordre d’affichage
        displayOrder.Clear();
        for (int i = 0; i < q.reponses.Count; i++)
            displayOrder.Add(i);

        if (randomizeAnswers)
        {
            for (int i = 0; i < displayOrder.Count; i++)
            {
                int r = Random.Range(i, displayOrder.Count);
                (displayOrder[i], displayOrder[r]) = (displayOrder[r], displayOrder[i]);
            }
        }

        // Distribution sur les boutons
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < q.reponses.Count)
            {
                var btn = answerButtons[i];
                if (!btn) continue;

                btn.gameObject.SetActive(true);

                int logicalIndex = displayOrder[i];
                var rep = q.reponses[logicalIndex];

                // On cherche d’abord un TextEffect sur le texte du bouton
                var labelEffect = btn.GetComponentInChildren<TextEffect>(true);
                if (labelEffect != null)
                {
                    labelEffect.Run(rep.text);
                }
                else
                {
                    var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (label) label.text = rep.text;
                }

                btn.onClick.RemoveAllListeners();
                int capturedIndex = logicalIndex;
                btn.onClick.AddListener(() => OnAnswerChosen(capturedIndex));
            }
            else
            {
                if (answerButtons[i])
                    answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnAnswerChosen(int logicalIndex)
    {
        if (gameData == null || gameData.questions == null) return;
        if (currentIndex < 0 || currentIndex >= gameData.questions.Count) return;

        var q = gameData.questions[currentIndex];
        if (q == null || q.reponses == null || logicalIndex < 0 || logicalIndex >= q.reponses.Count) return;

        var r = q.reponses[logicalIndex];

        Debug.Log($"[GC] Click: Q{currentIndex} R{logicalIndex} -> next={r.nextQuestion} (impact {r.impact})");

        gauge += r.impact;
        UpdateGaugeUI();

        if (r.nextQuestion == -2)
        {
            currentIndex = gameData.questions.Count;
            ShowEnding();
            return;
        }

        if (r.nextQuestion >= 0 && r.nextQuestion < gameData.questions.Count)
            currentIndex = r.nextQuestion;
        else
            currentIndex++;

        Debug.Log($"[GC] Next Q = {currentIndex}");

        if (currentIndex >= gameData.questions.Count)
        {
            ShowEnding();
            return;
        }

        RenderCurrentQuestion();
    }

    void UpdateGaugeUI()
    {
        if (gaugeText) gaugeText.text = $"Jauge : {gauge}";
    }

    void ShowEnding()
    {
        Debug.Log($"[Ending] gauge={gauge} | badMax={gameData.badEndingMax} | goodMin={gameData.goodEndingMin}");

        // Bonne fin
        if (gauge >= gameData.goodEndingMin)
        {
            GameResult.lastEnding = EndingType.Good;   // on stocke la fin
            SceneManager.LoadScene("EndScene");        // on charge la même scène
            return;
        }

        // Mauvaise fin
        if (gauge <= gameData.badEndingMax)
        {
            GameResult.lastEnding = EndingType.Bad;    // on stocke la fin
            SceneManager.LoadScene("EndScene");
            return;
        }

        // Fin neutre
        GameResult.lastEnding = EndingType.Neutral;
        SceneManager.LoadScene("EndScene");
    }


    private void UpdateBackground(BackgroundType type)
    {
        if (backgroundImage == null) return;

        switch (type)
        {
            case BackgroundType.Salon:
                backgroundImage.sprite = salonSprite;
                break;
            case BackgroundType.Chambre:
                backgroundImage.sprite = chambreSprite;
                break;
            case BackgroundType.Cuisine:
                backgroundImage.sprite = cuisineSprite;
                break;
        }
    }
}
