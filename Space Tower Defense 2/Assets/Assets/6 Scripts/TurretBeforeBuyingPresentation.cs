using UnityEngine;
using System.Collections;
using TMPro;

public class TurretBeforeBuyingPresentation : MonoBehaviour
{
    public TextMeshProUGUI turretNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI specificText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI presentationText;

    public GameObject panel;

    public void DisplayPanel()
    {
        panel.SetActive(true);
    }
    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void UpdateTexts(string turretName)
    {
        TowerScript towerScript = BuildToolbar.instance.GetTurretPrefab(turretName).GetComponent<TowerScript>();
        UpdateBasicStatsText(towerScript);
        UpdatePriceText(towerScript);
        UpdatePresentationText(turretName);
        turretNameText.text = turretName + " Turret";
        specificText.text = GetSpecificText(towerScript);
    }

    private string GetSpecificText(TowerScript towerScript)
    {
        if (IsCannonTurret(towerScript))
        {
            return "Explosion Range : " + towerScript.bulletG.GetComponent<MissileBullet>().ExplosionRange;
        }
        else if (IsElectricTurret(towerScript))
            return "Ricochet : " + towerScript.lightningBounceCount;
        else if (IsChargingTurret(towerScript))
            return "Min Attackspeed : " + towerScript.minAttackspeed;
        else
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
    private void UpdatePresentationText(string _turretName)
    {
        BuildToolbar.Turret turret = BuildToolbar.instance.GetTurret(_turretName);
        presentationText.text = turret.presentationText;
    }

    private void UpdatePriceText(TowerScript towerScript)
    {
        TurretBluePrint turretBluePrint = towerScript.GetComponent<TurretBluePrint>();
        priceText.text = turretBluePrint.cost.ToString() + '$';
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
        else if (IsSlowTurret(towerScript))
            damageText.text = "Slow : " + towerScript.slowAmount + '%';
        else
            damageText.text = "Damage : " + towerScript.attackDamage.ToString();
    }
}
