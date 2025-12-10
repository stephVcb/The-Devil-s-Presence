using UnityEngine;
using TMPro;

public class AutoTextEffect : MonoBehaviour
{
    [SerializeField] private TextEffect effect;
    [TextArea] public string overrideText;

    private void Awake()
    {
        // Si je n'ai rien mis dans l’inspector, il va chercher automatiquement
        if (effect == null)
            effect = GetComponent<TextEffect>();
    }

    private void Start()
    {
        if (effect == null) return;

        string textToUse = overrideText;

        // Si override est vide → on utilise le texte déjà écrit dans le TMP
        if (string.IsNullOrWhiteSpace(textToUse))
        {
            var tmp = GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                textToUse = tmp.text;
        }

        // Si on a un texte à afficher → on lance l'effet
        if (!string.IsNullOrWhiteSpace(textToUse))
            effect.Run(textToUse);
    }
}
