using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> answerButtons;    // 3 boutons
    [SerializeField] private TextMeshProUGUI gaugeText;     // optionnel

    [Header("Données")]
    [SerializeField] private GameData gameData;

    [Header("Options")]
    [SerializeField] private bool randomizeAnswers = true;
    [SerializeField] private string badEndingScene = "BadEnd";
    [SerializeField] private string neutralEndingScene = "NeutralEnd";
    [SerializeField] private string goodEndingScene = "GoodEnd";

    private int currentIndex = 0;
    private int gauge = 0;
    private readonly List<int> displayOrder = new List<int>(3);

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
        if (gameData == null || gameData.questions == null)
        {
            Debug.LogError("[GC] GameData ou sa liste de questions est null.");
            return;
        }

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

        if (questionText != null) questionText.text = q.prompt;

        // prépare l'ordre d’affichage
        displayOrder.Clear();
        for (int i = 0; i < q.reponses.Count; i++) displayOrder.Add(i);
        if (randomizeAnswers)
        {
            for (int i = 0; i < displayOrder.Count; i++)
            {
                int r = Random.Range(i, displayOrder.Count);
                (displayOrder[i], displayOrder[r]) = (displayOrder[r], displayOrder[i]);
            }
        }

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < q.reponses.Count)
            {
                var btn = answerButtons[i];
                if (!btn) continue;

                btn.gameObject.SetActive(true);

                int logicalIndex = displayOrder[i];
                var rep = q.reponses[logicalIndex];

                var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
                if (label) label.text = rep.text;

                btn.onClick.RemoveAllListeners();
                int capturedIndex = logicalIndex;
                btn.onClick.AddListener(() => OnAnswerChosen(capturedIndex));
            }
            else
            {
                if (answerButtons[i]) answerButtons[i].gameObject.SetActive(false);
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
        //FIN ANTICIPÉE : -2 => on évalue la fin directement maintenant
        if (r.nextQuestion == -2)
        {
            currentIndex = gameData.questions.Count; // force la condition de fin
            ShowEnding();
            return;
        }

        if (r.nextQuestion >= 0 && r.nextQuestion < gameData.questions.Count)
        {
            currentIndex = r.nextQuestion; // saut explicite
        }
        else
        {
            currentIndex++; // enchaînement linéaire si nextQuestion est invalide (-1 ou hors plage)
        }

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

        if (gauge >= gameData.goodEndingMin)
        {
            SceneManager.LoadScene(goodEndingScene);
            return;
        }

        if (gauge <= gameData.badEndingMax)
        {
            SceneManager.LoadScene(badEndingScene);
            return;
        }

        SceneManager.LoadScene(neutralEndingScene);
    }
}
