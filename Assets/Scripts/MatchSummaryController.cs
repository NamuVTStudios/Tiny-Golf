using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MatchSummaryController : MonoBehaviour
{
    [Header("UI Panel Reference")]
    [SerializeField] private GameObject summaryPanel;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI winnerResultText;
    [SerializeField] private TextMeshProUGUI scoreboardDetailsText;

    private void Awake()
    {
        // Blindaje: Nos aseguramos de que la pantalla de ganador empiece apagada
        if (summaryPanel != null) summaryPanel.SetActive(false);
    }

    // Método principal que calcula el podio al terminar la partida
    public void DisplayMatchResults(Dictionary<int, int> finalScores, int playerCount)
    {
        summaryPanel.SetActive(true); // Encendemos el panel con tu fondo verde de grilla

        int highestScore = -1;
        List<int> winningPlayersIndices = new List<int>();
        string scoreboardContent = "FINAL LEADERBOARD:\n\n";

        // Recorremos los datos basándonos en el ID del diccionario seguro
        for (int i = 0; i < playerCount; i++)
        {
            int playerScore = finalScores.ContainsKey(i) ? finalScores[i] : 0;
            scoreboardContent += $"Player {i + 1}: {playerScore} Points\n";

            if (playerScore > highestScore)
            {
                highestScore = playerScore;
                winningPlayersIndices.Clear();
                winningPlayersIndices.Add(i);
            }
            else if (playerScore == highestScore)
            {
                winningPlayersIndices.Add(i);
            }
        }

        // Pintamos los textos de manera limpia arriba de los casilleros de la imagen
        scoreboardDetailsText.text = scoreboardContent;

        if (winningPlayersIndices.Count > 1)
        {
            string tiedPlayers = "";
            for (int i = 0; i < winningPlayersIndices.Count; i++)
            {
                tiedPlayers += $"Player {winningPlayersIndices[i] + 1}";
                if (i < winningPlayersIndices.Count - 1) tiedPlayers += " & ";
            }
            winnerResultText.text = $"MATCH RESULT: TIE!\n({tiedPlayers})";
        }
        else
        {
            int winnerId = winningPlayersIndices[0];
            winnerResultText.text = $"🏆 WINNER: Player {winnerId + 1} 🏆";
        }
    }

    // Funciones obligatorias para los botones de cierre
    public void RestartMatch()
    {
        // Vuelve a cargar la escena de gameplay actual reiniciando todo el ciclo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        // Regresa a la pantalla de configuración inicial de la Fase 1
        SceneManager.LoadScene("MainMenu"); // Asegúrate de colocar el nombre exacto de tu escena de menú
    }
}