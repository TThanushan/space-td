using UnityEngine;
using System.Collections;

public class SpawnObjectOnDisable : MonoBehaviour
{
    public GameObject prefab;
    [Range(0f, 2f)]
    public float randomXPosition;
    [Range(0f, 2f)]
    public float randomYPosition;

    private void OnDisable()
    {
        Vector2 newPosition = transform.position;
        if (randomXPosition != 0f || randomYPosition != 0f)
            newPosition = RandomPosition(); 
        Instantiate(prefab, newPosition, prefab.transform.rotation, PoolObject.instance.GetBinTransform);
    }

    private Vector2 RandomPosition()
    {
        Vector2 randomPosition = transform.position;
        randomPosition.x += Random.Range(-randomXPosition, randomXPosition);
        randomPosition.y += Random.Range(-randomYPosition, randomYPosition);
        return randomPosition;
    }
}
