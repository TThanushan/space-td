using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserBeam : MonoBehaviour
{
    public float damage;
    public float moveSpeed;
    public float intensity = 1f;


    private Transform[] pathArray;
    private int pathPointIndex;
    Transform spawnPoint;

    List<GameObject> enemyDamaged;
    List<float> nextAttackTime;

    private void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn Point").transform;
        pathArray = PoolObject.instance.pathArray;
        pathPointIndex = pathArray.Length - 1;
        enemyDamaged = new List<GameObject>();
        nextAttackTime = new List<float>();
    }

    private void Update()
    {
        if (PlayerStatsScript.instance.IsGamePaused || DisableIfNoEnemies())
            return;
        if (ReachedSpawnPoint())
            Disable();
        else if (ReachedPathPoint())
            MoveToNextPoint();
        Move();
        ShakeCamera.instance.Shake(intensity, 0.1f);
        DamageEnemies();
    }

    private void DamageEnemies()
    {
        foreach (GameObject enemy in PoolObject.instance.enemies)
        {
            if (enemyDamaged.Contains(enemy) && nextAttackTime[enemyDamaged.IndexOf(enemy)] > Time.time)
                continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < 1f)
            {

                AudioManager.instance.Play("Boom");
                enemyDamaged.Add(enemy);
                enemy.GetComponent<ProgressBarScript>().GetDamage(damage);
                nextAttackTime.Add(1f + Time.time);
            }
        }
    }

    private bool DisableIfNoEnemies()
    {
        if (!SpawnerScript.instance.WaveIsInProgress() && gameObject.activeSelf)
        {
            Disable();
            return true;
        }
        return false;
    }

    IEnumerator DisableAfterAnimation()
    {
        gameObject.GetComponent<Animator>().Play("Hide");
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }

    private void Disable()
    {
        StartCoroutine(DisableAfterAnimation());
        enemyDamaged.Clear();
    }

    private void OnEnable()
    {
        if (pathArray == null)
            return;
        gameObject.GetComponent<Animator>().Play("Show");
        pathPointIndex = pathArray.Length - 1;
        transform.position = PoolObject.instance._base.transform.position;
    }


    void Move()
    {
        Vector2 dir = GetMoveDirection();
        transform.Translate(dir.normalized * Time.deltaTime * moveSpeed);
    }

    bool ReachedPathPoint()
    {
        Vector2 dir = GetMoveDirection();
        float distance = moveSpeed * Time.deltaTime;
        return dir.magnitude <= distance;
    }

    private Vector2 GetMoveDirection()
    {
        if (pathPointIndex < 0)
            return spawnPoint.position - transform.position;
        return pathArray[pathPointIndex].position - transform.position;
    }

    bool ReachedSpawnPoint()
    {
        return ReachedPathPoint() && pathPointIndex < 0;
    }

    void MoveToNextPoint()
    {
        pathPointIndex--;
    }

}
