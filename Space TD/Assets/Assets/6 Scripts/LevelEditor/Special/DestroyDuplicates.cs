using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DestroyDuplicates : MonoBehaviour
{
    private void OnEnable()
    {
        DestroyAllDuplicates();

    }

    private void DestroyAllDuplicates()
    {
        GameObject[] duplicates = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (duplicates.Length > 1)
        {
            foreach (GameObject obj in duplicates)
            {
                if (obj.GetInstanceID() != gameObject.GetInstanceID())
                {
                    Destroy(obj);
                }
            }
        }
    }

}
