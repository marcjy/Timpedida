using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
    }

    private void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
}
