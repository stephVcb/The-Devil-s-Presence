using UnityEngine;

public enum EndingType
{
    Bad,
    Neutral,
    Good
}

// Petit truc global crado mais pratique : 
// on stocke juste la dernière fin atteinte.
public static class GameResult
{
    public static EndingType lastEnding = EndingType.Neutral;
}

