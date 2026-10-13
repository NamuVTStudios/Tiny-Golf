using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    // Regla UNAHUR: 2 puntos al primero en embocar, 1 a los demás (se reinicia en cada nivel/escena)
    private bool hasAnyPlayerScoredInCurrentHole = false;

    public void InitializeScoreboard(int playerCount)
    {
        hasAnyPlayerScoredInCurrentHole = false;
        for (int i = 0; i < playerCount; i++)
        {
            if (!MatchSession.Scores.ContainsKey(i)) MatchSession.Scores[i] = 0;
        }
    }

	public void StartNewHole()
	{
		hasAnyPlayerScoredInCurrentHole = false;
	}

	public void RegisterHoleScore(int playerId)
    {
        if (!MatchSession.Scores.ContainsKey(playerId)) return;

        int points = hasAnyPlayerScoredInCurrentHole ? 1 : 2;
        hasAnyPlayerScoredInCurrentHole = true;
        MatchSession.Scores[playerId] += points;
        Debug.Log($"ScoreController: Player {playerId + 1} anotó. +{points} Punto(s).");
    }

    public int GetCurrentHoleNumber()
    {
        return Mathf.Min(MatchSession.CurrentLevel + 1, MatchSession.LevelScenes.Length);
    }

    public int GetPlayerScore(int playerId)
    {
        return MatchSession.Scores.ContainsKey(playerId) ? MatchSession.Scores[playerId] : 0;
    }

    public Dictionary<int, int> GetRawScores()
    {
        return MatchSession.Scores;
    }
}