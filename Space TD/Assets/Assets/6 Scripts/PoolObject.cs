using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    public static PoolObject instance;

    public GameObject[] enemies;

    public Transform[] pathArray;

    public string enemyTag;

    public int poolSize;

    public List<GameObject> unitPool;

    public GameObject _base;

    Transform binTransform;

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    void Start () {
        _base = GameObject.FindGameObjectWithTag("Base");
        GameObject _binTransform = GameObject.FindGameObjectWithTag("Bin");
        if (_binTransform)
            binTransform = _binTransform.GetComponent<Transform>();
        pathArray = GetPathPointsTransform();
    }

    void FixedUpdate () {
        FindEnemyFunc();
    }

    private Transform[] GetPathPointsTransform()
    {
        List<Transform> pathPointsList = new List<Transform>();
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Path Point"))
        {
            pathPointsList.Add(item.transform);
        }
        if (GameObject.FindGameObjectWithTag("Base"))
            pathPointsList.Add(GameObject.FindGameObjectWithTag("Base").transform);
        return pathPointsList.ToArray();
    }

    void FindEnemyFunc()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public GameObject GetPoolObject(GameObject unitP)
    {
        foreach (GameObject currentUnit in unitPool)
        {
            if (currentUnit && currentUnit.activeInHierarchy == false && currentUnit.name == unitP.name)
            {
                currentUnit.SetActive(true);
                return currentUnit;
            }
        }

        GameObject newUnit = (GameObject)Instantiate(unitP, transform.position, transform.rotation, binTransform);

        newUnit.name = unitP.name;

        newUnit.SetActive(true);

        unitPool.Add(newUnit);
       

        return newUnit;
    }

	public Transform GetBinTransform
	{
		get
		{
			return binTransform;
		}
	}


}
