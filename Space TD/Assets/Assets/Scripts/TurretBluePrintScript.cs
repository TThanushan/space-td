using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBluePrintScript{

    public GameObject prefab;

    public int cost;

    public GameObject upgradePrefab;
    public int upgradeCost;

    public int GetSellAmount(bool _upgraded)
    {
        if (!_upgraded)
        {
            return (int)(cost *  0.8f);
        }
        else
            return (int)((cost + upgradeCost) * 0.8f);
    }

	public TowerScript GetTowerScript
	{
		get
		{
			return prefab.GetComponent<TowerScript>();
		}
	}
}
