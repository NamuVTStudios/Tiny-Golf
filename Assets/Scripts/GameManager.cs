using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameSettings gameSettings;

    [Header("Sub-Systems")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private ShotController shotController;

    [Header("Gameplay References")]
    [SerializeField] private Ball gameplayBall;
    [SerializeField] private Transform spawnPoint;

    private bool isProcessingHole = false;
    private bool isBallOutOfBounds = false;
    private bool isMatchActive = false;
    private bool activePlayerMaxStrokesReached = false;
    private ScoreController scoreController;

    private HashSet<int> playersWhoFinishedCurrentHole = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        scoreController = GetComponent<ScoreController>();
    }

    private void Start()
    {
        StartGlobalMatch();
    }

    private void StartGlobalMatch()
    {
        isMatchActive = true;
        isProcessingHole = false;
        activePlayerMaxStrokesReached = false;
        playersWhoFinishedCurrentHole.Clear();

        scoreController.InitializeScoreboard(gameSettings.playerCount, gameSettings.totalHoles);
        turnController.InitializeTurns(gameSettings.playerCount, gameSettings.turnTimeLimit, spawnPoint.position);

        gameplayBall.OnBallStopped -= HandleBallStopped;
        gameplayBall.OnBallStopped += HandleBallStopped;

        AdvanceToNextTurn();
    }

    public void AdvanceToNextTurn()
    {
        if (!isMatchActive) return;

        shotController.enabled = false;

        // 1. Guardar la posición del jugador que acaba de tirar (si no terminó el hoyo)
        if (turnController.CurrentPlayer != null &&
            !playersWhoFinishedCurrentHole.Contains(turnController.CurrentPlayer.Id) &&
            !isBallOutOfBounds)
        {
            turnController.CurrentPlayer.LastSavedPosition = gameplayBall.transform.position;
        }

        if (isBallOutOfBounds)
        {
            if (turnController.CurrentPlayer != null)
            {
                turnController.CurrentPlayer.LastSavedPosition = spawnPoint.position;
            }
            isBallOutOfBounds = false;
        }

        // 2. Buscar secuencialmente al siguiente jugador habilitado para este hoyo
        bool nextPlayerFound = false;
        for (int i = 0; i < gameSettings.playerCount; i++)
        {
            turnController.PassToNextPlayer();

            if (!playersWhoFinishedCurrentHole.Contains(turnController.CurrentPlayer.Id))
            {
                nextPlayerFound = true;
                break;
            }
        }

        // 3. Evaluar si la ronda terminó porque todos completaron o agotaron golpes
        if (!nextPlayerFound || playersWhoFinishedCurrentHole.Count >= gameSettings.playerCount)
        {
            EvaluateHoleProgression();
            return;
        }

        // 4. Mover la bola física a la marca del jugador de turno
        ResetBallToSavedPosition(turnController.CurrentPlayer.LastSavedPosition);

        gameplayBall.SetBallState(true);

        // --- COMPROBÁ QUE ESTAS DOS LÍNEAS ESTÉN AQUÍ DE FORMA CONSECUTIVA ---
        ApplyPlayerVisuals();  // Cambia el Sprite de la pelota en la cancha
        UpdateGameplayUI();    // Cambia el Nombre, los Golpes y el Retrato del personaje en el Canvas HUD

        shotController.enabled = true;
    }

    private void HandleBallStopped()
    {
        if (isProcessingHole || !isMatchActive) return;

        // --- INTERCEPCIÓN REGLAMENTARIA DE GOLPES MÁXIMOS POR HOYO (UNAHUR) ---
        if (activePlayerMaxStrokesReached)
        {
            activePlayerMaxStrokesReached = false;

            Debug.LogWarning($"GameManager: {turnController.CurrentPlayer.Name} se quedó sin tiros. Fin de su turno en este hoyo.");

            // Lo anotamos en la lista de los que terminaron este hoyo
            playersWhoFinishedCurrentHole.Add(turnController.CurrentPlayer.Id);

            gameplayBall.SetBallState(false);
            ResetBallToSavedPosition(spawnPoint.position);

            // Si con este jugador ya terminaron absolutamente todos, avanzamos de hoyo lógico
            if (playersWhoFinishedCurrentHole.Count >= gameSettings.playerCount)
            {
                EvaluateHoleProgression();
            }
            else
            {
                AdvanceToNextTurn();
            }
            return;
        }

        // Caso normal: Si le quedan tiros, el turno pasa y la bola retiene su posición
        AdvanceToNextTurn();
    }

    public void NotifyPlayerMaxStrokesReached()
    {
        activePlayerMaxStrokesReached = true;
    }

    public void NotifyBallOutOfBounds()
    {
        isBallOutOfBounds = true;
    }

    public void RegisterHoleCompletion()
    {
        if (isProcessingHole || !isMatchActive) return;
        StartCoroutine(ProcessHoleSequence());
    }

    private IEnumerator ProcessHoleSequence()
    {
        isProcessingHole = true;
        shotController.enabled = false;
        gameplayBall.OnBallStopped -= HandleBallStopped;

        activePlayerMaxStrokesReached = false;
        gameplayBall.PlayHoleInSound();

        PlayerData activePlayer = turnController.CurrentPlayer;
        scoreController.RegisterHoleScore(activePlayer.Id);

        playersWhoFinishedCurrentHole.Add(activePlayer.Id);
        gameplayBall.SetBallState(false);

        yield return new WaitForSeconds(0.6f);

        ResetBallToSavedPosition(spawnPoint.position);

        gameplayBall.OnBallStopped += HandleBallStopped;
        isProcessingHole = false;

        AdvanceToNextTurn();
    }

    private void EvaluateHoleProgression()
    {
        scoreController.AdvanceToNextHole();
        playersWhoFinishedCurrentHole.Clear();

        if (scoreController.IsMatchFinished() || scoreController.GetCurrentHoleNumber() > gameSettings.totalHoles)
        {
            FinishMatch();
        }
        else
        {
            // CORRECCIÓN MAGISTRAL: Reemplazamos el "4" fijo por el parámetro real del GameSettings
            for (int i = 0; i < gameSettings.playerCount; i++)
            {
                turnController.PassToNextPlayer();
                turnController.CurrentPlayer.LastSavedPosition = spawnPoint.position;
                turnController.CurrentPlayer.CurrentHoleStrokes = 0; // Reset de golpes para el nuevo hoyo
            }

            gameplayBall.OnBallStopped -= HandleBallStopped;
            gameplayBall.OnBallStopped += HandleBallStopped;

            AdvanceToNextTurn();
        }
    }

    private void ResetBallToSavedPosition(Vector2 positionToMove)
    {
        if (gameplayBall == null) return;
        gameplayBall.ClearTrail();

        Rigidbody2D ballRb = gameplayBall.GetComponent<Rigidbody2D>();
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        gameplayBall.transform.position = positionToMove;
    }

    private void ApplyPlayerVisuals()
    {
        // 1. Buscamos el componente encargado de renderizar la imagen de la bola física en la cancha
        SpriteRenderer ballRenderer = gameplayBall.GetComponent<SpriteRenderer>();

        if (ballRenderer != null && gameSettings.availableBallSprites != null && turnController.CurrentPlayer != null)
        {
            int activePlayerId = turnController.CurrentPlayer.Id;

            // Comprobamos que el ID del jugador exista dentro del arreglo de elecciones del menú
            if (gameSettings.selectedPlayerSpriteIndices != null && activePlayerId < gameSettings.selectedPlayerSpriteIndices.Length)
            {
                // Conseguimos el índice estético que este jugador específico eligió
                int spriteIndex = gameSettings.selectedPlayerSpriteIndices[activePlayerId];

                // Inyectamos de forma dinámica el diseño de pelota correspondiente de tu Spritesheet
                if (spriteIndex >= 0 && spriteIndex < gameSettings.availableBallSprites.Length)
                {
                    ballRenderer.sprite = gameSettings.availableBallSprites[spriteIndex];
                    Debug.Log($"[Visual Bola] Aplicado diseño para {turnController.CurrentPlayer.Name}. Índice: {spriteIndex}");
                }
            }
        }
    }

    public void UpdateGameplayUI()
    {
        if (!isMatchActive) return;

        Sprite activeAvatarSprite = null;

        if (turnController != null && turnController.CurrentPlayer != null)
        {
            int activePlayerId = turnController.CurrentPlayer.Id;

            if (gameSettings.selectedPlayerSpriteIndices != null && activePlayerId < gameSettings.selectedPlayerSpriteIndices.Length)
            {
                // Buscamos el índice del personaje que eligió este jugador en el menú principal
                int avatarIndex = gameSettings.selectedPlayerSpriteIndices[activePlayerId];

                // Extraemos la foto de la biblioteca de avatares de personajes
                if (gameSettings.availablePlayerSprites != null && avatarIndex >= 0 && avatarIndex < gameSettings.availablePlayerSprites.Length)
                {
                    activeAvatarSprite = gameSettings.availablePlayerSprites[avatarIndex];
                }
            }
        }

        // Refrescamos toda la interfaz HUD enviando el avatar/personaje del jugador actual
        GameplayUIManager.Instance.UpdateHUD(
            turnController.CurrentPlayer != null ? turnController.CurrentPlayer.Name : "Player",
            turnController.CurrentPlayer != null ? turnController.CurrentPlayer.CurrentHoleStrokes : 0,
            scoreController.GetCurrentHoleNumber(),
            gameSettings.totalHoles,
            activeAvatarSprite // <--- INYECCIÓN DEL PERSONAJE COMPLETA EN LA PANTALLA DE GAMEPLAY
        );
    }

    private void FinishMatch()
    {
        isMatchActive = false;
        shotController.enabled = false;
        gameplayBall.SetBallState(false);

        MatchSummaryController summaryCtrl = FindAnyObjectByType<MatchSummaryController>();
        if (summaryCtrl != null)
        {
            summaryCtrl.DisplayMatchResults(scoreController.GetRawScores(), gameSettings.playerCount);
        }
    }

    public GameSettings GetGameSettings() => gameSettings;
}