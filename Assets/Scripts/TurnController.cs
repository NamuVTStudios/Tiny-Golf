using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private List<PlayerData> playersList = new List<PlayerData>();
    private int currentPlayerIndex = -1;
    private float turnTimeLimit;
    private Coroutine turnTimerCoroutine;
    public event System.Action<float> OnTimeChanged;
    public IReadOnlyList<PlayerData> Players => playersList;

    public PlayerData CurrentPlayer
    {
        get
        {
            if (playersList == null || playersList.Count == 0 || currentPlayerIndex < 0) return null;
            return playersList[currentPlayerIndex];
        }
    }

    public void InitializeTurns(int playerCount, float timeLimit, Vector2 startingPosition, string[] names)
    {
        turnTimeLimit = timeLimit;
        playersList.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            string playerName = (names != null && i < names.Length) ? names[i] : $"Player {i + 1}";
            PlayerData newPlayer = new PlayerData(i, playerName);
            newPlayer.LastSavedPosition = startingPosition;
            playersList.Add(newPlayer);
        }

        currentPlayerIndex = -1;
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
        if (turnTimeLimit <= 0) yield break; // 0 = sin límite
        float t = turnTimeLimit;
        while (t > 0)
        {
            t -= Time.deltaTime;
            OnTimeChanged?.Invoke(Mathf.Max(t, 0f));
            yield return null;
        }
        GameManager.Instance.OnTurnTimeExpired();
    }
}