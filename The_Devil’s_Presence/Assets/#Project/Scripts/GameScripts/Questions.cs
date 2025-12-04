using UnityEngine;
using System.Collections.Generic;

// Dans quelle "pièce" se passe la question
public enum BackgroundType
{
    Salon,
    Chambre,
    Cuisine
}

// Script qui construit le squelette des question

[System.Serializable]
public class Questions
{
    public string id;
    public string prompt;
    public List<Reponses> reponses;

    // Pour savoir quel décor afficher pour cette question
    public BackgroundType background;
}


