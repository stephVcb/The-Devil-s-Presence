using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Instance globale accessible partout
    public static AudioManager Instance;

    // La source principale pour les ambiances
    public AudioSource ambienceSource;

    // Les sons à utiliser selon la pièce/scene
    public AudioClip introClip;
    public AudioClip salonClip;
    public AudioClip cuisineClip;
    public AudioClip chambreClip;

    // Musiques pour les fins (good / neutral / bad)
    public AudioClip endBadClip;
    public AudioClip endNeutralClip;
    public AudioClip endGoodClip;


    // pour les petits sons
    [SerializeField] private AudioSource sfxSource; 
    // pour faire les fade-in / fade-out
    [SerializeField] private AudioFader audioFade;


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

        // choisir la bonne ambiance selon la pièce
        switch (background)
        {
            case "Salon":
                nextClip = salonClip;
                break;

            case "Chambre":
                nextClip = chambreClip;
                break;

            case "Cuisine":
                nextClip = cuisineClip;
                break;
            case "Intro":
                nextClip = introClip;
                break;
        }

        if (nextClip == null || ambienceSource == null)
            return;

        // si c'est déjà cette musique, rien à faire
        if (ambienceSource.clip == nextClip)
            return;

        // fade-out -> changement -> fade-in
        StartCoroutine(SwitchAmbience(nextClip));
    }
    
    //jouer musique de fin
    public void PlayEndingMusic(string endingType)
    {
        AudioClip nextClip = null;

        switch (endingType)
        {
            case "Bad":
                nextClip = endBadClip;
                break;

            case "Neutral":
                nextClip = endNeutralClip;
                break;

            case "Good":
                nextClip = endGoodClip;
                break;
        }

        if (nextClip == null || ambienceSource == null)
            return;

        if (ambienceSource.clip == nextClip)
            return;

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
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        // on change le son du SFX
        sfxSource.clip = clip;

        // on force le volume à 0 pour faire la montée en douceur
        sfxSource.volume = 0f;

        // on lance le fade-in sur 0.2 sec (petit effet smooth)
        if (audioFade != null)
            audioFade.FadeIn(0.2f, 1f);  // // augmenter le son en 0.2 sec jusqu'à volume 1
        else
            sfxSource.Play();            // // si pas de fade -> on joue brut
    }


}
