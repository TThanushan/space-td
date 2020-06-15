using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    [Header ("Color Settings")]
    public Color highLightColor;

    public Color buildHighLightColor;

    public Vector3 positionOffSet;

    public float colorLerpSpeed;

    public GameObject turret;
    public TurretBluePrint turretBlueprint;

    BuildManagerScript buildManager;

    SpriteRenderer sprite;

	GameObject turretPreview;
	GameObject turretPreviewRange;

	//First color of the sprite.
	Color startColor;

    bool lerpActivated = false;

    float startTime;

    float t;

    PlayerStatsScript playerStatsScript;

    void Start()
    {
        buildManager = BuildManagerScript.instance;
        playerStatsScript = PlayerStatsScript.instance;
    }

    void Update()
    {
        //Change the color of the sprite.
        ColorLerp(startColor, highLightColor);
	}

	void Awake()
    {
        lerpActivated = false;
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            sprite = gameObject.GetComponent<SpriteRenderer>();
            startColor = sprite.color;
        }
    }

    void BuildTurret(TurretBluePrint bluePrint)
    {
        if (!bluePrint)
            return;
        playerStatsScript.money -= bluePrint.cost;

        GameObject newTurret = PoolObject.instance.GetPoolObject(bluePrint.gameObject);
        newTurret.transform.position = transform.position;
        turret = newTurret;

        turretBlueprint = bluePrint;

        buildManager.SetTurretToBuild(null);

        BuildToolbar.instance.DisableAllLocationSprite();
        Vector2 mousePos = BuildToolbar.instance.GetMouseRealPosition();
        UIScript.instance.DisplayText("-" + bluePrint.cost.ToString() + " $", mousePos, 1, "Red");
        DisplayEffect(NodeUI.instance.upgradeEffect);
        AudioManager.instance.Play("Turret Build");
        BuildToolbar.instance.ShowToolbar();

    }

    public void UpgradeTurret()
    {
        Vector3 mousePos = (Vector3)BuildToolbar.instance.GetMouseRealPosition();
        if (!turretBlueprint.UpgradeAvailable())
        {
            AudioManager.instance.Play("Error");
        }
        else if (PlayerHasEnoughMoney())
        {
            UIScript.instance.DisplayText("-" + turretBlueprint.cost.ToString() + " $", mousePos, 1,  "Red");
            playerStatsScript.money -= turretBlueprint.GetUpgradeCost();
            float damageDealt = turret.GetComponent<TowerScript>().damageDealt;
            int killCount = turret.GetComponent<TowerScript>().killCount;
            DestroyTurret();
            InstantiateUpgradeTurret();
            SetDamageDealtAndKillCOunt(damageDealt, killCount);
            UpdateNodeUITarget();
            DisplayEffect(NodeUI.instance.upgradeEffect);
            AudioManager.instance.Play("Turret Build");
            AudioManager.instance.Play("Upgrade");
        }
        else if (turretBlueprint.upgradePrefab == null)
            UIScript.instance.DisplayText("Error 404 !", mousePos, 2, "Red");
        else
        {
            AudioManager.instance.Play("Error");
            //UIScript.instance.DisplayText("Not enough money to Upgrade !", mousePos, 2, "Red");
        }
    }

    private void SetDamageDealtAndKillCOunt(float damageDealt, int killCount)
    {
        turret.GetComponent<TowerScript>().damageDealt = damageDealt;
        turret.GetComponent<TowerScript>().killCount = killCount;
    }


    void DestroyTurret()
    {
        turret.SetActive(false);
        turret = null;
    }

    void InstantiateUpgradeTurret()
    {
        GameObject newTurret = PoolObject.instance.GetPoolObject(turretBlueprint.upgradePrefab);
        newTurret.transform.position = transform.position;
        newTurret.GetComponent<TurretBluePrint>().PreviousCost
            = turretBlueprint.cost + turretBlueprint.PreviousCost;
        turret = newTurret;
        turretBlueprint = turret.GetComponent<TurretBluePrint>();
    }

    void UpdateNodeUITarget()
    {
        NodeUI nodeUI = NodeUI.instance;
        nodeUI.DisplayTurretRange(NodeUI.instance.GetNodeTarget, true);
        nodeUI.ShowRangeUpgrade(true);
        nodeUI.SetTarget(this);
    }

    bool PlayerHasEnoughMoney()
    {
        return playerStatsScript.money >= turretBlueprint.GetUpgradeCost();
    }

    void DisplayEffect(GameObject _effect)
    {
        if (_effect == null)
            return;
        GameObject newEffect = PoolObject.instance.GetPoolObject(_effect);
        newEffect.transform.position = transform.position;
    }

    public void SellTurret()
    {
        if (turret == null)
            return;
        int sellAmount = turretBlueprint.GetSellAmount();
        playerStatsScript.money +=  sellAmount;
        UIScript.instance.DisplayText("+" + sellAmount + "$" , Camera.main.ScreenToWorldPoint(Input.mousePosition), 2);
        buildManager.DeselectNode();
        turret.SetActive(false);
        turret = null;
        turretBlueprint = null;
        DisplayEffect(NodeUI.instance.SellingEffect);
    }

    void OnMouseDown()
    {
		if (EventSystem.current.IsPointerOverGameObject())
			return;
        if (!buildManager.CanBuild)
            return;
        if (turret != null)
        {
			buildManager.SelectNode(this);
			return;
        }
        NodeUI.instance.Hide();
        BuildTurret(buildManager.GetTurretToBuild());
    }

    void OnMouseEnter ()
    {
        StartColorFade();
        buildManager.mouseOverNode = true;
        if (!buildManager.GetSelectedNode)
            NodeUI.instance.DisplayTurretRange(this, true);
    }

    void StartColorFade()
    {
        lerpActivated = true;
        startTime = Time.time;
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0) && turret)
        {
            if (turret != null)
            {
                AudioManager.instance.Play("Usual Button");
                buildManager.SelectNode(this);
                return;
            }
            buildManager.nodeUI.Hide();
            BuildTurret(buildManager.GetTurretToBuild());
        }
    }

    void ColorLerp(Color startColor, Color endColor)
    {
        t = (Time.time - startTime) * colorLerpSpeed;
        if (lerpActivated)
            sprite.color = Color.Lerp(startColor, endColor, t);
        else
            sprite.color = Color.Lerp(endColor, startColor, t);
    }

    public Vector2 GetBuildPosition()
    {
        return new Vector2(transform.position.x, transform.position.y + 0.5f);
    }

    void OnMouseExit ()
	{
		buildManager.mouseOverNode = false;
		if (!buildManager.GetSelectedNode)
			buildManager.nodeUI.DisplayTurretRange(this, false);
		lerpActivated = false;  
        ColorLerp(highLightColor, startColor);
    }
}
