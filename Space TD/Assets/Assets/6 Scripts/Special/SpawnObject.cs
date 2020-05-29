using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

    public GameObject[] prefabs;
    public float timeBetweenSpawn = 1;
    public Transform positionA;
    public Transform positionB;
    public float sizeA;
    public float sizeB;
    //Does it need to choose a prefab randomly ?
    public bool randomPrefabs;

    //Does it need to generate a random position for the prefabs ?
    public bool randomPos;
    public bool randomSize;

    float positionX;
    float positionY;

	void Start () {
        InvokeRepeating("SpawnPrefabFunc", 0f, timeBetweenSpawn);
    }
    
    void Update () {
        
	}
        
    void SpawnPrefabFunc()
    {
        if (positionA == null && positionB == null)
            return;
        if (PlayerStatsScript.instance != null && PlayerStatsScript.instance.IsGamePaused)
            return;

        GameObject newPrefab = null;

        if (randomPrefabs)
        {
            int i = Random.Range(0, prefabs.Length);
            newPrefab = PoolObject.instance.GetPoolObject(prefabs[i]);
        }

        else
        {
            foreach (GameObject currentPrefabs in prefabs)
                newPrefab = PoolObject.instance.GetPoolObject(currentPrefabs);
        }

        RandomPosition(newPrefab);
        RandomSize(newPrefab);
    
    }


    void RandomSize(GameObject newPrefabs)
    {
        if (randomSize)
        {
            float newRandomSize = Random.Range(sizeA, sizeB);
            newPrefabs.transform.localScale = new Vector2(newRandomSize, newRandomSize);
        }
    }

    void RandomPosition(GameObject newPrefabs)
    {
        if (randomPos)
        {
            positionX = Random.Range(positionA.position.x, positionB.position.x);
            positionY = Random.Range(positionA.position.y, positionB.position.y);

            newPrefabs.transform.position = new Vector2(positionX, positionY);



        }
        else
            newPrefabs.transform.position = new Vector2(positionA.position.x, positionA.position.y);


    }
}
