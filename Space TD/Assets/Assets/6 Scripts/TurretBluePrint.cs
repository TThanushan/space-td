using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurretBluePrint : MonoBehaviour{

    public int cost;

    public GameObject upgradePrefab;

    public int GetSellAmount()
    {
        return (int)(cost *  0.8f);
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
