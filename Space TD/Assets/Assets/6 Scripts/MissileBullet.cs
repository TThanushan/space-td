using UnityEngine;
using System.Collections;

public class MissileBullet : BulletScript
{
    [SerializeField]
    private float explosionRange;
    [SerializeField]
    private float explosionDamage;

    public float ExplosionRange { get => explosionRange; set => explosionRange = value; }
    public float ExplosionDamage { get => explosionDamage; set => explosionDamage = value; }

    public static System.Action<int> enemyKilled;

    protected override void AttackTarget()
    {
        if (target && IsTargetInRange())
        {
            DamageEnemiesAround();
            DestroyEffect();
            ExplosionSFX();
            target = null;
            gameObject.SetActive(false);
        }
    }

    private void DamageEnemiesAround()
    {
        int i = 0;
        float totalDamageDealt = 0f;
        foreach (GameObject enemy in PoolObject.instance.enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= ExplosionRange)
            {
                totalDamageDealt += ComputeDamageDealt(enemy.GetComponent<ProgressBarScript>());

                EnemyDestroyEffect(enemy);
                DamageEnemy(enemy);
                if (enemy.GetComponent<ProgressBarScript>().IsKilled(explosionDamage))
                   i++;

            }
        }
        if (i > 0)
            enemyKilled?.Invoke(i);
        damageEvent?.Invoke(totalDamageDealt);
    }

    private float ComputeDamageDealt(ProgressBarScript progressBarScript)
    {
        float damageDealt = explosionDamage;
        float enemyHealth = progressBarScript.currentHealth - damageDealt;
        if (enemyHealth < 0)
            damageDealt += enemyHealth;
        return damageDealt;
    }

    private void EnemyDestroyEffect(GameObject enemy)
    {
        ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();
        if (progressBarScript.IsKilled((int)ExplosionDamage) && progressBarScript.deathEffect)
            CreateTargetDeathEffect(progressBarScript.deathEffect);
    }

    private void DamageEnemy(GameObject enemy)
    {
        ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();
        progressBarScript.GetDamage(explosionDamage);
    }

    private void ExplosionSFX()
    {
        AudioManager.instance.PlaySfx("Boom");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
