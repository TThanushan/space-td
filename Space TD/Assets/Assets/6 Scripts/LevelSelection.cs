using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor.Events;

public class LevelSelection : MonoBehaviour
{
    public Levels levels;
    public GameObject buttonPrefab;
    public int startIndex;
    public void Start()
    {
        UpdateLevelArrayFromLevelsScriptableObject();
        GenerateLevelButton();
    }

    public void UpdateLevelArrayFromLevelsScriptableObject()
    {
        foreach (Levels.Level level in levels.levelArray)
        {
            level.waves = (Waves)Resources.Load("ScriptableObject/Waves/" + level.Name);
        }
    }

    public void GenerateLevelButton()
    {
        Transform panelParent = transform.Find("MainPanel/ScrollView/ScrollViewport/Levels Panel");
        foreach (Levels.Level level in levels.levelArray)
        {
            GameObject newButton = InstantiateNewButton(level, panelParent);
            if (!level.IsLocked)
                AddLoadSceneFunctionToOnclickEvent(ref newButton, level.SceneIndex);
            AddSFXMethodToOnClickEvent(ref newButton);
        }
    }

    private GameObject InstantiateNewButton(Levels.Level level, Transform panelParent)
    {
        GameObject newButton = (GameObject)Instantiate(buttonPrefab, transform.position, transform.rotation, panelParent);
        SetNewButtonProperties(ref newButton, level);
        return newButton;
    }

    private void SetNewButtonProperties(ref GameObject newButton, Levels.Level level)
    {
        newButton.transform.Find("Image").GetComponent<Image>().sprite = level.Screenshot;
        newButton.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = level.Name;
        newButton.transform.Find("LockImage").gameObject.SetActive(level.IsLocked);
    }

    private void AddLoadSceneFunctionToOnclickEvent(ref GameObject button, int index)
    {
        Button buttonScript = button.GetComponent<Button>();
        UnityAction<int> functionToAdd = new UnityAction<int>(GetComponent<MenuScript>().LoadScene);
        UnityEventTools.AddIntPersistentListener(buttonScript.onClick, functionToAdd, index + startIndex - 1);
    }

    private void AddSFXMethodToOnClickEvent(ref GameObject button)
    {
        Button buttonScript = button.GetComponent<Button>();
        UnityAction playSFXMethod = System.Delegate.CreateDelegate(typeof(UnityAction), AudioManager.instance, "PlaySfx") as UnityAction;
        UnityEventTools.AddPersistentListener(buttonScript.onClick, playSFXMethod);
    }

}
