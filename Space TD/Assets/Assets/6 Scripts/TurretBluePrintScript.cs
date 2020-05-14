using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TurretBluePrintScript{

    public string name;
    public GameObject prefab;

    public int cost;

    public GameObject upgradePrefab;
    public int upgradeCost;
    public TextMeshProUGUI priceText;

    public int GetSellAmount(bool _upgraded)
    {
        if (!_upgraded)
        {
            return (int)(cost *  0.8f);
        }
        else
            return (int)((cost + upgradeCost) * 0.8f);
    }


}
