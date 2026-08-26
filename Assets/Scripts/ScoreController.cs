using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    // Diccionario para guardar el puntaje acumulado por cada ID de jugador
    private Dictionary<int, int> playerTotalScores = new Dictionary<int, int>();
    private int totalHolesInMatch;
    private int currentHoleNumber = 1;
    
    // Bandera para la regla oficial de la UNAHUR (2 puntos al primero, 1 al resto)
    private bool hasAnyPlayerScoredInCurrentHole = false;

    public void InitializeScoreboard(int playerCount, int totalHoles)
    {
        totalHolesInMatch = totalHoles;
        currentHoleNumber = 1;
        hasAnyPlayerScoredInCurrentHole = false;
        playerTotalScores.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            playerTotalScores.Add(i, 0); // Registramos a cada jugador de forma única
        }
    }

    // Recibe el ID exacto del jugador para que no haya confusiones posicionales
    public void RegisterHoleScore(int playerId)
    {
        if (!playerTotalScores.ContainsKey(playerId)) return;

        int pointsToAssign = 0;

        if (!hasAnyPlayerScoredInCurrentHole)
        {
            pointsToAssign = 2; // Premio mayor para el primero
            hasAnyPlayerScoredInCurrentHole = true; 
            Debug.Log($"ScoreController: Player {playerId + 1} anotó PRIMERO. +2 Puntos.");
        }
        else
        {
            pointsToAssign = 1; // Puntos para los siguientes
            Debug.Log($"ScoreController: Player {playerId + 1} anotó. +1 Punto.");
        }

        playerTotalScores[playerId] += pointsToAssign;
    }

    public void AdvanceToNextHole()
    {
        currentHoleNumber++;
        hasAnyPlayerScoredInCurrentHole = false; // Se resetea el premio de 2 puntos para el nuevo hoyo
    }

    public bool IsMatchFinished()
    {
        return currentHoleNumber > totalHolesInMatch;
    }

    public int GetCurrentHoleNumber()
    {
        return currentHoleNumber;
    }

    public int GetPlayerScore(int playerId)
    {
        return playerTotalScores.ContainsKey(playerId) ? playerTotalScores[playerId] : 0;
    }

    public Dictionary<int, int> GetRawScores()
    {
        return playerTotalScores;
    }
}