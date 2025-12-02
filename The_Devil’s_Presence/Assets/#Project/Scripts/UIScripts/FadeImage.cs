using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeImage : MonoBehaviour
{
    [Header("Référence UI")]
    // CanvasGroup qui permet le fade propre sur tout un élément UI
    public CanvasGroup canvasGroup;

    // Durée du fade en secondes, apparision en 1s ou 2 etc
    [Header("Paramètres du fade")]
    public float fadeDuration = 1f;

    // Fade générique pour pouvoir utiliser pour d'autre transitions avec d'autre images etc

    private void Reset()
    {
        // Sécurité pour éviter les bug: si on ajoute le script et qu'il n'y a pas de CanvasGroup,
        // ici, Unity va en ajoute un automatiquement.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // méthode pour faire apparaître l'image (alpha 0 → 1)
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f));
    }

    // methode  pour faire disparaître (alpha 1 → 0)
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f));
    }

    // Coroutine centrale du fade
    IEnumerator FadeRoutine(float targetAlpha)
    {
        // Juste une micro-sécurité au cas où Unity bug et perd la référence
        if (canvasGroup == null)
            yield break;

        // Je prends l'alpha actuel
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Interpolation entre alpha actuel et alpha souhaité
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null; // On attend la frame suivante
        }

        // forçage de l'alpha final (petite sécurité)
        canvasGroup.alpha = targetAlpha;
    }
}
