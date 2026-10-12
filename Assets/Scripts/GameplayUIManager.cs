using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance { get; private set; }

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private TextMeshProUGUI strokesText;
    [SerializeField] private TextMeshProUGUI holeNumberText;
    [SerializeField] private TextMeshProUGUI scoreboardText;

    [Header("UI Character References")]
    [SerializeField] private Image activePlayerAvatarImage;

    [Header("Timers")]
    [SerializeField] private TextMeshProUGUI matchTimeText;
    [SerializeField] private TextMeshProUGUI turnTimeText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Método utilitario centralizado para refrescar de un solo golpe toda la pantalla
    public void UpdateHUD(string playerName, int currentStrokes, int maxStrokes, int currentHole, int totalHoles, Sprite playerAvatar)
    {
        if (currentPlayerText != null)
            currentPlayerText.text = $"Turn: {playerName}";

        if (strokesText != null)
            strokesText.text = $"Strokes: {currentStrokes} / {maxStrokes}";

        if (holeNumberText != null)
            holeNumberText.text = $"Hole: {currentHole} / {totalHoles}";

        if (activePlayerAvatarImage != null && playerAvatar != null)
        {
            activePlayerAvatarImage.sprite = playerAvatar;
        }
    }

    public void UpdateMatchTime(float s) { if (matchTimeText != null) matchTimeText.text = $"Match: {Mathf.CeilToInt(Mathf.Max(s, 0)) / 60}:{Mathf.CeilToInt(Mathf.Max(s, 0)) % 60:00}"; }
    public void UpdateTurnTime(float s) { if (turnTimeText != null) turnTimeText.text = $"Time: {Mathf.CeilToInt(s)}s"; }
    public void UpdateScoreboard(string text) { if (scoreboardText != null) scoreboardText.text = text; }
}