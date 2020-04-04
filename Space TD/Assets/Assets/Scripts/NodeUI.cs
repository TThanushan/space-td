using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {

    private Node target;

    public GameObject uI;

    public GameObject rangeSprite;

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
        rangeSprite = transform.Find("Canvas/Range Sprite").gameObject;
        uI = transform.Find("Canvas/Buttons").gameObject;
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

    public void SetTarget(Node _node)
    {
        target = _node;

        Text upgradeText = transform.Find("Canvas/Buttons/UpgradeButton/Text").GetComponent<Text>();


        Text sellText = transform.Find("Canvas/Buttons/SellButton/Text").GetComponent<Text>();
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
        Text sellText = transform.Find("Canvas/Buttons/SellButton/Text").GetComponent<Text>();
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
		if (state == true)
		{
			rangeSprite.transform.localScale = new Vector3(_node.turret.GetComponent<TowerScript>().attackRange * 200,
				_node.turret.GetComponent<TowerScript>().attackRange * 200, 0);
		}
		else
			rangeSprite.transform.localScale = Vector3.zero;
        
    }

}
