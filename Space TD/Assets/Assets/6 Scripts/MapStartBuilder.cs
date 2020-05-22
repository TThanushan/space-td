using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapStartBuilder : MonoBehaviour
{
    public GameObject[] prefabs;

    public void Awake()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        LoadMapData();
        Map map = SaveData.current.map;
        
        CreateObjectFromSave(map.BuildingBlocks, "Node");
        CreateObjectFromSave(map.PathPoints, "Path Point");
        CreateObjectFromSave(map.PathGrounds, "Path Ground");
        CreateObjectFromSave(map.SpawnPoint, "Spawn Point");
        CreateObjectFromSave(map.Base, "Base");
    }

    private void LoadMapData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SaveData.current.Load(sceneName);
    }

    private void CreateObjectFromSave(Vector2 _position, string name, string parentTag="Bin")
    {
        GameObject newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
        newObj.transform.position = _position;

        Transform parent = GameObject.FindGameObjectWithTag(parentTag).transform;
        newObj.transform.parent = parent;
    }

    private void CreateObjectFromSave(List<Vector2> _list, string name, string parentTag = "Bin")
    {
        GameObject newObj;
        Transform parent = GameObject.FindGameObjectWithTag(parentTag).transform;
        foreach (Vector2 _pos in _list)
        {
            newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
            newObj.transform.position = _pos;
            newObj.transform.parent = parent;
        }
    }

    private GameObject GetCorrespondingPrefab(string name)
    {
        foreach (GameObject obj in prefabs)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }
}
