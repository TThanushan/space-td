using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float attackRange = 1;

    public float moveSpeed = 1;

    public float attackDamage = 1;

    public GameObject target;

	public GameObject effect;
    PlayerStatsScript playerStats;

	void Awake () {
        playerStats = PlayerStatsScript.instance;
	}
    void FixedUpdate()
    {
        if (playerStats.IsGamePaused)
            return;
        AttackTarget();
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (target != null)
        {
            Vector2 dir = target.transform.position - transform.position;
            transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    void AttackTarget()
    {
        Vector2 dir = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            if (target.GetComponent<ProgressBarScript>() == null)
            {
                gameObject.SetActive(false);
                return;
            }
            ProgressBarScript targetProgressBarScript = target.GetComponent<ProgressBarScript>();

            DamageTarget();

            if (targetProgressBarScript.currentHealth <= 0 && targetProgressBarScript.deathEffect)
                CreateTargetDeathEffect(targetProgressBarScript.deathEffect);

            if (effect)
            {
                GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
                newEffect.transform.position = transform.position;
                RotateObjAwayFrom(newEffect, target);

            }
            gameObject.SetActive(false);
        }
        if(target == null)
            gameObject.SetActive(false);
    }

    void DamageTarget()
    {
        ProgressBarScript targetProgressBarScript = target.GetComponent<ProgressBarScript>();
        targetProgressBarScript.currentHealth -= attackDamage;
    }

    void CreateTargetDeathEffect(GameObject deathEffect)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(deathEffect);
        RotateObjToward(effect, target);
        effect.transform.position = target.transform.position;
    }

    void RotateObjToward(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void RotateObjAwayFrom(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
