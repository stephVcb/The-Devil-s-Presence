using System.Collections;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
    // appel/choix de l'Audiosource
    public AudioSource audioSource;

    // Appel de la fonction pour augmenter en x seconde
    public void FadeIn(float duration, float targetVolume)
    {
        StartCoroutine(FadeInCoroutine(duration, targetVolume));
    }

    // Appel de la fonction pour diminuer en x seconde
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    // augmenter le volume progressivement
    IEnumerator FadeInCoroutine(float duration, float targetVolume)
    {
        // On part de 0 (silence total)
        float startVolume = 0f;
        audioSource.volume = 0f;

        // On lance réellement le son
        audioSource.Play();

        float t = 0;

        // Boucle qui augmente le volume petit à petit
        while (t < duration)
        {
            t += Time.deltaTime;

            // le volume grimpe doucement jusqu'au volume final
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);

            yield return null;
        }
    }

    // baisse le volume jusqu'au silence
    IEnumerator FadeOutCoroutine(float duration)
    {
        // mémo le volume actuel avant de le faire descendre
        float startVolume = audioSource.volume;
        float t = 0;

        // Boucle qui réduit le volume petit à petit
        while (t < duration)
        {
            t += Time.deltaTime;

            // volume descend vers 0
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);

            yield return null;
        }

        // arrêt total du son
        audioSource.Stop();
    }
}
