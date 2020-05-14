using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    public static PoolObject instance;

    [Header("Data")]
    public GameObject[] enemies;
    public Transform[] pathArray;
    public string enemyTag;
    [Space(30)]
    [Space]
    //Size to start with.
    public int poolSize;
    public List<GameObject> unitPool;
    Transform binTransform;

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    void Start () {

        GameObject _binTransform = GameObject.FindGameObjectWithTag("Bin");
        if (_binTransform)
            binTransform = _binTransform.GetComponent<Transform>();

        GameObject _pathArray = GameObject.FindGameObjectWithTag("PathPoints");
        if (_pathArray)
            pathArray = _pathArray.GetComponent<PathPointsScript>().points;
    }

    void FixedUpdate () {
        FindEnemyFunc();
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
