using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour {


    public TurretBluePrintScript standardTurret;

    public TurretBluePrintScript fastTurret;

    public TurretBluePrintScript multiTurret;

    public TurretBluePrintScript slowTurret;


    public TurretBluePrintScript burnTurret;

    public TurretBluePrintScript electricTurret;

    public TurretBluePrintScript superMultiTurret;

    public TurretBluePrintScript laserTurret;

    public static ShopScript instance;

    GameObject shopCursor;
     
    BuildManagerScript buildManager;

    
    /// <summary>
    /// Shop Cursor
    /// </summary>

    public Vector3 newShopCursorPosition;

    void Awake()
    {
        
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        
        shopCursor = gameObject.transform.Find("shopCursor").gameObject;

        buildManager = BuildManagerScript.instance;
        newShopCursorPosition = shopCursor.transform.position;
    }

    void Update()
    {
        
        shopCursor.transform.position = Vector2.Lerp(shopCursor.transform.position, newShopCursorPosition, Time.deltaTime * 6);
    }

    public void MoveShopCursor(float yPosition)
    {
		newShopCursorPosition = new Vector2(shopCursor.transform.position.x, yPosition);
		AudioManager.instance.Play("Jump", true);

    }

    public void SelectStandardTurret()
    {
        buildManager.SetTurretToBuild(standardTurret);
    }

    public void SelectFastTurret()
    {

        buildManager.SetTurretToBuild(fastTurret);

    }

    public void SelectMultiTurret()
    {

        buildManager.SetTurretToBuild(multiTurret);

    }

    public void SelectSlowTurret()
    {
        buildManager.SetTurretToBuild(slowTurret);
    }

    public void SelectBurnTurret()
    {
        buildManager.SetTurretToBuild(burnTurret);
    }

    public void SelectElectricTurret()
    {
        buildManager.SetTurretToBuild(electricTurret);
    }

    public void selectSuperMultiTurret()
    {
        buildManager.SetTurretToBuild(superMultiTurret);
    }

    public void selectLaserTurret()
    {
        buildManager.SetTurretToBuild(laserTurret);
    }


}
