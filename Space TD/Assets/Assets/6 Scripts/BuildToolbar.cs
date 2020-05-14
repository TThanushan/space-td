using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolbar : MonoBehaviour {

    public static BuildToolbar instance;

    public List<TurretBluePrintScript> allTurretBluePrints;

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
        foreach (TurretBluePrintScript blueprint in allTurretBluePrints)
        {
            if (!blueprint.priceText.IsActive())
                continue;
            if (buildManager.HasMoneyToBuildTurret(blueprint.cost))
                blueprint.priceText.color = new Color(0.6593881f, 1, 0, 1);
            else
                blueprint.priceText.color = new Color(0.6593881f, 1, 0, 0.2117647f);
            blueprint.priceText.text = blueprint.cost.ToString() + " $";
        }
    }

    void ShowPreviewOnMouse()
    {
        TurretBluePrintScript currentBlueprint = buildManager.GetTurretToBuild();
        if (currentBlueprint == null)
        {
            DestroyPreview();
            return;
        }
        LoadAndGetPreviewPoolObject(ref currentBlueprint);

        float towerRange = currentBlueprint.prefab.GetComponent<TowerScript>().attackRange / 1.3f;
        turretPreviewRange.transform.localScale = new Vector3(towerRange, towerRange, 0);

        MoveTurretPreview(mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    public Vector2 GetMouseRealPosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void LoadAndGetPreviewPoolObject(ref TurretBluePrintScript currentBlueprint)
    {
        if (!turretPreview)
        {
            turretPreview = (GameObject)Resources.Load("TurretPreviews/Preview" + currentBlueprint.prefab.name);
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

    public TurretBluePrintScript GetTurretBluePrintScript(string name)
    {
        return allTurretBluePrints.Find(bluePrint => bluePrint.name == name);
    }

    public void SelectTurret(string name)
    {
        if (buildManager.HasMoneyToBuildTurret(GetTurretBluePrintScript(name).cost))
            buildManager.SetTurretToBuild(GetTurretBluePrintScript(name));
        else
        {
            AudioManager.instance.Play("Error");
            UIScript.instance.DisplayText("Not enough money !", new Vector2(0, -5), 2, "Red");
        }
    }

}
