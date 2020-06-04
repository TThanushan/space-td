using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAScript : MonoBehaviour {



    public float moveSpeed;
    public int damage = 1;
    public Transform[] pathArray;
    private bool isSlowByTurret;
    //The index of the point to go.
    int pathPointIndex = 0;

    public float startMoveSpeed;

    PlayerStatsScript playerStats;

    void Start() {
        pathArray = PoolObject.instance.pathArray;
        InvokeRepeating("CancelSlowEffect", 0f, 1f);
    }

    void Awake()
    {
        startMoveSpeed = moveSpeed;
        playerStats = PlayerStatsScript.instance;
    }

    void OnEnable()
    {
        pathPointIndex = 0;
    }

    public float GetMoveSpeed
    {
        get
        {
            return startMoveSpeed;
        }
    }

    public int GetPathPointIndex { get { return pathPointIndex; } }

    public void Slow(float slowAmount)
    {
        moveSpeed = startMoveSpeed - (startMoveSpeed * slowAmount);
        EnableSlowEffect(true);
    }

    private void EnableSlowEffect(bool value)
    {
        transform.Find("Sprite/SlowEffect").gameObject.SetActive(value);
    }

    void FixedUpdate() {
        if (playerStats.IsGamePaused)
            return;
        if (ReachedPlayerBase())
        {
            SpawnerScript.instance.enemiesRemainingAlive--;
            Feedback();
            DoDamageToPlayer();
            Disable();
        }
        else if (ReachedPathPoint())
            MoveToNextPoint();
        Move();
    }

    void CancelSlowEffect()
    {
        moveSpeed = startMoveSpeed;
        EnableSlowEffect(false);
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

    void DoDamageToPlayer()
    {
        PlayerStatsScript.instance.life -= damage;
        SpawnerScript.instance.IncreasePlayerHealthLoss();
    }
    void DisplayDamageAmount()
    {
        UIScript.instance.DisplayText("-" + damage.ToString(), transform.position, 2, "Red");
    }
    void Feedback()
    {
        DisplayDamageAmount();
        AudioManager.instance.Play("Lose life", true);
        ShakeCamera.instance.Shake(0.2f, 0.2f);
    }
    void Disable()
    {
        gameObject.SetActive(false);
    }
}
