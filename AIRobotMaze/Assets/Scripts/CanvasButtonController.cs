using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtonController : MonoBehaviour
{
    public Button buttonOne;
    public Button buttonTwo;

    void Start()
    {
        buttonOne.onClick.AddListener(LoadPathfindingScene);
        buttonTwo.onClick.AddListener(QuitApp);
    }

    void LoadPathfindingScene()
    {
        Debug.Log("Loading PathfindingScene...");
        SceneManager.LoadScene("PathfindingScene");
    }

    void QuitApp()
    {
        Debug.Log("Quitting application...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // This stops play mode in the editor
#endif
    }
}