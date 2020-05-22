using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAScript : MonoBehaviour {



    public float moveSpeed;
    public int damage = 1;
    public Transform[] pathArray;
    bool isSlowByTurret;
    bool isBurnByTurret;
    //The index of the point to go.
    int pathPointIndex = 0;

    public float startMoveSpeed;

    PlayerStatsScript playerStats;

    void Start() {
        pathArray = PoolObject.instance.pathArray;
        InvokeRepeating("RemoveEffect", 0f, 2f);
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
        if (!isSlowByTurret)
            CheckEffect("SlowEffect", isSlowByTurret, true);
    }

    void RemoveEffect()
    {
        if (isSlowByTurret)
            CheckEffect("SlowEffect", isSlowByTurret, false);
    }

    void CheckEffect(string name, bool isEffect, bool newValue)
    {
        isSlowByTurret = newValue;
        GameObject slowEffect = transform.Find("Sprite/" + name).gameObject;
        if (!slowEffect)
        {
            GameObject slowRessource = (GameObject)Resources.Load("Effects/" + name);
            slowEffect = (GameObject)Instantiate(slowRessource, transform.position, transform.rotation, transform.Find("Sprite"));
        }
        transform.Find("Sprite/" + name).gameObject.SetActive(newValue);
    }

    void FixedUpdate() {
        if (playerStats.IsGamePaused)
            return;
        if (ReachedPlayerBase())
        {
            Feedback();
            DoDamageToPlayer();
            Disable();
        }
        else if (ReachedPathPoint())
            MoveToNextPoint();
        Move();
        CancelSlowEffect();
    }

    void CancelSlowEffect()
    {
        moveSpeed = startMoveSpeed;
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
