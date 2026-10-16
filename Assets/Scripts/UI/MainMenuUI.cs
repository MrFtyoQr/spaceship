using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button shipSelectButton;
    public Button quitButton;
    public TextMeshProUGUI pointsText;

    void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(Play);
        if (shipSelectButton != null) shipSelectButton.onClick.AddListener(OpenShipSelect);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);
        UpdatePoints();
    }

    void UpdatePoints()
    {
        if (pointsText != null && GameManager.Instance != null)
            pointsText.text = $"Puntos: {GameManager.Instance.TotalPoints}";
    }

    void Play()
    {
        SceneManager.LoadScene("Game");
    }

    void OpenShipSelect()
    {
        SceneManager.LoadScene("ShipSelect");
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
