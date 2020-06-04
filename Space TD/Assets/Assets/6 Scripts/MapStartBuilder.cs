using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapStartBuilder : MonoBehaviour
{
    public GameObject[] prefabs;
    public int startMoney = 100;
    public int startLife = 50;

    public void Awake()
    {
        LoadMap();
        PlayerStatsScript.instance.money = startMoney;
        PlayerStatsScript.instance.life = startLife;
    }

    public void LoadMap()
    {
        LoadMapData();
        Map map = SaveData.current.map;
        
        CreateObjectFromSave(map.BuildingBlocks, "Node", "Nodes");
        CreateObjectFromSave(map.PathPoints, "Path Point", "Path Points");
        CreateObjectFromSave(map.PathGrounds, "Path Ground", "Path Grounds");
        CreateObjectFromSave(map.SpawnPoint, "Spawn Point");
        CreateObjectFromSave(map.Base, "Base");
    }

    private void LoadMapData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SaveData.current.Load(sceneName);
    }

    private void CreateObjectFromSave(Vector2 _position, string name)
    {
        GameObject newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
        newObj.transform.position = _position;
        Transform parent = GameObject.FindGameObjectWithTag("Bin").transform;
        newObj.transform.parent = parent;
    }

    private GameObject InstantiateParent(string name)
    {
        GameObject objModel = new GameObject(name);
        GameObject parent = Instantiate(objModel, GameObject.FindGameObjectWithTag("Bin").transform);
        parent.name = name;
        Destroy(objModel);
        return parent;
    }

    private void CreateObjectFromSave(List<Vector2> _list, string name, string parentName)
    {
        GameObject newObj;
        GameObject parent = InstantiateParent(parentName);
        foreach (Vector2 _pos in _list)
        {
            newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
            newObj.transform.position = _pos;
            newObj.transform.parent = parent.transform;
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
