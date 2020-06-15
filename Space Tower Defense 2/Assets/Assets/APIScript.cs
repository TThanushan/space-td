using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
public class APIScript : MonoBehaviour
{
    private const string URL = "WWW.google.ca";

    public void Request()
    {
        WWW request = new WWW(URL);
    }
}
