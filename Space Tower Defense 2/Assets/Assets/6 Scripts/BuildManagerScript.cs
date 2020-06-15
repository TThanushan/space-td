using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BuildManagerScript : MonoBehaviour {

    public static BuildManagerScript instance;


    private TurretBluePrint turretToBuild;

    private Node selectedNode;

    public NodeUI nodeUI;

    public GameObject[] allNodes;

	public bool mouseOverNode;


    public static System.Action SelectNodeEvent;
    public static System.Action DeselectNodeEvent;

    private void Start()
    {
        allNodes = GameObject.FindGameObjectsWithTag("Node");
    }

    private void Update()
    {
		if (!mouseOverNode && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			DeselectNode();
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
        SceneManager.sceneLoaded += SetEventToNull;
     }

    void SetEventToNull(Scene scene, LoadSceneMode mode)
    {
        SelectNodeEvent = null;
        DeselectNodeEvent = null;
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

    public void SetTurretToBuild(TurretBluePrint turret)
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
        DeselectNodeEvent?.Invoke();
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
        SelectNodeEvent?.Invoke();
    }

    public void CancelBuilding()
    {
        turretToBuild = null;
    }

    public TurretBluePrint GetTurretToBuild()
    {
        return turretToBuild;
    }
}   
