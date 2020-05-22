using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;
public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance;
    public GameObject selectedPrefab;
    public GameObject cursor;
    public GameObject prefabPreview;
    public string levelName;
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
            LevelEditorNode _node = GetCorrespondingNode(cursor.transform.position);
            if (_node)
            {
                if (_node.GetCurrentPrefab())
                    SetSelectedPrefabByName(_node.GetCurrentPrefab().name);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateLevelSelectionDropdown();
    }

    public void UpdateLevelSelectionDropdown()
    {
        levelSelectionDropdown.ClearOptions();
        levelSelectionDropdown.AddOptions(GetAllSavedFileNames());

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
        string savePath = Application.dataPath + "/Assets/7 Others/Maps/";
        string[] allFiles = Directory.GetFiles(savePath);
        List<string> filesList = new List<string>(allFiles);
        return filesList.FindAll(EndsWithSave);
    }

    private List<string> GetAllSavedFileNames()
    {
        string savePath = Application.dataPath + "/Assets/7 Others/Maps/";
        List<string> _list = GetAllSavedFilesPath();
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i] = _list[i].Substring(savePath.Length);
            _list[i] = _list[i].Substring(0, _list[i].LastIndexOf(".save"));
        }
        return _list;
    }

    public LevelEditorNode GetCorrespondingNode(Vector2 _position)
    {
        GameObject[] buildingNodes = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject node in buildingNodes)
        {
            if (node.transform.position.Equals(_position))
                return node.GetComponent<LevelEditorNode>();
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
        SaveData.current.Save();
    }

    public void LoadMap()
    {
        string _name = levelSelectionDropdown.options[levelSelectionDropdown.value].text;
        if (_name == "")
            return;
        Debug.Log(_name.Length);
        SaveData.current.Load(_name);
        SaveData.current.map.LoadAllObjectFromSaveData();
        levelName = SaveData.current.map.Name;
        SetNameFieldText(levelName.ToString());
    }

    private void SetNameFieldText(string _text)
    {
        transform.GetComponentInChildren<TMP_InputField>().text = _text;
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
        LevelEditorNode levelEditorNode;
        foreach (GameObject currentNode in nodes)
        {
            levelEditorNode = currentNode.GetComponent<LevelEditorNode>();
            if (levelEditorNode.GetCurrentPrefab())
            {
                levelEditorNode.ClearCurrentPrefab();
                LevelEditorUI.instance.CreateDeleteEffect(levelEditorNode.transform.position);
            }
        }
        Debug.Log("Map reset !");
    }

    public void DeleteSaveFile(string _name)
    {
        string savePath = Application.dataPath + "/Assets/7 Others/Maps/";

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
        DeleteSaveFile(fileName);
        LevelEditorUI.instance.ShowInfoText("'" + fileName + "' deleted !", Color.yellow);
    }

    public void SetName()
    {
        levelName = GetComponentInChildren<TMP_InputField>().text;
    }

}
