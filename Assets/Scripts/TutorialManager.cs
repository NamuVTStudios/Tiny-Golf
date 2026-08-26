using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // Este método lo vincularemos al botón de omitir
    public void SkipAndPlay()
    {
        // Cargamos la escena de juego real de forma directa
        // Asegúrate de escribir el nombre exacto de tu escena de gameplay
        SceneManager.LoadScene("GameplayScene"); 
    }
}