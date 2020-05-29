using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TowerScript : MonoBehaviour {

    public enum TowerTargetMod { singleTarget, multipleTarget, special }
    public enum TowerEffect { noEffect, slowTarget, BurnTarget, Electric, LaserBeam, ChargingTurret }

    public GameObject debugClosestE;

    [Header("Main Data")]
    public float attackDamage = 1;

    public float attackSpeed;

    public float attackRange;

    [Space]
    [Header("Others")]
    public float bulletSpeed = 1;

    [Range(0, 100)]
    public float slowAmount = 1;

    public float rotateSpeed;


    public GameObject shootPos;

    public GameObject bulletG;

    public List<GameObject> enemiesToAttack;

    public TowerTargetMod towerTargetMod = TowerTargetMod.singleTarget;

    public TowerEffect towerEffect = TowerEffect.noEffect;

    public bool isDisable = false;

    public string shootSFXName = "Tower shoot";

    public GameObject lightningEffect;

    Transform partToRotate;

    GameObject target;

    GameObject[] enemies;

    PoolObject poolScript;

    float nextAttackTime;

    GameObject[] newEnemie;


    [Header("Use Laser")]
    public LineRenderer lineRenderer;
    //Particle System

    public GameObject burnTurretEffectG;

    [Header("Charging Turret")]
    public float chargingSpeed;
    public float maxAttackspeed;
    public float minAttackspeed;
    private Image chargingBar;

    private new ParticleSystem particleSystem;

    LineRenderer targetLineRenderer;

    Animator animator;
    PlayerStatsScript playerStats;

    void Awake() {
        poolScript = GameObject.FindGameObjectWithTag("Data").GetComponent<PoolObject>();
        playerStats = PlayerStatsScript.instance;
    }

    void Start()
    {
        if (isDisable)
            return;
        //For burnTurret.

        if (burnTurretEffectG != null)
            particleSystem = burnTurretEffectG.GetComponent<ParticleSystem>();
        if (towerEffect == TowerEffect.Electric || towerEffect == TowerEffect.LaserBeam)
            lineRenderer = GetComponent<LineRenderer>();
        else
            targetLineRenderer = GetComponent<LineRenderer>();
        if (animator = transform.GetComponent<Animator>())
            animator.speed = (animator.speed / attackSpeed) / 3;

        partToRotate = transform.Find("Sprite").transform;

        InvokeRepeating("LookAtEnemy", 0f, 1 * Time.deltaTime);

    }


    void Update() {

        if (playerStats.IsGamePaused)
            return;
        if (isDisable)
            return;
        DrawLineToTarget();
        TowerManager();
    }


    void TowerManager()
    {
        if (towerEffect == TowerEffect.slowTarget)
            SlowEnemy();
        if (towerEffect == TowerEffect.BurnTarget)
            BurnEnemy();
        if (towerEffect == TowerEffect.Electric)
            ThrowLightning();
        if (towerEffect == TowerEffect.LaserBeam)
            LaserBeam();
        if (towerEffect == TowerEffect.ChargingTurret)
        {
            UpdateChargingBar();
            UnchargeAttackSpeed();
        }
        if (towerTargetMod == TowerTargetMod.singleTarget)
        {
            target = GetEnemyWithinRangeClosestToBase();
            LookAtEnemy();
            AttackEnemy();

            IsTheEnemyStillAlive();
        }
        else if (towerTargetMod == TowerTargetMod.multipleTarget)
        {
            AttackAllNearEnemy();
        }

    }

    void UpdateChargingBar()
    {
        if (!chargingBar)
            chargingBar = transform.Find("Sprite/ChargingBar/Canvas/Bar").GetComponent<Image>();
        float chargePercent = attackSpeed / maxAttackspeed;
        float fillAmount = 1 - chargePercent;
        if (chargePercent <= minAttackspeed)
            fillAmount = 1f;
        chargingBar.fillAmount = fillAmount;
    }

    void ChargeAttackSpeed()
    {
        if (target && attackSpeed > minAttackspeed)
            attackSpeed -= chargingSpeed;
        if (attackSpeed < minAttackspeed)
            attackSpeed = minAttackspeed;
    }

    void UnchargeAttackSpeed()
    {
        if (!target && attackSpeed < maxAttackspeed)
            attackSpeed += chargingSpeed / 4;
        if (attackSpeed > maxAttackspeed)
            attackSpeed = maxAttackspeed;
    }


    List<GameObject> FindAllNearEnemy()
    {
        enemies = poolScript.enemies;

        List<GameObject> enemiesToReturn = new List<GameObject>();

        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + attackSpeed;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(enemy.transform.position, transform.position);

                if (enemy.activeSelf == true)
                {

                    if (distance <= attackRange)
                        enemiesToReturn.Add(enemy);
                    else
                        enemiesToReturn.Remove(enemy);
                }
            }
        }
        else
            enemiesToReturn = null;

        return enemiesToReturn;


    }

    void AttackEnemy()
    {

        if (Time.time > nextAttackTime && target != null && target.activeSelf)
        {
            nextAttackTime = Time.time + attackSpeed;
            GameObject newBullet = poolScript.GetPoolObject(bulletG);
            BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();

            newBulletScript.target = target;
            newBulletScript.attackDamage = attackDamage;
            newBulletScript.moveSpeed = bulletSpeed;
            newBullet.transform.position = shootPos.transform.position;
            if (animator)
                animator.Play("Shoot");
            AudioManager.instance.Play(shootSFXName, true, 0.3f);
            if (towerEffect == TowerEffect.ChargingTurret)
                ChargeAttackSpeed();
        }
    }

    void AttackAllNearEnemy()
    {
        List<GameObject> enemies = FindAllNearEnemy();

        if (enemies == null)
            return;

        foreach (GameObject enemy in enemies)
        {

            GameObject newBullet = poolScript.GetPoolObject(bulletG);
            BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();

            newBulletScript.target = enemy;
            newBulletScript.attackDamage = attackDamage;
            newBulletScript.moveSpeed = bulletSpeed;
            newBullet.transform.position = shootPos.transform.position;
            AudioManager.instance.Play(shootSFXName, true);


        }

    }

    bool IsEnemyInRange(GameObject target)
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        return distance <= attackRange;
    }

    bool IsListNullOrEmpty(GameObject[] list)
    {
        return list == null || (list != null && list.Length == 0);
    }

    GameObject[] GetAllEnemiesWithinRange()
    {
        GameObject[] _enemies;
        if (IsListNullOrEmpty(_enemies = poolScript.enemies))
            return null;
        List<GameObject> enemiesWithinRange = new List<GameObject>();
        foreach (GameObject enemy in _enemies)
        {
            if (IsEnemyInRange(enemy))
                enemiesWithinRange.Add(enemy);
        }
        return enemiesWithinRange.ToArray();
    }

    int GetFarthestPathPointAimed(GameObject[] _enemies)
    {
        if (IsListNullOrEmpty(_enemies))
            return 0;
        int farthestPathPointAimed = 0;
        foreach (GameObject enemy in _enemies)
        {
            IAScript enemyScript = enemy.GetComponent<IAScript>();
            if (enemyScript.GetPathPointIndex > farthestPathPointAimed)
                farthestPathPointAimed = enemyScript.GetPathPointIndex;
        }
        return farthestPathPointAimed;
    }

    GameObject[] GetEnemiesAimingThisPathPoint(GameObject[] _enemies, int pathPointIndex)
    {
        if (IsListNullOrEmpty(_enemies))
            return null;
        List<GameObject> enemiesAimingPathPoint = new List<GameObject>();
        foreach (GameObject enemy in _enemies)
        {
            IAScript enemyScript = enemy.GetComponent<IAScript>();
            if (enemyScript.GetPathPointIndex == pathPointIndex)
                enemiesAimingPathPoint.Add(enemy);
        }
        return enemiesAimingPathPoint.ToArray();
    }

    GameObject GetClosestEnemyToPathPoint(GameObject[] _enemies, Transform pathPoint)
    {
        if (IsListNullOrEmpty(_enemies))
            return null;
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in _enemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, pathPoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    // Find enemies within range DONE
    // Find the furthest pathpoint aimed DONE
    // Get enemies aiming for this pathpoint DONE
    // Find the closest enemy to this pathpoint DONE

    GameObject GetEnemyWithinRangeClosestToBase()
    {
        if (IsListNullOrEmpty(poolScript.enemies))
            return null;
        GameObject[] enemiesWithinRange = GetAllEnemiesWithinRange();
        int farthestPathPointIndex = GetFarthestPathPointAimed(enemiesWithinRange);
        GameObject[] enemiesAimingThisPoint = GetEnemiesAimingThisPathPoint(enemiesWithinRange, farthestPathPointIndex);
        Transform pathPoint = poolScript.pathArray[farthestPathPointIndex];
        GameObject closestEnemy = GetClosestEnemyToPathPoint(enemiesAimingThisPoint, pathPoint);
        return closestEnemy;
    }

    bool IsTargetDesactivate(GameObject target)
    {
        return !target || (target && !target.activeSelf);
    }

	void DrawLineToTarget()
	{
		if (!targetLineRenderer)
			return;
		if (target)
		{
			
			Vector3 startPos = new Vector3(transform.position.x, transform.position.y, -1);
			Vector3 endPos = new Vector3(target.transform.position.x, target.transform.position.y, -1);
			targetLineRenderer.SetPosition(0, startPos);
			targetLineRenderer.SetPosition(1, endPos);
			targetLineRenderer.enabled = true;
		}
		else
			targetLineRenderer.enabled = false;

	}

	void LookAtEnemy()
    {
        if (target != null)
        {
            //Calculate the direction.
            Vector3 dir = target.transform.position - partToRotate.position;

            float angle;

            //Calculate the angle to rotate to face the target.
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion newQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);

            //Rotate for X degre
            partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, newQuaternion, rotateSpeed * Time.deltaTime);
        }
    }

    //Check if the enemy is still active in the scene.
    void IsTheEnemyStillAlive()
    {
        if (target != null && target.GetComponent<ProgressBarScript>().currentHealth <= 0)
            target = null;


    }

    //Slow enemy within range.
    void SlowEnemy()
    {
        List<GameObject> enemies = FindAllNearEnemy();

        if (enemies == null)
            return;
        
        foreach (GameObject enemy in enemies)
        {
            IAScript enemyScript = enemy.GetComponent<IAScript>();
            enemyScript.Slow(slowAmount / 100f);
        }
    }

    void BurnEnemy()
    {
        List<GameObject> enemies = FindAllNearEnemy();

        ActivateEffect();

        if (enemies == null || enemies.Count == 0)
            return;

        foreach (GameObject enemy in enemies)
        {
            ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();

            progressBarScript.currentHealth -= attackDamage;
        }
    }

    public GameObject DisplayImpactEffect(GameObject effect)
    {
        if (effect == null)
            return null;

        GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
        return newEffect;
    }


    void ThrowLightning()
    {
        List<GameObject> enemies = FindAllNearEnemy();

        ActivateEffect();

        List<Vector3> lightningPositions = new List<Vector3>();

        //Null could means that the turret is reloading.
        if (enemies == null)
        {
            return;
        }

        //If there is not enemy, erase the line.
        if (enemies.Count == 0)
        {
            GetComponent<Animator>().SetBool("Attacking", false);
            lineRenderer.positionCount = 0;
            return;
        }

        GetComponent<Animator>().SetBool("Attacking", true);

        GetComponent<Animator>().Play("ElectricTurretAttack", 0);

        lightningPositions.Add(transform.position);

        foreach (GameObject enemy in enemies)
        {
            lightningPositions.Add(enemy.transform.position);
            lightningPositions.Add(enemy.transform.position + new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), 0));
            ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();
            progressBarScript.currentHealth -= attackDamage;
            GameObject newEffect = PoolObject.instance.GetPoolObject(lightningEffect);
            newEffect.transform.position = enemy.transform.position;
            AudioManager.instance.Play("Punch", true);
        }
        lineRenderer.positionCount = lightningPositions.Count;
        foreach (Vector3 pos in lightningPositions)
        {
            lineRenderer.SetPosition(lightningPositions.IndexOf(pos), pos);
        }
    }

    void LaserBeam()
    {
        List<GameObject> enemies = FindAllNearEnemy();
        ActivateEffect();
        List<Vector3> lightningPositions = new List<Vector3>();
        //Null means that the turret is reloading.
        if (enemies == null)
            return;
        //If there is not enemy, erase the line.
        if (enemies.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        lightningPositions.Add(transform.position);
        foreach (GameObject enemy in enemies)
        {
            lightningPositions.Add(transform.position);

            lightningPositions.Add(enemy.transform.position);

            ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();

            progressBarScript.currentHealth -= attackDamage;
        }
        lineRenderer.positionCount = lightningPositions.Count;
        int i = 0;
        foreach (Vector3 pos in lightningPositions)
        {
            
            lineRenderer.SetPosition(i, pos);
            i++;
        }
    }

    void ActivateEffect()
    {
        if (particleSystem == null)
            return;
        if (particleSystem.isPlaying == true && SpawnerScript.instance.enemiesRemainingToSpawn <= 0 && SpawnerScript.instance.enemiesRemainingToSpawn <= 0)
            particleSystem.Stop();
        else if(particleSystem.isPlaying == false)
            particleSystem.Play();
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
