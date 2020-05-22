using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if (_current == null)
                _current = new SaveData();
            return _current;
        }
    }

    public Map map = new Map();

    public void Save()
    {
        if (!SaveConditionFullfilled())
            return;

        map.SaveAllObject();
        SerializationManager.Save(map.Name, current);
        if (LevelEditor.instance)
            LevelEditorUI.instance.ShowInfoText("'" + _current.map.Name + "' saved !");
    }

    public void Load(string _name)
    {
        _current = (SaveData)SerializationManager.Load(Application.dataPath + "/Assets/7 Others/Maps/" + _name + ".save");

        Debug.Log("<"+_current.map.Name+">");

        if (LevelEditor.instance)
            LevelEditorUI.instance.ShowInfoText("'" + _current.map.Name + "' loaded !");
        Debug.Log("'" + _current.map.Name + "' loaded !");
    }

    private bool SaveConditionFullfilled()
    {
        bool isFullfilled = true;
        string errorMessage = "";
        if (!GameObject.FindGameObjectWithTag("Base"))
        {
            isFullfilled = false;
            errorMessage = "You must place a Base";
        }
        else if (!GameObject.FindGameObjectWithTag("Spawn Point"))
        {
            isFullfilled = false;
            errorMessage = "You must place a Spawn Point";
        }
        else if (!LevelEditor.instance.IsLevelNameDefined())
        {
            isFullfilled = false;
            errorMessage = "Choose a name";
        }
        else if (!GameObject.FindGameObjectWithTag("Building Block"))
        {
            isFullfilled = false;
            errorMessage = "You must place at least one build block";
        }
        if (!isFullfilled && LevelEditor.instance)
            LevelEditorUI.instance.ShowInfoText(errorMessage);
        return isFullfilled;
    }

}
 