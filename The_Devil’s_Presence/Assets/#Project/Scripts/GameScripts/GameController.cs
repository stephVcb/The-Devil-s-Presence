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
    [SerializeField] private bool useEndingScenes = false;
    [SerializeField] private string badEndingScene = "End_Bad";
    [SerializeField] private string neutralEndingScene = "End_Neutral";
    [SerializeField] private string goodEndingScene = "End_Good";

    private int currentIndex = 0;
    private int gauge = 0;
    private readonly List<int> displayOrder = new List<int>(3);

    void Start()
    {
        gauge = 0;
        currentIndex = 0;
        UpdateGaugeUI();
        RenderCurrentQuestion();
    }

    void RenderCurrentQuestion()
    {
        if (currentIndex >= gameData.questions.Count)
        {
            ShowEnding();
            return;
        }

        var q = gameData.questions[currentIndex];
        if (q == null || q.reponses == null || q.reponses.Count == 0)
        {
            Debug.LogWarning($"[GameController] Question {currentIndex} invalide, on saute.");
            currentIndex++;
            RenderCurrentQuestion();
            return;
        }

        questionText.text = q.prompt;

        // prépare ordre d’affichage
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
                btn.gameObject.SetActive(true);

                int logicalIndex = displayOrder[i];
                var rep = q.reponses[logicalIndex];

                var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
                if (label) label.text = rep.text;

                btn.onClick.RemoveAllListeners();
                int capturedImpact = rep.impact;
                btn.onClick.AddListener(() => OnAnswerChosen(capturedImpact));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnAnswerChosen(int impact)
    {
        gauge += impact;
        UpdateGaugeUI();
        currentIndex++;
        RenderCurrentQuestion();
    }

    void UpdateGaugeUI()
    {
        if (gaugeText) gaugeText.text = $"Jauge : {gauge}";
    }

    void ShowEnding()
    {
        if (gauge <= gameData.badEndingMax)
        {
            if (useEndingScenes) SceneManager.LoadScene(badEndingScene);
            else EndInline("Fin : Mauvaise 💀");
            return;
        }
        if (gauge <= gameData.neutralEndingMax)
        {
            if (useEndingScenes) SceneManager.LoadScene(neutralEndingScene);
            else EndInline("Fin : Neutre 😐");
            return;
        }
        if (useEndingScenes) SceneManager.LoadScene(goodEndingScene);
        else EndInline("Fin : Bonne 🌞");
    }

    void EndInline(string endMessage)
    {
        questionText.text = endMessage + $"\n\n(SCORE FINAL : {gauge})";
        foreach (var btn in answerButtons) btn.gameObject.SetActive(false);
    }
}
