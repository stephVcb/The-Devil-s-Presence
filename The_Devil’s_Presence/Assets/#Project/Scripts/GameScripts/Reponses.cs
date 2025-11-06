using UnityEngine;

//construire le squelette des reponses, le modèle

[System.Serializable]
public class Reponses
{
    public string text;   // text affiché sur le bouton
    public int impact;    // la valeur qui influence la jauge (+ ou -)
    public int nextQuestion = -1; //pour passer à la question suivante
}
