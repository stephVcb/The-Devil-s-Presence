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
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

void OnAnswerChosen(int logicalIndex)
{
    //éviter les bugs d’index 
    if (currentIndex < 0 || currentIndex >= gameData.questions.Count) return;

    var q = gameData.questions[currentIndex];
    if (q == null || q.reponses == null || logicalIndex < 0 || logicalIndex >= q.reponses.Count) return;

    var r = q.reponses[logicalIndex];

    //  on applique l'impact de la réponse sur la jauge 
    gauge += r.impact;
    UpdateGaugeUI(); // (si tu as une fonction d’affichage de la jauge)

    // on décide quelle question vient ensuite 
    if (r.nextQuestion >= 0 && r.nextQuestion < gameData.questions.Count)
    {
        // saute vers une question précise (Q1, Q2, etc.)
        currentIndex = r.nextQuestion;
    }
    else
    {
        // question suivante dans la liste
        currentIndex++;
    }

    //affichage de la fin selon la jauge 
    if (currentIndex >= gameData.questions.Count)
    {
        ShowEnding(); 
        return;
    }

    //affiche la question suivante
    RenderCurrentQuestion();
}

    //affichage de la jauge pdt le jeu==> sert au debug 
    void UpdateGaugeUI()
    {
        if (gaugeText) gaugeText.text = $"Jauge : {gauge}";
    }

    // méthode qui charge Bad/Neutral/Good
void ShowEnding()
    {
        Debug.Log($"[Ending] gauge={gauge} | badMax={gameData.badEndingMax} | goodMin={gameData.goodEndingMin}");
    // Good si on atteint le seuil "bon"
    if (gauge >= gameData.goodEndingMin)
    {
        SceneManager.LoadScene(goodEndingScene);
        return;
    }

    // Bad si on est au-dessous ou égal au seuil "mauvais"
    if (gauge <= gameData.badEndingMax)
    {
        SceneManager.LoadScene(badEndingScene);
        return;
    }

    // Sinon, c'est forcément Neutral
    SceneManager.LoadScene(neutralEndingScene);
}

}
