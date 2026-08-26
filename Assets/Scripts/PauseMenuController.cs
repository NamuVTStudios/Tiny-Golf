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
        // Blindaje: Nos aseguramos de que el menú de pausa empiece 100% cerrado
        if (pausePanel != null) pausePanel.SetActive(false);
        if (controlsSubPanel != null) controlsSubPanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; // Aseguramos que el tiempo corra normalmente
    }



    void Update()
    {
        // Escucha si el jugador presiona la tecla Escape o P para alternar la pausa
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        controlsSubPanel.SetActive(false); // Nos aseguramos de que empiece cerrado

        // Bloqueamos el script de tiro para que no se pueda interactuar con la bola de fondo
        if (shotController != null) shotController.enabled = false;

        // Congelamos el tiempo del motor de Unity (corrutinas, físicas, temporizadores se detienen)
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        controlsSubPanel.SetActive(false);

        // Devolvemos el tiempo a su velocidad normal
        Time.timeScale = 1f;

        // Reactivamos el sistema de tiro de forma segura
        if (shotController != null) shotController.enabled = true;
    }

    // Opción Obligatoria 1: Reiniciar la partida
    public void RestartMatch()
    {
        Time.timeScale = 1f; // Muy importante devolver el tiempo a 1 antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Opción Obligatoria 2: Consultar los controles
    public void ToggleControlsWindow(bool show)
    {
        controlsSubPanel.SetActive(show);
    }

    // Opción Obligatoria 3: Volver al menú principal
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Devolver el tiempo a 1 para que el menú principal funcione bien
        SceneManager.LoadScene("MainMenu"); // Coloca el nombre exacto de tu escena de la Fase 1
    }
}