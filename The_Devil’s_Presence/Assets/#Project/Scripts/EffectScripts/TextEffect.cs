using UnityEngine;
using TMPro;
using System.Collections;

public class TextEffect : MonoBehaviour
{
    public System.Action OnFinished; // Appelée quand le texte a fini de s'écrire

    [SerializeField] private TextMeshProUGUI targetText; // le texte à écrire lettre par lettre
    [SerializeField] private float speed = 0.03f;        // vitesse d'écriture (à régler selon ton goût)

    private Coroutine writingRoutine; // pour arrêter proprement si on change de phrase en plein milieu
    private bool skip = false;        // si le joueur clique pour afficher tout d'un coup

    private void Awake()
    {
        // Si j’ai la flemme de brancher le champ dans l’inspector,
        // le script va chercher un TextMeshProUGUI sur le même objet.
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
    }

    // Appelé par un autre script pour lancer l'effet
    public void Run(string text)
    {
        // si déjà en train d'écrire > on coupe
        if (writingRoutine != null)
            StopCoroutine(writingRoutine);

        skip = false; // reset du skip
        writingRoutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string fullText)
    {
        if (targetText == null) yield break;

        targetText.text = ""; // on vide le texte avant d'écrire

        foreach (char c in fullText)
        {
            if (skip)
            {
                targetText.text = fullText;
                break;
            }

            targetText.text += c;
            yield return new WaitForSeconds(speed);
        }

        writingRoutine = null;

        // Signale que l’écriture est terminée
        OnFinished?.Invoke();
    }

    // Méthode publique si tu veux permettre au joueur de cliquer pour afficher directement tout le texte
    public void Skip()
    {
        skip = true;
    }
}
