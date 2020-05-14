using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditPathPointText : MonoBehaviour
{
    public int index;

    private void Awake()
    {
        SetTextToIndex();
    }

    private void OnEnable()
    {
        SetTextToIndex();
    }

    private void OnDisable()
    {
        UpdateAllPathPointText();
    }

    private void SetTextToIndex()
    {
        index = GetNewIndex();
        if (index > 1)
        {
            TextMeshProUGUI textMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = index.ToString();
        }
    }

    private int GetNewIndex()
    {
        GameObject[] allPathPoints = GameObject.FindGameObjectsWithTag(gameObject.tag);
        return allPathPoints.Length;
    }

    private void UpdateAllPathPointText()
    {
        GameObject[] allPathPoints = GameObject.FindGameObjectsWithTag(gameObject.tag);
        int count = 1;
        foreach(GameObject pathPoint in allPathPoints)
        {
            if (pathPoint.GetInstanceID() == gameObject.GetInstanceID())
                continue;
            TextMeshProUGUI textMeshPro = pathPoint.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = count.ToString();
            pathPoint.GetComponent<EditPathPointText>().index = count;
            count++;
        }
    }

}
