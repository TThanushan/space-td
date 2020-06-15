using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNodesColor : MonoBehaviour
{
    [Range(0f, 1f)]
    public float hueMin;
    [Range(0f, 1f)]
    public float hueMax;
    [Range(0f, 1f)]
    public float saturationMin;
    [Range(0f, 1f)]
    public float saturationMax;
    [Range(0f, 1f)]
    public float valueMin;
    [Range(0f, 1f)]
    public float valueMax;
    [Range(0f, 1f)]
    public float alphaMin;
    [Range(0f, 1f)]
    public float alphaMax;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SetRandomColor();
    }

    private void SetRandomColor()
    {
        Color randomColor = Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, alphaMin, alphaMax);
        //GetComponent<SpriteRenderer>().color = new Color(randomColor.r, randomColor.g, randomColor.b);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
        {
            obj.transform.Find("Border").GetComponent<SpriteRenderer>().color = randomColor;
        }
    }
}
