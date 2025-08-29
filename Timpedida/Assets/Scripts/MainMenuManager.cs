using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(LoadGame);
        _quitButton.onClick.AddListener(QuitGame);

    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(LoadGame);
        _quitButton.onClick.RemoveListener(QuitGame);
    }

    private void LoadGame() => SceneManager.LoadScene("Game");
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
