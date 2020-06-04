using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NodeUI : MonoBehaviour {

    private Node nodeTarget;
    public Node nodeTargetHovering;

    public GameObject uI;

	public GameObject rangeSprite;
	public Transform rangeSpriteMask;

	public GameObject upgradeRangeSprite;

	public static NodeUI instance;

    public GameObject upgradeEffect;
    public GameObject SellingEffect;

    public static System.Action OverTurret;
    public static System.Action NotOverTurret;

    void Awake()
    {
        if (instance == null)
            instance = this;
        SceneManager.sceneLoaded += SetEventToNull;
    }

    void SetEventToNull(Scene scene, LoadSceneMode mode)
    {
        OverTurret = null;
        NotOverTurret = null;
    }

    void Start()
    {
		rangeSprite = transform.Find("Canvas/UpgradesPanel/RangeSprite").gameObject;
		upgradeRangeSprite = transform.Find("Canvas/UpgradesPanel/UpgradeRangeSprite").gameObject;
		rangeSpriteMask = transform.Find("Canvas/UpgradesPanel/Mask").gameObject.transform;
		uI = transform.Find("Canvas/UpgradesPanel/Buttons").gameObject;
        if (nodeTarget == null)
            Hide();
    }

    public Node GetNodeTarget
    {
        get
        {
            return nodeTarget;
        }
    }

	public void ShowRangeUpgrade(bool show)
	{
        if (show && nodeTarget.turretBlueprint.IsUpgradeAvailable())
        {
            TowerScript towerScript = nodeTarget.turretBlueprint.upgradePrefab.GetComponent<TowerScript>();
            float towerRange = towerScript.attackRange / 1.3f;
            upgradeRangeSprite.transform.localScale = new Vector3(towerRange, towerRange, 0);
            upgradeRangeSprite.transform.position = nodeTarget.transform.position;
            upgradeRangeSprite.SetActive(true);
        }
        else
            upgradeRangeSprite.SetActive(false);
    }

    public void SetTarget(Node _node)
    {
        nodeTarget = _node;

        TextMeshProUGUI upgradeText = transform.Find("Canvas/UpgradesPanel/Buttons/UpgradeButton/Text").GetComponent<TextMeshProUGUI>();


        TextMeshProUGUI sellText = transform.Find("Canvas/UpgradesPanel/Buttons/SellButton/Text").GetComponent<TextMeshProUGUI>();
        sellText.text = _node.turretBlueprint.GetSellAmount() + "$";
        if(nodeTarget.turretBlueprint.UpgradeAvailable())
            upgradeText.text = _node.turretBlueprint.GetUpgradeCost() + "$";
        else
            upgradeText.text = "Done";
        

        transform.position = nodeTarget.GetBuildPosition();
        uI.SetActive(true);

        DisplayTurretRange(nodeTarget, true);
    }

    public void Hide()
    {
        if(nodeTarget != null)
            DisplayTurretRange(nodeTarget, false);
        uI.SetActive(false);
    }

    public void Upgrade()
    {
        nodeTarget.UpgradeTurret();

        TextMeshProUGUI sellText = transform.Find("Canvas/UpgradesPanel/Buttons/SellButton/Text").GetComponent<TextMeshProUGUI>();
        TurretBluePrint turretBluePrint = BuildManagerScript.instance.GetTurretToBuild();
        if (turretBluePrint != null)
            sellText.text = turretBluePrint.GetSellAmount() + "$";
    }

    public void Sell()
    {
        nodeTarget.SellTurret();
    }

	public void DisplayTurretRange(Node _node, bool state)
    {
        //If the _node is null or the rangeSprite is disable.
        if (_node == null || rangeSprite.activeSelf == false || _node.turret == null)
            return;
		rangeSprite.transform.position = _node.turret.transform.position;
		rangeSpriteMask.position = _node.turret.transform.position;

        if (state == true)
        {
            float towerRange = _node.turret.GetComponent<TowerScript>().attackRange / 1.3f;
            rangeSprite.transform.localScale = new Vector3(towerRange, towerRange, 0);
            nodeTargetHovering = _node;
            OverTurret?.Invoke();
        }
        else
        {
            rangeSprite.transform.localScale = Vector3.zero;
            nodeTargetHovering = null;
            NotOverTurret?.Invoke();
        }
    }

}
