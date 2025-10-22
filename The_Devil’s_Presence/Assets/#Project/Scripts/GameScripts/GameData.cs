using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    [Header("Liste ordonnée des questions/texte (Q1 → Q2 → Q3...)")]
    public List<Questions> questions = new();

    [Header("Paramètres de fin")]
    public int badEndingMax = -1;      // jauge <= badEndingMax => mauvaise fin
    public int neutralEndingMax = 3;   // jauge <= neutralEndingMax && > badEndingMax => fin neutre
    public int goodEndingMin = 4;      // jauge >= goodEndingMin => bonne fin

    // Calcule les bornes min/max possibles de la jauge
    public void ComputeGaugeBounds(out int minTotal, out int maxTotal)
    {
        minTotal = 0;
        maxTotal = 0;

        // On parcourt toutes les questions du jeu

        foreach (Questions q in questions)
        {
            // Vérifie que la question est bien remplie

            if (q == null || q.reponses == null || q.reponses.Count == 0)
                continue;

            int qMin = int.MaxValue;
            int qMax = int.MinValue;

            // Trouve les impacts minimum et maximum parmi les réponses

            foreach (Reponses r in q.reponses)
            {
                if (r == null)
                    continue;

                if (r.impact < qMin) qMin = r.impact;
                if (r.impact > qMax) qMax = r.impact;
            }

            // Si les valeurs sont valides, on les ajoute au total

            if (qMin != int.MaxValue && qMax != int.MinValue)
            {
                minTotal += qMin;
                maxTotal += qMax;
            }
        }
    }


    // Permet de voir les bornes dans la console (ici pour vérifier si tout est ok, bout de code provisoire)
    void PrintGaugeBounds()
    {
        int minTotal, maxTotal;
        ComputeGaugeBounds(out minTotal, out maxTotal);

        Debug.Log($"[GameData] Jauge possible : min={minTotal}, max={maxTotal}... ");
    }


    // Corrige automatiquement les valeurs si l’ordre est incohérent
    void OnValidate()
    {
        if (neutralEndingMax < badEndingMax)
            neutralEndingMax = badEndingMax;

        if (goodEndingMin <= neutralEndingMax)
            goodEndingMin = neutralEndingMax + 1;
    }
}

