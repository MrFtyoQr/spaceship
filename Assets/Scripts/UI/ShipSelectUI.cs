using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShipSelectUI : MonoBehaviour
{
    public Transform shipGrid;
    public GameObject shipCardPrefab;
    public TextMeshProUGUI pointsText;
    public Button backButton;

    ShipCard[] cards;

    void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(GoBack);
        PopulateGrid();
        UpdatePointsDisplay();
    }

    void PopulateGrid()
    {
        if (GameManager.Instance == null || GameManager.Instance.config == null) return;

        var ships = GameManager.Instance.config.allShips;
        cards = new ShipCard[ships.Length];

        for (int i = 0; i < ships.Length; i++)
        {
            GameObject cardObj = Instantiate(shipCardPrefab, shipGrid);
            var card = cardObj.GetComponent<ShipCard>();
            card.Setup(ships[i], i, this);
            cards[i] = card;
        }
        RefreshAllCards();
    }

    public void OnShipAction(int index)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.IsShipUnlocked(index))
        {
            gm.SelectShip(index);
        }
        else
        {
            gm.TryUnlockShip(index);
        }

        RefreshAllCards();
        UpdatePointsDisplay();
    }

    void RefreshAllCards()
    {
        if (cards == null) return;
        var gm = GameManager.Instance;
        for (int i = 0; i < cards.Length; i++)
        {
            bool unlocked = gm.IsShipUnlocked(i);
            bool selected = gm.SelectedShipIndex == i;
            bool canAfford = gm.TotalPoints >= cards[i].ShipData.unlockCost;
            cards[i].Refresh(unlocked, selected, canAfford);
        }
    }

    void UpdatePointsDisplay()
    {
        if (pointsText != null && GameManager.Instance != null)
            pointsText.text = $"Puntos: {GameManager.Instance.TotalPoints}";
    }

    void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
