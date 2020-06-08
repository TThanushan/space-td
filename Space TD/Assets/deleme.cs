using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class deleme : MonoBehaviour
{

    public Gradient gradient;
    [Range(0, 1)]
    public float value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = gradient.Evaluate(value);
    }
}
