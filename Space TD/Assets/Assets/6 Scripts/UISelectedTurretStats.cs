using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISelectedTurretStats : MonoBehaviour
{
    public TextMeshProUGUI turretNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI damageDealtText;
    public TextMeshProUGUI killCounterText;


    public TextMeshProUGUI damageUpgradeText;
    public TextMeshProUGUI attackSpeedUpgradeText;
    public TextMeshProUGUI rangeUpgradeText;

    public GameObject upgradeStatsPanel;
    public GameObject panel;

    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI sellAmountText;

    private void Start()
    {
        InvokeRepeating("UpdateText", 0f, 0.5f);
        AddSubscribers();
    }

    private void OnSelected()
    {
        UpdateText();
        panel.SetActive(true);
    }

    private void OnDeselect()
    {
        panel.SetActive(false);
    }

    private void AddSubscribers()
    {
        BuildManagerScript.SelectNodeEvent += OnSelected;
        BuildManagerScript.DeselectNodeEvent += OnDeselect;
        NodeUI.OverTurret += OnSelected;
        NodeUI.NotOverTurret += OnDeselect;
    }

    public void DisplayUpgradeStat()
    {
        upgradeStatsPanel.SetActive(true);
        TowerScript towerScript = GetSelectUpgradeTowerScript();
        if (!towerScript)
            return;
        if (towerScript.bulletG && towerScript.bulletG.GetComponent<MissileBullet>())
            damageUpgradeText.text = "=> " + towerScript.bulletG.GetComponent<MissileBullet>().ExplosionDamage;
        else
            damageUpgradeText.text = "=> " + towerScript.attackDamage;
        attackSpeedUpgradeText.text = "=> " + towerScript.attackSpeed;
        rangeUpgradeText.text = "=> " + towerScript.attackRange;
    }
    
    public void HideUpgradeStatsPanel()
    {
        upgradeStatsPanel.SetActive(false);
    }

    private void UpdateText()
    {
        TowerScript towerScript = GetNodeUISelectedTowerScript();
        if (towerScript || (towerScript = GetBuildManagerSelectedTowerScript()))
        {
            turretNameText.text = towerScript.name;
            UpdateBasicStatsText(towerScript);
            UpdatedDamageDealtAndKillCounterText(towerScript);
            UpdateButtonText();
        }
    }

    private void UpdateBasicStatsText(TowerScript towerScript)
    {
        UpdateDamageText(towerScript);
        attackSpeedText.text = "Attack Speed : " + towerScript.attackSpeed.ToString();
        rangeText.text = "Range : " + towerScript.attackRange.ToString();
    }

    private void UpdateDamageText(TowerScript towerScript)
    {
        if (towerScript.bulletG && towerScript.bulletG.GetComponent<MissileBullet>())
            damageText.text = "Damage : " + towerScript.bulletG.GetComponent<MissileBullet>().ExplosionDamage.ToString();
        else
            damageText.text = "Damage : " + towerScript.attackDamage.ToString();
    }

    private void UpdatedDamageDealtAndKillCounterText(TowerScript towerScript)
    {
        killCounterText.text = "Enemy Killed : " + towerScript.killCount.ToString();
        damageDealtText.text = "Damage Dealt : " + towerScript.damageDealt.ToString();
        sellAmountText.text = "Sell : " + GetNodeUISelectedTowerScript().GetComponent<TurretBluePrint>().GetSellAmount().ToString() + " $";
    }

    private void UpdateButtonText()
    {
        if (!GetSelectUpgradeTowerScript())
            upgradeCostText.text = "Done";
        else
            upgradeCostText.text = "Upgrade : " + GetSelectUpgradeTowerScript().GetComponent<TurretBluePrint>().cost + " $";
    }

    private TowerScript GetSelectUpgradeTowerScript()
    {
        GameObject upgradePrefab = GetUpgradePrefab();
        if (!upgradePrefab)
            return null;
        return upgradePrefab.GetComponent<TowerScript>();
    }

    private GameObject GetUpgradePrefab()
    {
        return GetNodeUISelectedTowerScript().GetComponent<TurretBluePrint>().upgradePrefab;
    }

    private TowerScript GetNodeUISelectedTowerScript()
    {
        Node node = NodeUI.instance.nodeTargetHovering;
        if (!node)
            return null;
        return node.turret.GetComponent<TowerScript>();
    }

    private TowerScript GetBuildManagerSelectedTowerScript()
    {
        Node node = BuildManagerScript.instance.GetSelectedNode;
        if (!node)
            return null;
        return node.GetComponent<TowerScript>();
    }
}
