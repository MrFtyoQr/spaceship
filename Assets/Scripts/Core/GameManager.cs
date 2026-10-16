using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameConfig config;

    public int TotalPoints { get; private set; }
    public HashSet<int> UnlockedShips { get; private set; }
    public int SelectedShipIndex { get; private set; }

    public ShipData SelectedShip => config.allShips != null && SelectedShipIndex < config.allShips.Length
        ? config.allShips[SelectedShipIndex]
        : null;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        TotalPoints = SaveManager.LoadPoints();
        UnlockedShips = SaveManager.LoadUnlockedShips();
        SelectedShipIndex = SaveManager.LoadSelectedShip();
    }

    public void AddPoints(int amount)
    {
        TotalPoints += amount;
        SaveManager.SavePoints(TotalPoints);
    }

    public bool TryUnlockShip(int index)
    {
        if (UnlockedShips.Contains(index)) return false;
        if (config.allShips == null || index >= config.allShips.Length) return false;

        int cost = config.allShips[index].unlockCost;
        if (TotalPoints < cost) return false;

        TotalPoints -= cost;
        UnlockedShips.Add(index);
        SaveManager.SavePoints(TotalPoints);
        SaveManager.SaveUnlockedShips(UnlockedShips);
        return true;
    }

    public void SelectShip(int index)
    {
        if (!UnlockedShips.Contains(index)) return;
        SelectedShipIndex = index;
        SaveManager.SaveSelectedShip(index);
    }

    public bool IsShipUnlocked(int index)
    {
        return UnlockedShips.Contains(index);
    }
}
