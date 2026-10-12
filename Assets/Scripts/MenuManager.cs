using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Data Container")]
    [SerializeField] private GameSettings gameSettings;

    [Header("UI General Components")]
    [SerializeField] private TMP_Dropdown playerDropdown;
    [SerializeField] private TMP_InputField holesInputField;
    [SerializeField] private TMP_InputField strokesInputField;
    [SerializeField] private TMP_InputField[] nameInputs; // 4, uno por contenedor
    [SerializeField] private Slider timeSlider;

    [Header("Dynamic Player Containers")]
    [SerializeField] private GameObject containerP1;
    [SerializeField] private GameObject containerP2;
    [SerializeField] private GameObject containerP3;
    [SerializeField] private GameObject containerP4;

    [Header("Sprites Dropdowns References")]
    [SerializeField] private TMP_Dropdown spriteDropdownP1;
    [SerializeField] private TMP_Dropdown spriteDropdownP2;
    [SerializeField] private TMP_Dropdown spriteDropdownP3;
    [SerializeField] private TMP_Dropdown spriteDropdownP4;

    private void Start()
    {
        // Vinculamos el detector de cambios en el desplegable de jugadores
        if (playerDropdown != null)
        {
            playerDropdown.onValueChanged.RemoveAllListeners();
            playerDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
        }

        // Forzamos la calibración inicial de los selectores según la opción por defecto
        OnPlayerCountChanged();
    }

    public void OnPlayerCountChanged()
    {
        if (containerP1 == null || containerP2 == null || containerP3 == null || containerP4 == null || playerDropdown == null)
            return;

        int selectedIndex = playerDropdown.value;

        // El Jugador 1 siempre se muestra
        containerP1.SetActive(true);

        // Apagamos los extras por defecto
        containerP2.SetActive(false);
        containerP3.SetActive(false);
        containerP4.SetActive(false);

        if (selectedIndex == 1) // Opción "2 Jugadores"
        {
            containerP2.SetActive(true);
        }
        else if (selectedIndex == 2) // Opción "4 Jugadores"
        {
            containerP2.SetActive(true);
            containerP3.SetActive(true);
            containerP4.SetActive(true);
        }
    }

    public void StartGame()
    {
        int[] playerOptions = { 1, 2, 4 };
        int actualPlayerCount = playerOptions[playerDropdown.value];

        gameSettings.playerCount = actualPlayerCount;
        gameSettings.selectedPlayerSpriteIndices = new int[actualPlayerCount];

        int.TryParse(holesInputField != null ? holesInputField.text : "", out int holes);
        gameSettings.totalHoles = Mathf.Max(1, holes);

        int.TryParse(strokesInputField.text, out int strokes);
        gameSettings.maxStrokesPerHole = strokes > 0 ? strokes : 6;

        if (timeSlider != null)
            gameSettings.turnTimeLimit = timeSlider.value;

        if (actualPlayerCount >= 1 && spriteDropdownP1 != null) gameSettings.selectedPlayerSpriteIndices[0] = spriteDropdownP1.value;
        if (actualPlayerCount >= 2 && spriteDropdownP2 != null) gameSettings.selectedPlayerSpriteIndices[1] = spriteDropdownP2.value;
        if (actualPlayerCount >= 4)
        {
            if (spriteDropdownP3 != null) gameSettings.selectedPlayerSpriteIndices[2] = spriteDropdownP3.value;
            if (spriteDropdownP4 != null) gameSettings.selectedPlayerSpriteIndices[3] = spriteDropdownP4.value;
        }

        gameSettings.playerNames = new string[actualPlayerCount];
        for (int i = 0; i < actualPlayerCount; i++)
        {
            string n = (nameInputs != null && i < nameInputs.Length && nameInputs[i] != null) ? nameInputs[i].text.Trim() : "";
            gameSettings.playerNames[i] = string.IsNullOrEmpty(n) ? $"Player {i + 1}" : n;
        }

        MatchSession.Reset(actualPlayerCount, gameSettings.matchTimeLimit);
        SceneManager.LoadScene("ControlsScene");
    }
}