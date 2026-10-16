using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipCard : MonoBehaviour
{
    public Image shipIcon;
    public TextMeshProUGUI shipNameText;
    public Slider speedBar;
    public Slider fireRateBar;
    public Slider damageBar;
    public Slider healthBar;
    public TextMeshProUGUI priceText;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public Image cardBackground;
    public GameObject lockedOverlay;
    public TextMeshProUGUI speedValue;
    public TextMeshProUGUI fireRateValue;
    public TextMeshProUGUI damageValue;
    public TextMeshProUGUI healthValue;

    ShipData shipData;
    int shipIndex;
    ShipSelectUI parentUI;

    public ShipData ShipData => shipData;

    public void Setup(ShipData data, int index, ShipSelectUI parent)
    {
        shipData = data;
        shipIndex = index;
        parentUI = parent;

        if (shipIcon != null && data.icon != null)
        {
            shipIcon.sprite = data.icon;
            shipIcon.preserveAspect = true;
        }
        if (shipNameText != null)
            shipNameText.text = string.IsNullOrEmpty(data.shipName) ? $"Ship {index + 1}" : data.shipName;

        if (speedBar != null) { speedBar.maxValue = 10f; speedBar.value = data.moveSpeed; }
        if (fireRateBar != null) { fireRateBar.maxValue = 8f; fireRateBar.value = data.fireRate; }
        if (damageBar != null) { damageBar.maxValue = 5f; damageBar.value = data.bulletDamage; }
        if (healthBar != null) { healthBar.maxValue = 200f; healthBar.value = data.maxHealth; }

        if (speedValue != null) speedValue.text = data.moveSpeed.ToString("0.0");
        if (fireRateValue != null) fireRateValue.text = data.fireRate.ToString("0.0");
        if (damageValue != null) damageValue.text = data.bulletDamage.ToString("0.0");
        if (healthValue != null) healthValue.text = data.maxHealth.ToString("0");

        if (actionButton != null)
            actionButton.onClick.AddListener(() => parentUI.OnShipAction(shipIndex));
    }

    public void Refresh(bool unlocked, bool selected, bool canAfford)
    {
        if (lockedOverlay != null)
            lockedOverlay.SetActive(!unlocked);

        if (selected)
        {
            if (actionButtonText != null) actionButtonText.text = "EQUIPADA";
            if (actionButton != null) actionButton.interactable = false;
            if (cardBackground != null) cardBackground.color = new Color(0.1f, 0.7f, 0.3f, 0.6f);
            if (priceText != null) priceText.text = "EN USO";
        }
        else if (unlocked)
        {
            if (actionButtonText != null) actionButtonText.text = "ELEGIR";
            if (actionButton != null) actionButton.interactable = true;
            if (cardBackground != null) cardBackground.color = new Color(0.15f, 0.25f, 0.5f, 0.6f);
            if (priceText != null) priceText.text = "DESBLOQUEADA";
        }
        else
        {
            if (actionButtonText != null) actionButtonText.text = canAfford ? "COMPRAR" : "BLOQUEADA";
            if (actionButton != null) actionButton.interactable = canAfford;
            if (cardBackground != null) cardBackground.color = canAfford
                ? new Color(0.6f, 0.5f, 0.1f, 0.6f)
                : new Color(0.3f, 0.3f, 0.3f, 0.6f);
            if (priceText != null) priceText.text = $"{shipData.unlockCost:N0} pts";
        }
    }
}
