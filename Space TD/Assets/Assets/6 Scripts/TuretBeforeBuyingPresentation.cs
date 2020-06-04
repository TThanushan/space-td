using UnityEngine;
using System.Collections;
using TMPro;

public class TuretBeforeBuyingPresentation : MonoBehaviour
{
    public TextMeshProUGUI turretNameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;
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
        else
            damageText.text = "Damage : " + towerScript.attackDamage.ToString();
    }
}
