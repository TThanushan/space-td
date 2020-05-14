using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightColor : MonoBehaviour
{
    public Color highlightColor;
    public float colorLerpSpeed;


    private Color startColor;
    private SpriteRenderer sprite;
    private bool lerpActivated = false;
    private float startTime;
    private float t;

    private void Awake()
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            sprite = GetComponent<SpriteRenderer>();
            startColor = sprite.color;
            lerpActivated = false;
        }
    }

    void Update()
    {
        ColorLerp();
    }

    void ColorLerp()
    {
        t = (Time.time - startTime) * colorLerpSpeed;
        if (lerpActivated)
            sprite.color = Color.Lerp(startColor, highlightColor, t);
        else
            sprite.color = Color.Lerp(highlightColor, startColor, t);
    }

    void StartColorFade()
    {
        lerpActivated = true;
        startTime = Time.time;
    }

    void StopColorFade()
    {
        lerpActivated = false;
    }

    private void OnMouseEnter()
    {
        StartColorFade();
    }

    private void OnMouseExit()
    {
        StopColorFade();
    }
}
