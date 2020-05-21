using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;
public class MapEditor : MonoBehaviour
{
    public static MapEditor instance;
    public GameObject selectedPrefab;
    public GameObject cursor;
    public GameObject prefabPreview;
    public string levelName;
    public TextMeshProUGUI levelNameTextMesh;
    public GameObject[] prefabs;
    public GameObject[] prefabPreviews;

    public bool placementEnable = true;

    private TMP_Dropdown levelSelectionDropdown;
    private Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            print("There is already an levelEditor Script");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!cursor)
            cursor = GameObject.Find("Cursor");
        GetlevelNameTextMeshComponent();

        if (!levelSelectionDropdown)
            levelSelectionDropdown = GetComponentInChildren<TMP_Dropdown>();
        UpdateLevelSelectionDropdown();
    }

    private void Update()
    {
        CopyingPrefabFromNode();
        ShowSelectedPrefabPreview();
    }

    public void SetDeletePanelText(TextMeshProUGUI textMeshProUGUI)
    {
        string levelName = 
        textMeshProUGUI.text = transform.parent.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    private void ShowSelectedPrefabPreview()
    {
        if (PrefabPreviewNeedsToBeDelete())
        {
            prefabPreview.SetActive(false);
            prefabPreview = null;
            return;
        }
        if (!prefabPreview && selectedPrefab)
        {
            prefabPreview = PoolObject.instance.GetPoolObject(GetPrefabPreview());
            prefabPreview.transform.parent = cursor.transform;
            prefabPreview.transform.position = cursor.transform.position;
        }
    }

    public TMP_Dropdown GetLevelSelectDropdown()
    {
        return levelSelectionDropdown;
    }

    public void ShowPrefabPreview(bool value)
    {
        if (prefabPreview)
            prefabPreview.SetActive(value);
    }
    private bool PrefabPreviewNeedsToBeDelete()
    {
        return (prefabPreview && !selectedPrefab ) || (prefabPreview && selectedPrefab) && prefabPreview.name != selectedPrefab.name + "Preview";
    }

    private GameObject GetPrefabPreview()
    {
        string _name = selectedPrefab.name+"Preview";
        foreach (GameObject obj in prefabPreviews)
        {
            if (obj.name == _name)
                return obj;
        }
        return null;
    }

    private void CopyingPrefabFromNode()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            MapEditorNode _node = GetCorrespondingNode(cursor.transform.position);
            if (_node)
            {
                if (_node.GetCurrentPrefab())
                    SetSelectedPrefabByName(_node.GetCurrentPrefab().name);
            }
        }
    }

    public void UpdateLevelSelectionDropdown()
    {
        levelSelectionDropdown.ClearOptions();
        levelSelectionDropdown.AddOptions(GetAllSavedFileNameWithEditTime());
    }

    public void HideShowLevelSelectDropdown()
    {
        StartCoroutine(RefreshLevelSelectDropdown(0.25f));
    }

    IEnumerator RefreshLevelSelectDropdown(float seconds=1f)
    {
        levelSelectionDropdown.Hide();
        yield return new WaitForSeconds(seconds);
        levelSelectionDropdown.Show();

    }

    private List<string> GetAllSavedFilesPath()
    {
        string savePath = SaveData.current.GetMapSavePath();
        string[] allFiles = Directory.GetFiles(savePath);
        List<string> filesList = new List<string>(allFiles);
        return filesList.FindAll(EndsWithSave);
    }

    private List<string> GetAllSavedFileNameWithEditTime()
    {
        return AddFilesLastWriteTime(GetAllSavedFileNames());
    }

    private List<string> GetAllSavedFileNames()
    {
        List<string> _list = GetAllSavedFilesPath();
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i] = GetOnlyFileName(_list[i]);
        }
        return _list;
    }

    private List<string> AddFilesLastWriteTime(List<string> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i] += " (" + GetLastWriteTime(_list[i]) + ")";
        }
        return _list;
    }

    private string GetOnlyFileName(string filePath)
    {
        filePath = filePath.Substring(SaveData.current.GetMapSavePath().Length);
        filePath = filePath.Substring(0, filePath.LastIndexOf(".save"));
        return filePath;
    }

    private string GetLastWriteTime(string fileName)
    {
        string filePath = SaveData.current.GetMapSavePath();
        filePath += fileName + ".save";
        return File.GetLastWriteTime(filePath).ToString("MM/dd/yyyy HH:mm:ss");
    }

    public MapEditorNode GetCorrespondingNode(Vector2 _position)
    {
        GameObject[] buildingNodes = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject node in buildingNodes)
        {
            if (node.transform.position.Equals(_position))
                return node.GetComponent<MapEditorNode>();
        }
        return null;
    }

    public bool IsLevelNameDefined()
    {
        return levelName != null && levelName != "" && levelName.Length > 1;
    }

    private bool EndsWithSave(string s)
    {
        return s.EndsWith(".save");
    }

    public void Save()
    {
        SaveData.current.SaveMap();
    }

    public void LoadMap()
    {
        string _name = GetLevelToLoadName();
        if (_name == null || _name == "")
            return;

        SaveData.current.LoadMap();
        SaveData.current.map.LoadAllObjectFromSaveData();
        levelName = SaveData.current.map.Name;
        SetNameFieldText(levelName);
        Debug.Log(_name + " loaded !");
    }

    public string GetLevelToLoadName()
    {
        string name = levelSelectionDropdown.captionText.text.ToString();
        if (name == null || name == "")
            return null;
        return RemoveEditTime(name);
    }

    private string RemoveEditTime(string name)
    {
        if (name.Length <= 19)
            return name;
        int editTimeLength = (System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")).Length + 3;
        return name.Substring(0, name.Length - editTimeLength);
    }


    private void SetNameFieldText(string _text)
    {
        transform.GetComponentInChildren<TMP_InputField>().text = _text;
    }

    public void GetlevelNameTextMeshComponent()
    {
        if (levelNameTextMesh)
            return;
        GameObject textObject = transform.Find("BuildToolbar/NameField/Text Area/Text").gameObject;
        levelNameTextMesh = textObject.GetComponent<TextMeshProUGUI>();
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    private void SetSelectedPrefabByName(string _name)
    {
        foreach (GameObject _prefab in prefabs)
        {
            if (_prefab.name == _name)
                SetSelectedPrefab(_prefab);
        }
    }

    public void ResetMap()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        MapEditorNode mapEditorNode;
        foreach (GameObject currentNode in nodes)
        {
            mapEditorNode = currentNode.GetComponent<MapEditorNode>();
            if (mapEditorNode.GetCurrentPrefab())
            {
                mapEditorNode.ClearCurrentPrefab();
                MapEditorUI.instance.CreateDeleteEffect(mapEditorNode.transform.position);
            }
        }
        Debug.Log("Map reset !");
    }

    public void DeleteSaveFile(string _name)
    {
        string savePath = SaveData.current.GetMapSavePath();

        if (!File.Exists(savePath + _name + ".save"))
            Debug.Log("File '" + _name + "' not found");
        else
        {
            Debug.Log("File '" + _name + " deleted !");
            File.Delete(savePath + _name + ".save");
        }
    }

    public void DeleteFromDropdownItem(TextMeshProUGUI _textMeshPro)
    {
        string fileName = _textMeshPro.text;
        DeleteSaveFile(RemoveEditTime(fileName));
        MapEditorUI.instance.ShowInfoText("'" + fileName + "' deleted !", Color.yellow);
    }

    public void SetName()
    {
        levelName = levelNameTextMesh.text;
    }

}
