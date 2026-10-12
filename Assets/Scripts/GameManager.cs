using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isLevelEnding = false;
    public bool CanPlayerShoot => isMatchActive && !isProcessingHole && !isLevelEnding;
    private float matchTimeRemaining;   // en StartGlobalMatch: = gameSettings.matchTimeLimit;
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

    public void OnTurnTimeExpired()
    {
        if (!isMatchActive || isProcessingHole || isLevelEnding) return;
        // Si la bola rueda, HandleBallStopped ya pasa el turno cuando frene
        if (gameplayBall.IsStopped()) AdvanceToNextTurn();
    }

    private void Update()
    {
        if (!isMatchActive) return;
        matchTimeRemaining -= Time.deltaTime;
        GameplayUIManager.Instance.UpdateMatchTime(matchTimeRemaining);
        if (matchTimeRemaining <= 0f) FinishMatch();
    }

    private void StartGlobalMatch()
    {
        isMatchActive = true;
        isProcessingHole = false;
        isLevelEnding = false;
        activePlayerMaxStrokesReached = false;
        playersWhoFinishedCurrentHole.Clear();

        // Por si abrís un nivel directo desde el editor sin pasar por el menú
        if (MatchSession.Scores.Count != gameSettings.playerCount)
            MatchSession.Reset(gameSettings.playerCount, gameSettings.matchTimeLimit);
        matchTimeRemaining = MatchSession.MatchTimeRemaining;

        scoreController.InitializeScoreboard(gameSettings.playerCount);
        turnController.InitializeTurns(gameSettings.playerCount, gameSettings.turnTimeLimit, spawnPoint.position, gameSettings.playerNames);
        turnController.OnTimeChanged += GameplayUIManager.Instance.UpdateTurnTime;

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

    public void RegisterHoleCompletion(Vector3 holePos)
    {
        if (isProcessingHole || !isMatchActive || isLevelEnding) return;
        StartCoroutine(ProcessHoleSequence(holePos));
    }

    private IEnumerator ProcessHoleSequence(Vector3 holePos)
    {
        isProcessingHole = true;
        shotController.enabled = false;
        gameplayBall.OnBallStopped -= HandleBallStopped;

        activePlayerMaxStrokesReached = false;
        gameplayBall.PlayHoleInSound();

        PlayerData activePlayer = turnController.CurrentPlayer;
        scoreController.RegisterHoleScore(activePlayer.Id);
        playersWhoFinishedCurrentHole.Add(activePlayer.Id);
        UpdateGameplayUI(); // el puntaje se actualiza al instante

        yield return StartCoroutine(gameplayBall.HoleInAnimation(holePos));
        yield return new WaitForSeconds(0.3f);

        ResetBallToSavedPosition(spawnPoint.position);

        gameplayBall.OnBallStopped += HandleBallStopped;
        isProcessingHole = false;

        AdvanceToNextTurn();
    }

    private void EvaluateHoleProgression()
    {
        if (isLevelEnding) return;
        isLevelEnding = true;
        shotController.enabled = false;
        StartCoroutine(EndLevelRoutine());
    }

    private IEnumerator EndLevelRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        playersWhoFinishedCurrentHole.Clear();
        MatchSession.MatchTimeRemaining = matchTimeRemaining;
        MatchSession.CurrentLevel++;

        if (MatchSession.CurrentLevel >= MatchSession.LevelScenes.Length)
            FinishMatch();
        else
            SceneManager.LoadScene(MatchSession.LevelScenes[MatchSession.CurrentLevel]);
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

        GameplayUIManager.Instance.UpdateHUD(
            turnController.CurrentPlayer != null ? turnController.CurrentPlayer.Name : "Player",
            turnController.CurrentPlayer != null ? turnController.CurrentPlayer.CurrentHoleStrokes : 0,
            gameSettings.maxStrokesPerHole,
            scoreController.GetCurrentHoleNumber(),
            gameSettings.totalHoles,
            activeAvatarSprite
        );

        GameplayUIManager.Instance.UpdateScoreboard(BuildScoreboard());
    }

    private string BuildScoreboard()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var p in turnController.Players)
        {
            string mark = p == turnController.CurrentPlayer ? "> " : "  ";
            sb.AppendLine($"{mark}{p.Name}: {scoreController.GetPlayerScore(p.Id)} pts");
        }
        return sb.ToString();
    }

    private void FinishMatch()
    {
        isMatchActive = false;
        shotController.enabled = false;
        gameplayBall.SetBallState(false);

        MatchSummaryController summaryCtrl = FindAnyObjectByType<MatchSummaryController>();
        if (summaryCtrl != null)
        {
            summaryCtrl.DisplayMatchResults(scoreController.GetRawScores(), gameSettings.playerCount, gameSettings.playerNames);
        }
    }

    public GameSettings GetGameSettings() => gameSettings;
}