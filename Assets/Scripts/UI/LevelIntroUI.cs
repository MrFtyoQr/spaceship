using UnityEngine;
using TMPro;
using System.Collections;

public class LevelIntroUI : MonoBehaviour
{
    public TextMeshProUGUI levelIntroText;
    public TextMeshProUGUI subText;
    public CanvasGroup canvasGroup;
    public LevelManager levelManager;

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (levelManager != null)
            levelManager.OnLevelStart += ShowIntro;
    }

    void OnDestroy()
    {
        if (levelManager != null)
            levelManager.OnLevelStart -= ShowIntro;
    }

    void ShowIntro(int level, bool isBoss)
    {
        StopAllCoroutines();
        if (levelIntroText != null)
            levelIntroText.text = isBoss ? "JEFE FINAL!" : $"NIVEL {level}";
        if (subText != null)
            subText.text = isBoss ? "Derrota la nave enemiga!" : "Destruye todos los asteroides!";

        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1.5f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            canvasGroup.alpha = 1f - t;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
