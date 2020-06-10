using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    private float attackRange = 1f;

    public float moveSpeed = 1;

    public float attackDamage = 1;

    public GameObject target;

	public GameObject effect;
    PlayerStatsScript playerStats;

    //public delegate void DamageEvent(float damage);
    //public static event DamageEvent DamageDealt;
    public static System.Action<float> damageEvent;

    void Awake ()   {
        playerStats = PlayerStatsScript.instance;
	}
    void FixedUpdate()
    {
        if (playerStats.IsGamePaused)
            return;
        AttackTarget();
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (target != null)
        {
            Vector2 dir = target.transform.position - transform.position;
            transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    protected virtual void AttackTarget()
    {
        if (target == null)
            gameObject.SetActive(false);
        if(IsTargetInRange())
        {
            TargetDestroyEffect();
            DamageTarget();
            DestroyEffect();
            target = null;
            gameObject.SetActive(false);
        }
    }

    private void TargetDestroyEffect()
    {
        ProgressBarScript targetProgressBarScript = target.GetComponent<ProgressBarScript>();
        if (targetProgressBarScript.IsKilled((int)attackDamage) && targetProgressBarScript.deathEffect)
            CreateTargetDeathEffect(targetProgressBarScript.deathEffect);
    }

    protected void DestroyEffect()
    {
        if (effect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
            newEffect.transform.position = transform.position;
            RotateObjAwayFrom(newEffect, target);
        }
    }

    protected bool IsTargetInRange()
    {
        Vector2 dir = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        return dir.magnitude <= distanceThisFrame;
    }

    private void DamageTarget()
    {
        OnDamageDealt(attackDamage);
        target.GetComponent<ProgressBarScript>().GetDamage(attackDamage);
    }

    public virtual void OnDamageDealt(float damage)
    {
        damageEvent?.Invoke(damage);
    }

    protected void CreateTargetDeathEffect(GameObject deathEffect)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(deathEffect);
        RotateObjToward(effect, target);
        effect.transform.position = target.transform.position;
    }

    private void RotateObjToward(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void RotateObjAwayFrom(GameObject obj, GameObject _target)
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
