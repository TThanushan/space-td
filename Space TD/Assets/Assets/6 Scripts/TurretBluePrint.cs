using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurretBluePrint : MonoBehaviour{

    public int cost;

    public GameObject upgradePrefab;
    private float previousCost;

    public float PreviousCost { get => previousCost; set => previousCost = value; }

    public int GetSellAmount()
    {
        return (int)(cost *  0.75f + PreviousCost);
    }

    public bool IsUpgradeAvailable()
    {
        return upgradePrefab != null;
    }

    public int GetUpgradeCost()
    {
        return upgradePrefab.GetComponent<TurretBluePrint>().cost;
    }

    public bool UpgradeAvailable()
    {
        return upgradePrefab != null;
    }
}
