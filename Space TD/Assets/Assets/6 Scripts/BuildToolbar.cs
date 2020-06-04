using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildToolbar : MonoBehaviour {

    public static BuildToolbar instance;

    public Turret[] turrets;

    private Animator myAnimator;

    BuildManagerScript buildManager;

    GameObject turretPreviewRange;
    GameObject turretPreview;

    Camera mainCamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
        myAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        buildManager = BuildManagerScript.instance;
        InvokeRepeating("UpdatePriceText", 0f, 0.5f);

    }

    private void Update()
    {
        ShowPreviewOnMouse();
        CancelBuildingIfRightClick();
    }

    void CancelBuildingIfRightClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CancelBuilding();
            AudioManager.instance.Play("Usual Button");
        }
    }

    void UpdatePriceText()
    {
        foreach (Turret turret in turrets)
        {
            if (!turret.enoughMoneyShadeG.transform.parent.gameObject.activeSelf)
                continue;

            if (buildManager.HasMoneyToBuildTurret(turret.GetCost()))
                turret.enoughMoneyShadeG.SetActive(false);
            else
                turret.enoughMoneyShadeG.SetActive(true);
        }
    }

    void ShowPreviewOnMouse()
    {
        TurretBluePrint currentBlueprint = buildManager.GetTurretToBuild();
        if (currentBlueprint == null)
        {
            DestroyPreview();
            return;
        }
        LoadAndGetPreviewPoolObject(ref currentBlueprint);

        float towerRange = currentBlueprint.GetComponent<TowerScript>().attackRange / 1.3f;
        turretPreviewRange.transform.localScale = new Vector3(towerRange, towerRange, 0);

        MoveTurretPreview(mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    public Vector2 GetMouseRealPosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void LoadAndGetPreviewPoolObject(ref TurretBluePrint currentBlueprint)
    {
        if (!turretPreview)
        {
            turretPreview = (GameObject)Resources.Load("TurretPreviews/Preview" + currentBlueprint.name);
            turretPreview = PoolObject.instance.GetPoolObject(turretPreview);
            turretPreview.transform.position = transform.position;
            if (!turretPreviewRange)
            {
                turretPreviewRange = (GameObject)Resources.Load("TurretPreviews/RangeSprite");
                turretPreviewRange = PoolObject.instance.GetPoolObject(turretPreviewRange);
            }
            else
                turretPreviewRange.SetActive(true);
        }
    }

    void MoveTurretPreview(Vector2 destination)
    {
        turretPreviewRange.transform.position = destination;
        turretPreview.transform.position = destination;
    }

    public void CancelBuilding()
    {
        ShowToolbar();
        buildManager.CancelBuilding();
    }

    public void DestroyPreview()
    {
        if (turretPreview)
        {
            turretPreview.SetActive(false);
            turretPreview = null;
            turretPreviewRange.SetActive(false);
        }
    }

    public bool IsClosestNodeCloserThan(float range)
    {
        Transform node = buildManager.GetNearestNode(Input.mousePosition);
        return Vector2.Distance(node.position, Input.mousePosition) < range;
    }

    

    public void HideToolbar()
    {
        myAnimator.SetBool("Hide", true);
    }

    public void ShowToolbar()
    {
        myAnimator.SetBool("Hide", false);
    }

    public TurretBluePrint GetTurretBluePrint(string name)
    {
        foreach (Turret turret in turrets)
        {
            if (turret.name == name)
                return turret.GetBluePrint();
        }
        return null;
    }

    public Turret GetTurret(string turretName)
    {
        foreach (Turret turret in turrets)
        {
            if (turret.name == turretName)
                return turret;
        }
        return null;
    }

    public GameObject GetTurretPrefab(string turretName)
    {
        foreach (Turret turret in turrets)
        {
            if (turret.name == turretName)
                return turret.prefab;
        }
        return null;
    }

    public void SelectTurret(string name)
    {
        if (buildManager.HasMoneyToBuildTurret(GetTurretBluePrint(name).cost))
            buildManager.SetTurretToBuild(GetTurretBluePrint(name));
        else
        {
            AudioManager.instance.Play("Error");
            UIScript.instance.DisplayText("Not enough money !", new Vector2(0, -5), 2, "Red");
        }
    }

    [System.Serializable]
    public class Turret
    {
        public string name;
        public GameObject prefab;
        public GameObject enoughMoneyShadeG;
        public string presentationText;

        public TurretBluePrint GetBluePrint()
        {
            return prefab.GetComponent<TurretBluePrint>();
        }

        public int GetCost()
        {
            return prefab.GetComponent<TurretBluePrint>().cost;
        }
    }
}
