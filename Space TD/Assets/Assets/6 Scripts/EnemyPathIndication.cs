using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPathIndication : MonoBehaviour
{
    public GameObject indicationPrefab;
    public float timeBetweenSpawn;

    private float nextSpawnTime;
    private GameObject[] pathArray;
    private PoolObject poolObject;
    private Vector2 spawnPosition;
    private bool activated = true;
    private List<GameObject> allIndications;


    private void Start()
    {
        allIndications = new List<GameObject>();
        poolObject = PoolObject.instance;
        pathArray = GameObject.FindGameObjectsWithTag("Path Point");
        spawnPosition = GameObject.FindGameObjectWithTag("Spawn Point").transform.position;
        SpawnerScript.instance.OnWaveStart += DisablePathIndications;
        SpawnerScript.instance.OnWaveOver += EnablePathIndication;
        InitPathIndication();
    }

    private void Update()
    {
        InstantiateIndicationPrefab();
    }

    private void DisablePathIndications()
    {
        activated = false;
        foreach (GameObject current in allIndications)
        {
            StartCoroutine(DisableAfterAnimation(current));
        }
    }

    private void EnablePathIndication()
    {
        activated = true;
        foreach (GameObject current in allIndications)
        {
            current.SetActive(true);
            current.GetComponent<Animator>().Play("Show");
        }
    }

    IEnumerator DisableAfterAnimation(GameObject _gameObject)
    {
        _gameObject.GetComponent<Animator>().Play("Hide");
        yield return new WaitForSeconds(0.6f);
        _gameObject.SetActive(false);
    }

    private void InitPathIndication()
    {
        Vector2 spawnerPosition = GameObject.FindGameObjectWithTag("Spawn Point").transform.position;
        Vector2 firstPathPoint = pathArray[0].transform.position;

        CreatePrefabBetweenTwoPoints(spawnerPosition, firstPathPoint, 0);
        CreatePrefabOnPathArray();
    }

    private void CreatePrefabOnPathArray()
    {
        for (int i = 0; i < pathArray.Length; i++)
        {
            if (i >= pathArray.Length - 1)
                break;
            CreatePrefabBetweenTwoPoints(pathArray[i].transform.position, pathArray[i + 1].transform.position, i + 1);
        }
    }

    private void CreatePrefabBetweenTwoPoints(Vector2 firstPos, Vector2 secondPos, int index)
    {
        float distance = Vector2.Distance(firstPos, secondPos);
        for (float y = 0; y < 1; y = y + 1 * (timeBetweenSpawn / distance))
        {
            Vector2 newPosition = Vector2.Lerp(firstPos, secondPos, y);
            GameObject newPrefab = CreatePrefab(newPosition);
            SetPrefabPathPointIndex(newPrefab, index);
        }
    }

    private GameObject CreatePrefab(Vector2 position)
    {
        GameObject newPrefab = poolObject.GetPoolObject(indicationPrefab);
        newPrefab.transform.position = position;
        allIndications.Add(newPrefab);
        return newPrefab;
    }

    private void SetPrefabPathPointIndex(GameObject prefab, int index)
    {
        prefab.GetComponent<FollowEnemyPath>().SetPathPointIndex(index);
    }

    private void InstantiateIndicationPrefab()
    {
        if (!activated)
            return;
        if (nextSpawnTime <= Time.time)
        {
            nextSpawnTime = Time.time + timeBetweenSpawn;
            CreatePrefab(spawnPosition).GetComponent<FollowEnemyPath>().SetPathPointIndex(0);
        }
    }

}
