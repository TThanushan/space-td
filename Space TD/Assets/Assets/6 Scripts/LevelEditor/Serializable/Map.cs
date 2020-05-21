using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
    public string Name;

    public List<Vector2> BuildingBlocks;
    public List<Vector2> PathPoints;
    public List<Vector2> PathGrounds;

    public Vector2 SpawnPoint;
    public Vector2 Base;

    public void SaveAllObject()
    {
        Name = MapEditor.instance.levelName;
        SaveObjectIntoList(ref BuildingBlocks, "Building Block");
        SaveObjectIntoList(ref PathPoints, "Path Point");
        SaveObjectIntoList(ref PathGrounds, "Path Ground");
        SpawnPoint = SaveObjectPosition("Spawn Point");
        Base = SaveObjectPosition("Base");
        
    }

    public void LoadAllObjectFromSaveData()
    {
        CreateObjectFromSave(BuildingBlocks, "Building Block");
        CreateObjectFromSave(PathPoints, "Path Point");
        CreateObjectFromSave(PathGrounds, "Path Ground");
        CreateObjectFromSave(SpawnPoint, "Spawn Point");
        CreateObjectFromSave(Base, "Base");
    }

    private void CreateObjectFromSave(Vector2 _position, string name)
    {
        GameObject newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
        newObj.transform.position = _position;
        newObj.SetActive(true);
        MapEditorNode mapEditorNode = MapEditor.instance.GetCorrespondingNode(_position);
        if (mapEditorNode)
            mapEditorNode.SetCurrentPrefab(newObj);

    }

    private void CreateObjectFromSave(List<Vector2> _list, string name)
    {
        GameObject newObj;

        foreach (Vector2 _pos in _list)
        {
             newObj = PoolObject.instance.GetPoolObject(GetCorrespondingPrefab(name));
            newObj.transform.position = _pos;
            MapEditor.instance.GetCorrespondingNode(_pos).SetCurrentPrefab(newObj);
        }
    }

    private GameObject GetCorrespondingPrefab(string name)
    {
        foreach (GameObject obj in MapEditor.instance.prefabs)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }

    private Vector2 SaveObjectPosition(string _tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(_tag);
        return obj.transform.position;
    }

    public void SaveObjectIntoList(ref List<Vector2> _list, string _tag)
    {
        GameObject[] objectsToAdd = GameObject.FindGameObjectsWithTag(_tag);
        _list = new List<Vector2>();
        if (objectsToAdd.Length <= 0)
            return;
        foreach (GameObject block in objectsToAdd)
        {
            _list.Add(block.transform.position);
        }
    }

}
