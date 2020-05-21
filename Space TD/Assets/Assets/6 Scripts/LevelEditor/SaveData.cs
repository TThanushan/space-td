using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

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
    public Waves waves = new Waves();
    public Level level = new Level();

    public void Save()
    {

        //MapEditorUI.instance.ShowInfoText("'" + _current.map.Name + "' saved !");
    }

    public void Load()
    {
        //MapEditorUI.instance.ShowInfoText("'" + _current.map.Name + "' loaded !");
    }

    public void SaveMap()
    {
        if (!SaveMapConditionFullfilled())
            return;
        map.SaveAllObject();
        SerializationManager.Save(map.Name, current, GetMapSavePath());
    }

    public void LoadMap()
    {
        string name = MapEditor.instance.GetLevelToLoadName();
        _current = (SaveData)SerializationManager.Load(GetMapSavePath() + name + ".save");
    }


    public string GetMapSavePath()
    {
        return Application.dataPath + "/Assets/Resources/Maps/";
    }

    private bool SaveMapConditionFullfilled()
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
        else if (!MapEditor.instance.IsLevelNameDefined())
        {
            isFullfilled = false;
            errorMessage = "Choose a name";
        }
        else if (!GameObject.FindGameObjectWithTag("Building Block"))
        {
            isFullfilled = false;
            errorMessage = "You must place at least one build block";
        }
        else if (!ContainsOnlyLettersNumbersUnderscoreAndSpace(MapEditor.instance.levelName))
        {
            isFullfilled = false;
            errorMessage = "Level name may contain only digits, and upper and lower letters";
        }
        if (!isFullfilled)
            MapEditorUI.instance.ShowInfoText(errorMessage);
        return isFullfilled;
    }

    private bool ContainsOnlyLettersNumbersUnderscoreAndSpace(string str)
    {
        for (int i = 0; i < str.Length - 1; i++)
        {
            if (!char.IsLetterOrDigit(str[i]) && str[i] != '_' && !char.IsWhiteSpace(str[i]))
                return false;
        }
        return true;
    }

}
 