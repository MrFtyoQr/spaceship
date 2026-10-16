using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public GameObject panel;
    public Slider healthSlider;
    public TextMeshProUGUI bossNameText;

    BossController currentBoss;

    void Start()
    {
        Hide();
    }

    void OnDestroy()
    {
        UnsubscribeBoss();
    }

    public void Show(BossController boss, string bossName)
    {
        UnsubscribeBoss();
        currentBoss = boss;

        if (panel != null) panel.SetActive(true);
        if (bossNameText != null) bossNameText.text = bossName;
        if (healthSlider != null)
        {
            healthSlider.maxValue = boss.MaxHealth;
            healthSlider.value = boss.CurrentHealth;
        }

        boss.OnHealthChanged += UpdateHealth;
        boss.OnBossDeath += HandleBossDeath;
    }

    public void Hide()
    {
        UnsubscribeBoss();
        if (panel != null) panel.SetActive(false);
    }

    void UnsubscribeBoss()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHealthChanged -= UpdateHealth;
            currentBoss.OnBossDeath -= HandleBossDeath;
            currentBoss = null;
        }
    }

    void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }

    void HandleBossDeath()
    {
        Hide();
    }
}
