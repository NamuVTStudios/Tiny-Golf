using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public void SkipAndPlay()
    {
        SceneManager.LoadScene(MatchSession.LevelScenes[0]);
    }
}