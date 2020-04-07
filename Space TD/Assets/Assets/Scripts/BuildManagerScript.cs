using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManagerScript : MonoBehaviour {

    public GameObject standardTurretPrefabs;

    public static BuildManagerScript instance;

    private TurretBluePrintScript turretToBuild;

    private Node selectedNode;

    public NodeUI nodeUI;

	public bool mouseOverNode;

	private void Update()
	{
		if (!mouseOverNode && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			BuildManagerScript.instance.DeselectNode();
		}
	}

	void Awake()
    {
        nodeUI = GameObject.Find("NodeUI").GetComponent<NodeUI>();
        if (instance != null)
        {
            print("There is already a build Manager !");
            return;
        }
        else
            instance = this;


    }
        
    public bool CanBuild{ get { return turretToBuild != null; } }

    public bool HasMoney { get { return PlayerStatsScript.instance.money >= turretToBuild.cost; } }



    public void SetTurretToBuild(TurretBluePrintScript turret)
    {
        turretToBuild = turret;
        DeselectNode();
    }

    public void SetTurretToBuild(GameObject turret, int cost)
    {
        turretToBuild = new TurretBluePrintScript();
        turretToBuild.prefab = turret;
        turretToBuild.cost = cost;


    }

	public Node GetSelectedNode
	{
		get
		{
			return selectedNode;
		}
	}

    public void DeselectNode()
    {
        
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }

        selectedNode = node;

        nodeUI.SetTarget(node);
    }

    public TurretBluePrintScript GetTurretToBuild()
    {
        return turretToBuild;
    }


	void Start () {
        turretToBuild = ShopScript.instance.standardTurret;
//        SetTurretToBuild(standardTurretPrefabs, 15);
    }
	

}
