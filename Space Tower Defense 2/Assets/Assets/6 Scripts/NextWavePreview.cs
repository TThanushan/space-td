using UnityEngine;
using System.Collections;
using TMPro;

public class NextWavePreview : MonoBehaviour
{
    public GameObject panel;
    public GameObject bar;
    public GameObject previewBlockPrefab;
    public static NextWavePreview instance;

    private void Start()
    {
        CreatePreview();
    }


    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    public void ShowPreview()
    {
        ClearPreviewBlocks();
        CreatePreview();
        GetComponent<Animator>().Play("Show");
    }

    public void HidePreview()
    {
        GetComponent<Animator>().Play("Hide");
    }

    private void CreatePreview()
    {
        Waves.EnemyType[] enemyTypes = SpawnerScript.instance.GetNextWaveEnemyType();
        foreach (Waves.EnemyType enemyType in enemyTypes)
        {
            GameObject previewBlock = GetNewPreviewBlock();
            previewBlock.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = 'x' + enemyType.currentEnemyCount.ToString();
            SetHealthText(previewBlock, enemyType);
            GameObject sprite =  Instantiate(enemyType.Enemy.transform.Find("Sprite").gameObject, previewBlock.transform);
            sprite.transform.localScale = new Vector2(40, 40);
        }
    }

    private void SetHealthText(GameObject previewBlock, Waves.EnemyType enemyType)
    {
        previewBlock.transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = SpawnerScript.instance.GetUpdatedEnemyHealth(enemyType).ToString();
    }

    private void ClearPreviewBlocks()
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject GetNewPreviewBlock()
    {
        return Instantiate(previewBlockPrefab, panel.transform);
    }
}
