﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    [Header ("Color Settings")]
    public Color highLightColor;

    public Color buildHighLightColor;

    public Vector3 positionOffSet;

    public float colorLerpSpeed;

    [HideInInspector] 
    public GameObject turret;
    [HideInInspector]
    public TurretBluePrintScript turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    BuildManagerScript buildManager;

    SpriteRenderer sprite;

	GameObject turretPreview;
	GameObject turretPreviewRange;

	//First color of the sprite.
	Color startColor;

    bool lerpActivated = false;

    float startTime;

    float t;


    void Start()
    {
        buildManager = BuildManagerScript.instance;
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

    void BuildTurret(TurretBluePrintScript bluePrint)
    {
        if (!bluePrint.prefab)
            return;
        else if (PlayerStatsScript.instance.money >= bluePrint.cost && bluePrint.prefab)
        {
            Vector2 mousePos = BuildToolbar.instance.GetMouseRealPosition();
            UIScript.instance.DisplayText("-" + bluePrint.cost.ToString() + " $", mousePos, 1, "Red");
            PlayerStatsScript.instance.money -= bluePrint.cost;
            GameObject newTurret = PoolObject.instance.GetPoolObject(bluePrint.prefab);
            newTurret.transform.position = transform.position;
            turret = newTurret;
            turretBlueprint = bluePrint;
            DisplayEffect(NodeUI.instance.upgradeEffect);
            AudioManager.instance.Play("Turret Build");
            BuildToolbar.instance.ShowToolbar();
            buildManager.SetTurretToBuild(null);
        }
        else
        {
            AudioManager.instance.Play("Error");

            UIScript.instance.DisplayText("Not enough money !", Camera.main.ScreenToWorldPoint(Input.mousePosition), 2, "Red");
        }
    }

    public void UpgradeTurret()
    {
        Vector3 mousePos = (Vector3)BuildToolbar.instance.GetMouseRealPosition();
        if (isUpgraded == true)
        {
            UIScript.instance.DisplayText("Max Upgrade !", mousePos + Vector3.up, 2, "Red");
            AudioManager.instance.Play("Error");
        }
        else if (PlayerStatsScript.instance.money >= turretBlueprint.upgradeCost && turretBlueprint.upgradePrefab != null)
        {
            UIScript.instance.DisplayText("-" + turretBlueprint.cost.ToString() + " $", mousePos, 1,  "Red");
            PlayerStatsScript.instance.money -= turretBlueprint.upgradeCost;
            turret.SetActive(false);
            turret = null;
            GameObject newTurret = PoolObject.instance.GetPoolObject(turretBlueprint.upgradePrefab);
            newTurret.transform.position = transform.position;
            turret = newTurret;
            isUpgraded = true;
            NodeUI.instance.DisplayTurretRange(NodeUI.instance.GetNodeTarget, true);
            NodeUI.instance.SetTarget(this);
            DisplayEffect(NodeUI.instance.upgradeEffect);
            AudioManager.instance.Play("Turret Build");
            AudioManager.instance.Play("Upgrade");
        }
        else if (turretBlueprint.upgradePrefab == null)
            UIScript.instance.DisplayText("Error 404 !", mousePos, 2, "Red");
        else
        {
            AudioManager.instance.Play("Error");
            UIScript.instance.DisplayText("Not enough money to Upgrade !", mousePos, 2, "Red");
        }
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
        int sellAmount = turretBlueprint.GetSellAmount(isUpgraded);
        PlayerStatsScript.instance.money +=  sellAmount;
        UIScript.instance.DisplayText("+" + sellAmount + "$" , Camera.main.ScreenToWorldPoint(Input.mousePosition), 2);
        isUpgraded = false;
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
        buildManager.nodeUI.Hide();
        BuildTurret(buildManager.GetTurretToBuild());
    }

    void OnMouseEnter ()
    {
        StartColorFade();
        buildManager.mouseOverNode = true;
		if (!buildManager.GetSelectedNode)
			buildManager.nodeUI.DisplayTurretRange(this, true);
    }

    void StartColorFade()
    {
        lerpActivated = true;
        startTime = Time.time;
    }

    private void OnMouseOver()
    {
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