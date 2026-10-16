using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject panel;
    public Button resumeButton;
    public Button mainMenuButton;

    bool paused;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (panel != null) panel.SetActive(paused);
    }

    void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
