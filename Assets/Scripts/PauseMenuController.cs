using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsSubPanel;

    [Header("Shot Controller Reference")]
    [SerializeField] private ShotController shotController;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (controlsSubPanel != null) controlsSubPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else if (GameManager.Instance != null && GameManager.Instance.CanPlayerShoot) PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        controlsSubPanel.SetActive(false);
        if (shotController != null) shotController.enabled = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        controlsSubPanel.SetActive(false);
        Time.timeScale = 1f;

        // Solo reactivamos el tiro si la partida lo permite en este momento
        if (shotController != null && GameManager.Instance != null && GameManager.Instance.CanPlayerShoot)
            shotController.enabled = true;
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        MatchSession.Restart();
        SceneManager.LoadScene(MatchSession.LevelScenes[0]);
    }

    public void ToggleControlsWindow(bool show)
    {
        controlsSubPanel.SetActive(show);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}