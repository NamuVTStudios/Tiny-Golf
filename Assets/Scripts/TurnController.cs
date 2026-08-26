using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private List<PlayerData> playersList = new List<PlayerData>();
    private int currentPlayerIndex = -1;
    private float turnTimeLimit;
    private Coroutine turnTimerCoroutine;

    public PlayerData CurrentPlayer
    {
        get
        {
            if (playersList == null || playersList.Count == 0 || currentPlayerIndex < 0) return null;
            return playersList[currentPlayerIndex];
        }
    }

    public void InitializeTurns(int playerCount, float timeLimit, Vector2 startingPosition)
{
    turnTimeLimit = timeLimit;
    playersList.Clear(); // Limpiamos cualquier rastro de partidas anteriores

    // CORRECCIÓN DINÁMICA: Solo creamos la cantidad de datos que el usuario eligió en la UI (1, 2 o 4)
    for (int i = 0; i < playerCount; i++)
    {
        PlayerData newPlayer = new PlayerData(i, $"Player {i + 1}");
        newPlayer.LastSavedPosition = startingPosition; 
        playersList.Add(newPlayer);
    }
    
    currentPlayerIndex = -1; // Reset seguro de índice para arrancar desde el primero
    Debug.Log($"TurnController: Inicializados exitosamente {playersList.Count} jugadores para la partida.");
}

    public void PassToNextPlayer()
    {
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }

        // Avanzar el índice de uno en uno de forma cíclica segura
        if (playersList.Count > 0)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % playersList.Count;
        }

        turnTimerCoroutine = StartCoroutine(CountTurnTime());
    }

    public bool RegisterStroke(int maxStrokesLimit)
    {
        if (CurrentPlayer != null)
        {
            CurrentPlayer.CurrentHoleStrokes++;
            Debug.Log($"{CurrentPlayer.Name} lleva {CurrentPlayer.CurrentHoleStrokes} / {maxStrokesLimit} golpes.");

            // Si el jugador alcanzó o superó el límite de golpes del hoyo
            if (CurrentPlayer.CurrentHoleStrokes >= maxStrokesLimit)
            {
                Debug.LogWarning($"TurnController: {CurrentPlayer.Name} ha alcanzado el límite máximo de golpes.");
                return true; // Retorna true indicando que ya gastó su última oportunidad
            }
        }
        return false; // Retorna false si todavía le quedan tiros disponibles
    }

    private IEnumerator CountTurnTime()
    {
        float timeRemaining = turnTimeLimit;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        // Al expirar el tiempo, llamamos de forma segura al GameManager centralizado
        GameManager.Instance.AdvanceToNextTurn();
    }
}