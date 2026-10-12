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
        if (summaryPanel != null) summaryPanel.SetActive(false);
    }

    private string NameOf(int i, string[] names)
    {
        return (names != null && i < names.Length) ? names[i] : $"Player {i + 1}";
    }

    public void DisplayMatchResults(Dictionary<int, int> finalScores, int playerCount, string[] names)
    {
        summaryPanel.SetActive(true);

        int highestScore = -1;
        List<int> winners = new List<int>();
        string content = "FINAL LEADERBOARD:\n\n";

        for (int i = 0; i < playerCount; i++)
        {
            int score = finalScores.ContainsKey(i) ? finalScores[i] : 0;
            content += $"{NameOf(i, names)}: {score} Points\n";

            if (score > highestScore)
            {
                highestScore = score;
                winners.Clear();
                winners.Add(i);
            }
            else if (score == highestScore)
            {
                winners.Add(i);
            }
        }

        scoreboardDetailsText.text = content;

        if (winners.Count > 1)
        {
            string tied = "";
            for (int i = 0; i < winners.Count; i++)
            {
                tied += NameOf(winners[i], names);
                if (i < winners.Count - 1) tied += " & ";
            }
            winnerResultText.text = $"MATCH RESULT: TIE!\n({tied})";
        }
        else
        {
            winnerResultText.text = $"🏆 WINNER: {NameOf(winners[0], names)} 🏆";
        }
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        MatchSession.Restart();
        SceneManager.LoadScene(MatchSession.LevelScenes[0]);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}