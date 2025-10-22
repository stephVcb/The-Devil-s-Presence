using UnityEngine;
using System.Collections.Generic;

// Script qui construit le squelette des question

[System.Serializable]
public class Questions
{
    public string id;
    public string prompt;
    public List<Reponses> reponses;
}



