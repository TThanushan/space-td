using UnityEngine;
using System.Collections;

public class PlaySFXOnStart : MonoBehaviour
{
    public string sFXName;
    void Start()
    {
        AudioManager.instance.PlaySfx(sFXName);
    }
}
