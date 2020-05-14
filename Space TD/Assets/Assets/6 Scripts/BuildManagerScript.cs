using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManagerScript : MonoBehaviour {

    public static BuildManagerScript instance;

    private TurretBluePrintScript turretToBuild;

    private Node selectedNode;

    public NodeUI nodeUI;

    public GameObject[] allNodes;

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
        if (instance != null)
        {
            print("There is already a build Manager !");
            return;
        }
        else
            instance = this;
        nodeUI = GameObject.Find("NodeUI").GetComponent<NodeUI>();
        allNodes = GameObject.FindGameObjectsWithTag("Node");
     }
    

    public Transform GetNearestNode(Vector2 fromTransform)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject node in allNodes)
        {
            float distance = Vector2.Distance(node.transform.position, fromTransform);
            if (distance < minDist)
            {
                closest = node.transform;
                minDist = distance;
            }
        }
        return closest;
    }

    public bool CanBuild{ get { return turretToBuild != null; } }

    public bool HasMoneyToBuildTurret(int cost)
    {
        return PlayerStatsScript.instance.money >= cost;
    }

    public void SetTurretToBuild(TurretBluePrintScript turret)
    {
        turretToBuild = turret;
        DeselectNode();
        if (turret != null)
            BuildToolbar.instance.HideToolbar();
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

    public void CancelBuilding()
    {
        turretToBuild = null;
    }

    public TurretBluePrintScript GetTurretToBuild()
    {
        return turretToBuild;
    }
}   
