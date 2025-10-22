using System.Collections;
using TMPro;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private TMP_Text quitText;
    [SerializeField] private float delay = 3f;

    public void Quit()
    {
        StartCoroutine(QuitRoutine());
    }
    private IEnumerator QuitRoutine()
    {
        quitText.gameObject.SetActive(true);
        quitText.SetText("Fermeture du jeu...");
        Debug.Log("Quit() cliqué — coroutine lancée");
        yield return new WaitForSecondsRealtime(delay);

        Debug.Log("Fin de QuitRoutine — Application.Quit() appelée");
        
        Application.Quit();
    }
}
