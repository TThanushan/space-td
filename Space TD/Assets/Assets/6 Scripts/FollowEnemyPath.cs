using UnityEngine;
using System.Collections;

public class FollowEnemyPath : MonoBehaviour
{
    public float moveSpeed;

    private int pathPointIndex;
    private Transform[] pathArray;

    private Animator animator;

    private void Start()
    {
        pathArray = PoolObject.instance.pathArray;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (ReachedPlayerBase())
            gameObject.SetActive(false);
        else if (ReachedPathPoint())
            MoveToNextPoint();
        Move();
    }


    void Move()
    {
        Vector2 dir = GetMoveDirection();
        transform.Translate(dir.normalized * Time.deltaTime * moveSpeed);
    }

    public void SetPathPointIndex(int value)
    {
        pathPointIndex = value;
    }

    bool ReachedPathPoint()
    {
        Vector2 dir = GetMoveDirection();
        float distance = moveSpeed * Time.deltaTime;
        return dir.magnitude <= distance;
    }

    private Vector2 GetMoveDirection()
    {
        if (pathPointIndex >= pathArray.Length - 1)
            return PoolObject.instance._base.transform.position - transform.position;
        return pathArray[pathPointIndex].position - transform.position;
    }

    bool ReachedPlayerBase()
    {
        return ReachedPathPoint() && pathPointIndex >= pathArray.Length - 1;
    }

    void MoveToNextPoint()
    {
        pathPointIndex++;
    }

}
