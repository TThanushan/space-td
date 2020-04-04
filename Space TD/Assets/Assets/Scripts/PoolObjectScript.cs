using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjectScript : MonoBehaviour {

    public static PoolObjectScript instance;

    [Header("Data")]
    public GameObject[] enemies;
    public GameObject endNode;

    public string enemyTag;

    [Space(30)]

    [Space]
    //Size to start with.
    public int poolSize;


    public List<GameObject> unitPool;


    Transform binTransform;



    void Start () {
        
        binTransform = GameObject.FindGameObjectWithTag("Bin").GetComponent<Transform>();
        endNode = GameObject.FindGameObjectWithTag("End");        

    }
    
    void Update () {

        FindEnemyFunc();
        
    }

    void FindEnemyFunc()
    {
        
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public GameObject GetPoolObject(GameObject unitP)
    {
//        print(unitP.name);
        foreach (GameObject currentUnit in unitPool)
        {
            if (currentUnit.activeInHierarchy == false && currentUnit.name == unitP.name)
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

    void Awake()
    {
        if (instance != null)
            return;
        
        instance = this;
    }
}
