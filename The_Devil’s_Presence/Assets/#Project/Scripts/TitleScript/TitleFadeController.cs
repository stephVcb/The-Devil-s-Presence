using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleFadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private string nextScene = "SceneIntro";

    [SerializeField] private AudioSource titleAudio;


    private void Start()
    {
            if (titleAudio != null)
                titleAudio.Play();

            StartCoroutine(PlayTitle());
    }

    IEnumerator PlayTitle()
    {
        // fade in, le titre apparait 
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        // pause
        yield return new WaitForSeconds(holdDuration);

        if (titleAudio != null)
            titleAudio.Stop();
            
        SceneManager.LoadScene(nextScene);
    }
}
