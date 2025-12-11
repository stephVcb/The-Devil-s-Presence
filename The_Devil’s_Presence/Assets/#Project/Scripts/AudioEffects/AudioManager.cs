using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Instance globale accessible partout
    public static AudioManager Instance;

    // La source principale pour les ambiances
    public AudioSource ambienceSource;

    // Les sons à utiliser selon la pièce
    public AudioClip salonClip;
    public AudioClip cuisineClip;
    public AudioClip chambreClip;

    void Awake()
    {
        // Assurer qu'il n'y ait qu'un seul AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // garder entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fonction pour changer de question
    public void PlayAmbience(string background)
    {
        AudioClip nextClip = null;

        // Choisir le bon son selon le background de la Q
        if (background == "Salon") nextClip = salonClip;
        if (background == "Cuisine") nextClip = cuisineClip;
        if (background == "Chambre") nextClip = chambreClip;

        if (nextClip == null)
        {
            Debug.LogWarning("Pas de son trouvé pour : " + background);
            return;
        }

        // changement d'ambiance (fade)
        StartCoroutine(SwitchAmbience(nextClip));
    }

    // Fade-out puis change de musique puis le fade-in
    IEnumerator SwitchAmbience(AudioClip newClip)
    {
        // Le Fade-out
        float startVol = ambienceSource.volume;
        float t = 0;

        while (t < 1f) // 1 seconde de fade-out
        {
            t += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(startVol, 0f, t / 1f);
            yield return null;
        }

        ambienceSource.Stop();

        // Charger la nouvelle musique
        ambienceSource.clip = newClip;
        ambienceSource.Play();

        // Le Fade-in 
        t = 0;
        while (t < 1f) // 1 seconde de fade-in
        {
            t += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(0f, startVol, t / 1f);
            yield return null;
        }
    }
}
