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

    [HideInInspector] 
    public GameObject turret;
    [HideInInspector]
    public TurretBluePrintScript turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    BuildManagerScript buildManager;

    SpriteRenderer sprite;

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
        if (PlayerStatsScript.instance.money >= bluePrint.cost && bluePrint.prefab != null)
        {
            UIScript.instance.DisplayText("-" + bluePrint.cost.ToString() + " $", Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, UIScript.instance.orangeColor);
            PlayerStatsScript.instance.money -= bluePrint.cost;
            GameObject newTurret = PoolObjectScript.instance.GetPoolObject(bluePrint.prefab);

            newTurret.transform.position = transform.position;

            turret = newTurret;

            turretBlueprint = bluePrint;

            DisplayEffect(NodeUI.instance.upgradeEffect);
            AudioManager.instance.Play("Cash Register", true);
            ShakeCamera.instance.Shake(0.1f, 0.05f);
        }
        else
        {
            AudioManager.instance.Play("SMS", false);

            UIScript.instance.DisplayText("Not enough money !", Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, Color.red);
        }
    }

    public void UpgradeTurret()
    {
        if (isUpgraded == true)
        {
            UIScript.instance.DisplayText("Upgrade Done !", Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.up, 6, Color.red);
            AudioManager.instance.Play("SMS", false);

        }
        else if (PlayerStatsScript.instance.money >= turretBlueprint.upgradeCost && turretBlueprint.upgradePrefab != null)
        {

            UIScript.instance.DisplayText("-" + turretBlueprint.upgradeCost.ToString() + " $", Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, UIScript.instance.orangeColor);
            PlayerStatsScript.instance.money -= turretBlueprint.upgradeCost;

            //Get rid of the old turret.
            turret.SetActive(false);
            turret = null;

            //Build a new one.
            GameObject newTurret = PoolObjectScript.instance.GetPoolObject(turretBlueprint.upgradePrefab);

            newTurret.transform.position = transform.position;

            turret = newTurret;

            isUpgraded = true;

            NodeUI.instance.DisplayTurretRange(NodeUI.instance.GetTarget, true);
            NodeUI.instance.SetTarget(this);


            //Display an effect.
            DisplayEffect(NodeUI.instance.upgradeEffect);

            AudioManager.instance.Play("Upgrade", true);
            ShakeCamera.instance.Shake(0.1f, 0.05f);

            
        }
        else if (turretBlueprint.upgradePrefab == null)
            UIScript.instance.DisplayText("Error 404 !", Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, Color.red);
        else
        {
            AudioManager.instance.Play("SMS", false);
            UIScript.instance.DisplayText("Not enough money to Upgrade !", Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, Color.red);
        }
    }

    
    void DisplayEffect(GameObject _effect)
    {
        if (_effect == null)
            return;
        
        GameObject newEffect = PoolObjectScript.instance.GetPoolObject(_effect);
        newEffect.transform.position = transform.position;
    }

    public void SellTurret()
    {
        if (turret == null)
            return;
        
        //Get the money given back to the player.
        int sellAmount = turretBlueprint.GetSellAmount(isUpgraded);

        //Give the money to the player.
        PlayerStatsScript.instance.money +=  sellAmount;

        //Display the money given back.
        UIScript.instance.DisplayText("+" + sellAmount + "$" , Camera.main.ScreenToWorldPoint(Input.mousePosition), 6, Color.green);

        //Reset the isUpgraded variable.
        isUpgraded = false;

        buildManager.DeselectNode();

        turret.SetActive(false);

        turret = null;

        turretBlueprint = null;
    
        DisplayEffect(NodeUI.instance.SellingEffect);
        AudioManager.instance.Play("Balloon Popping", true);
        ShakeCamera.instance.Shake(0.1f, 0.2f);


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

    //When the mouse cursor is on the object.
    void OnMouseEnter ()
    {
		buildManager.mouseOverNode = true;
		if (!buildManager.GetSelectedNode)
			buildManager.nodeUI.DisplayTurretRange(this, true);

		lerpActivated = true;
        startTime = Time.time;
        if (!Input.GetKey(KeyCode.Mouse0))
            return;
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

    void ColorLerp(Color startColor, Color endColor)
    {
        //Change the color of the sprite.
        t = (Time.time - startTime) * colorLerpSpeed;

        if (lerpActivated)
        {
            sprite.color = Color.Lerp(startColor, endColor, t);
        }

        else
            sprite.color = Color.Lerp(endColor, startColor, t);
        //Change the color of the sprite.
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
