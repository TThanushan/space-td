using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class WavesEditorUI : MonoBehaviour
{
    public static WavesEditorUI instance;
    public TextMeshProUGUI numberOfWavesText;
    public TMP_Dropdown wavesDropdown;

    public GameObject enemyTypeBlockPrefab;
    public Transform enemyTypeListParent;

    private void Start()
    {
        UpdateNumberOfWavesText();
    }

    public void UpdateNumberOfWavesText()
    {
        numberOfWavesText.text = WavesEditor.instance.GetNumberOfWaves().ToString() + " waves";
    }

    public void UpdateWavesDropdown()
    {
        wavesDropdown.ClearOptions();
        wavesDropdown.AddOptions(GetWavesStringList());
    }

    private List<string> GetWavesStringList()
    {
        List<string> wavesNameList = new List<string>();
        for (int i = 0; i < WavesEditor.instance.GetNumberOfWaves(); i++)
        {
            wavesNameList.Add("Wave " + (i + 1).ToString());
        }
        return wavesNameList;
    }

    public void AddEnemyTypeBlock()
    {
        GameObject newBlock = (GameObject)Instantiate(enemyTypeBlockPrefab, enemyTypeListParent);
    }

    public GameObject GetNewEnemyTypeBlock()
    {
        GameObject newBlock = (GameObject)Instantiate(enemyTypeBlockPrefab, enemyTypeListParent);
        return newBlock;
    }

    public void CreateEnemyTypeBlock(Waves.EnemyType enemyType)
    {
        GameObject newBlock = GetNewEnemyTypeBlock();
        foreach (Transform child in newBlock.transform.Find("Infos"))
        {
            child.GetComponent<TMP_InputField>().text = GetEnemyTypeBlockPropeties(enemyType, child.name);
        }
    }

    private string GetEnemyTypeBlockPropeties(Waves.EnemyType enemyType, string name)
    {
        if (name == "Health")
            return enemyType.Health.ToString();
        else if (name == "Speed")
            return enemyType.MoveSpeed.ToString();
        else if (name == "Loot")
            return enemyType.MoneyRewardPerUnit.ToString();
        else if (name == "SpawnDelay")
            return enemyType.TimeBetweenSpawn.ToString();
        else if (name == "Count")
            return enemyType.EnemyCount.ToString();

        return enemyType.Health.ToString();
    }

    private GameObject GetEnemyTypeBlockInfoGameObject(GameObject block, string infoName)
    {
        return block.transform.Find("Infos/" + infoName).gameObject;
    }


}
