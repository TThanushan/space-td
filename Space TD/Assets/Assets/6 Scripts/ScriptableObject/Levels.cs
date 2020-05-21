using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Create new Levels")]
public class Levels : ScriptableObject
{
    public Level[] levelArray;

    [System.Serializable]
    public class Level
    {
        public string Name;
        public int SceneIndex;
        public Sprite Screenshot;
        public bool IsLocked = true;
        public Waves waves;
        public Map map;
    }
}
