using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {

    private Node target;

    public GameObject uI;

	public GameObject rangeSprite;
	public Transform rangeSpriteMask;

	public GameObject upgradeRangeSprite;

	public static NodeUI instance;

    public GameObject upgradeEffect;
    public GameObject SellingEffect;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	void Start()
    {
		rangeSprite = transform.Find("Canvas/UpgradesPanel/RangeSprite").gameObject;
		upgradeRangeSprite = transform.Find("Canvas/UpgradesPanel/UpgradeRangeSprite").gameObject;
		rangeSpriteMask = transform.Find("Canvas/UpgradesPanel/Mask").gameObject.transform;
		uI = transform.Find("Canvas/UpgradesPanel/Buttons").gameObject;
        if (target == null)
            Hide();
    }

	void Update()
	{

	}

	public Node GetTarget
    {
        get
        {
            return target;
        }
    }

	public void ShowRangeUpgrade(bool show)
	{
		if (show)
		{
			TowerScript towerScript = target.turretBlueprint.upgradePrefab.GetComponent<TowerScript>();
			float towerRange = towerScript.attackRange / 1.3f;
			upgradeRangeSprite.transform.localScale = new Vector3(towerRange, towerRange, 0);
			upgradeRangeSprite.transform.position = target.transform.position;
			upgradeRangeSprite.SetActive(true);
		}
		else
			upgradeRangeSprite.SetActive(false);
	}

	public void SetTarget(Node _node)
    {
        target = _node;

        Text upgradeText = transform.Find("Canvas/UpgradesPanel/Buttons/UpgradeButton/Text").GetComponent<Text>();


        Text sellText = transform.Find("Canvas/UpgradesPanel/Buttons/SellButton/Text").GetComponent<Text>();
//        sellText.text = BuildManagerScript.instance.GetTurretToBuild().GetSellAmount(_node.isUpgraded).ToString() + "$";
        sellText.text = _node.turretBlueprint.GetSellAmount(_node.isUpgraded) + "$";
        if(!target.isUpgraded)
            upgradeText.text = _node.turretBlueprint.upgradeCost.ToString() + "$";
        else
            upgradeText.text = "Done";
        

        transform.position = target.GetBuildPosition();
        uI.SetActive(true);
        
        DisplayTurretRange(target ,true);
    }

    public void Hide()
    {
        if(target != null)
            DisplayTurretRange(target, false);

        uI.SetActive(false);
    }

    public void Upgrade()
    {
        target.UpgradeTurret();
        Text sellText = transform.Find("Canvas/UpgradesPanel/Buttons/SellButton/Text").GetComponent<Text>();
        sellText.text = BuildManagerScript.instance.GetTurretToBuild().GetSellAmount(target.isUpgraded).ToString() + "$";

    }

    public void Sell()
    {
        target.SellTurret();
    }

	public void DisplayTurretRange(Node _node, bool state)
    {
        //If the _node is null or the rangeSprite is disable.
        if (_node == null || rangeSprite.activeSelf == false || _node.turret == null)
            return;
		//        1.65f

		rangeSprite.transform.position = _node.turret.transform.position;
		rangeSpriteMask.position = _node.turret.transform.position;

		if (state == true)
		{
			float towerRange = _node.turret.GetComponent<TowerScript>().attackRange / 1.3f;
			rangeSprite.transform.localScale = new Vector3(towerRange, towerRange, 0);
		}
		else
			rangeSprite.transform.localScale = Vector3.zero;
        
    }

}
