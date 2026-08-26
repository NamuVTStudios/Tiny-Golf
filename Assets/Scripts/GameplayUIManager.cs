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

    [Header("UI Character References")]
    [SerializeField] private Image activePlayerAvatarImage; 

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
    public void UpdateHUD(string playerName, int currentStrokes, int currentHole, int totalHoles, Sprite playerAvatar)
    {
        if (currentPlayerText != null) 
            currentPlayerText.text = $"Turn: {playerName}";

        if (strokesText != null) 
            strokesText.text = $"Strokes: {currentStrokes}";

        if (holeNumberText != null) 
            holeNumberText.text = $"Hole: {currentHole} / {totalHoles}";

        if (activePlayerAvatarImage != null && playerAvatar != null)
        {
            activePlayerAvatarImage.sprite = playerAvatar;
        }
    }
}