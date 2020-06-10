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
    public TextMeshProUGUI specificText;
    public TextMeshProUGUI damageDealtText;
    public TextMeshProUGUI killCounterText;


    public TextMeshProUGUI damageUpgradeText;
    public TextMeshProUGUI attackSpeedUpgradeText;
    public TextMeshProUGUI rangeUpgradeText;
    public TextMeshProUGUI specificUpgradeText;

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
        damageUpgradeText.text = "=> " + GetAttackDamage(towerScript).ToString();
        if (IsSlowTurret(towerScript))
            damageUpgradeText.text += '%';
        attackSpeedUpgradeText.text = "=> " + towerScript.attackSpeed;
        rangeUpgradeText.text = "=> " + towerScript.attackRange;
        if (towerScript.towerEffect != TowerScript.TowerEffect.noEffect)
            specificUpgradeText.text = "=> " + GetSpecificUpgradeText(towerScript);
    }
    
    public void HideUpgradeStatsPanel()
    {
        upgradeStatsPanel.SetActive(false);
    }

    private float GetAttackDamage(TowerScript towerScript)
    {
        if (IsCannonTurret(towerScript))
            return towerScript.bulletG.GetComponent<MissileBullet>().ExplosionDamage;
        else if (IsSlowTurret(towerScript))
            return towerScript.slowAmount;
        return towerScript.attackDamage;
    }

    private string GetSpecificText(TowerScript towerScript)
    {
        if (IsCannonTurret(towerScript))
            return "Explosion Range : " + towerScript.bulletG.GetComponent<MissileBullet>().ExplosionRange;
        else if (IsElectricTurret(towerScript))
            return "Ricochet : " + towerScript.lightningBounceCount;
        else if (IsChargingTurret(towerScript))
            return "Min Attackspeed : " + towerScript.minAttackspeed;
        return string.Empty;
    }

    private string GetSpecificUpgradeText(TowerScript towerScript)
    {
        if (IsCannonTurret(towerScript))
            return towerScript.bulletG.GetComponent<MissileBullet>().ExplosionRange.ToString();
        else if (IsElectricTurret(towerScript))
            return towerScript.lightningBounceCount.ToString();
        else if (IsChargingTurret(towerScript))
            return towerScript.minAttackspeed.ToString();
        return string.Empty;
    }

    private bool IsSlowTurret(TowerScript towerScript)
    {
        return towerScript.towerEffect == TowerScript.TowerEffect.slowTarget;
    }

    private bool IsChargingTurret(TowerScript towerScript)
    {
        return towerScript.towerEffect == TowerScript.TowerEffect.ChargingTurret;
    }

    private bool IsElectricTurret(TowerScript towerScript)
    {
        return towerScript.towerEffect == TowerScript.TowerEffect.Electric;
    }

    private bool IsCannonTurret(TowerScript towerScript)
    {
        return towerScript.bulletG && towerScript.bulletG.GetComponent<MissileBullet>();
    }

    private void UpdateText()
    {
        TowerScript towerScript = GetNodeUISelectedTowerScript();
        if (towerScript || (towerScript = GetBuildManagerSelectedTowerScript()))
        {
            turretNameText.text = towerScript.name;
            UpdateBasicStatsText(towerScript);
            specificText.text = GetSpecificText(towerScript);
            UpdatedDamageDealtAndKillCounterText(towerScript);
            UpdateButtonText();
        }
    }

    private void UpdateBasicStatsText(TowerScript towerScript)
    {
        if (IsSlowTurret(towerScript))
            damageText.text = "Slow : " + GetAttackDamage(towerScript) + '%';
        else
            damageText.text = "Damage : " + GetAttackDamage(towerScript);
        attackSpeedText.text = "Attack Speed : " + towerScript.attackSpeed;
        rangeText.text = "Range : " + towerScript.attackRange;
    }

    private void UpdatedDamageDealtAndKillCounterText(TowerScript towerScript)
    {
        killCounterText.text = "Enemy Killed : " + towerScript.killCount;
        damageDealtText.text = "Damage Dealt : " + towerScript.damageDealt;
        sellAmountText.text = "Sell : " + GetNodeUISelectedTowerScript().GetComponent<TurretBluePrint>().GetSellAmount() + " $";
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
