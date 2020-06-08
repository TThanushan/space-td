using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TowerScript : MonoBehaviour {

    public enum TowerTargetMod { singleTarget, multipleTarget, special }
    public enum TowerEffect { noEffect, slowTarget, Electric, LaserBeam, ChargingTurret }

    [Header("Main Data")]
    public float attackDamage = 1;

    public float attackSpeed;

    public float attackRange;

    public int killCount;

    public float damageDealt;

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

    [Header("Eletric")]
    public GameObject lightningEffect;

    public int lightningBounceCount;

    public float lightningBounceRange;

    public float lightningBounceDelay = 0.15f;

    Transform partToRotate;

    GameObject target;

    GameObject[] enemies;

    PoolObject poolScript;

    float nextAttackTime;

    GameObject[] newEnemie;

    [Header("Use Laser")]
    public LineRenderer lineRenderer;
    //Particle System

    [Header("Charging Turret")]
    public float chargingSpeed;
    public float maxAttackspeed;
    public float minAttackspeed;
    private Image chargingBar;

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

    private void OnDisable()
    {
        killCount = 0;
        damageDealt = 0;
    }

    void TowerManager()
    {
        if (towerEffect == TowerEffect.slowTarget)
            SlowEnemy();
        if (towerEffect == TowerEffect.Electric)
            ThrowLightning();
        if (towerEffect == TowerEffect.LaserBeam)
            LaserBeam();
        if (towerEffect == TowerEffect.ChargingTurret)
        {
            UpdateChargingBar();
            ChargeAttackSpeed();
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
        if (!ReadyToShoot())
            return;
        if (target && attackSpeed > minAttackspeed)
            attackSpeed -= chargingSpeed;
        if (attackSpeed < minAttackspeed)
            attackSpeed = minAttackspeed;
    }

    void UnchargeAttackSpeed()
    {
        if (!target && attackSpeed < maxAttackspeed)
            attackSpeed += chargingSpeed / 8;
        if (attackSpeed > maxAttackspeed)
            attackSpeed = maxAttackspeed;
    }


    List<GameObject> FindAllNearEnemy()
    {
        enemies = poolScript.enemies;
        List<GameObject> enemiesToReturn = new List<GameObject>();
        if (ReadyToShoot())
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

        if (target && ReadyToShoot())
        {
            nextAttackTime = Time.time + attackSpeed;
            GameObject newBullet = InstantiateBullet();
            BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();

            if (target.GetComponent<ProgressBarScript>().IsKilled((int)attackDamage))
                killCount++;

            AddSubscribers();
            PlayAnimation();
            AudioManager.instance.Play(shootSFXName, true, 0.3f);

            if (towerEffect == TowerEffect.ChargingTurret)
                ChargeAttackSpeed();
        }
    }

    private bool ReadyToShoot()
    {
        return Time.time > nextAttackTime;
    }

    private void AddSubscribers()
    {
        if (!bulletG)
            return;
        if (bulletG.GetComponent<MissileBullet>())
        {
            MissileBullet.enemyKilled += IncreaseKillCount;
            BulletScript.damageEvent += IncreaseDamageDealtMissile;
        }
        else
            BulletScript.damageEvent += IncreaseDamageDealt;
    }

    private void PlayAnimation()
    {
        if (animator)
            animator.Play("Shoot");
    }
    private GameObject InstantiateBullet()
    {
        if (!bulletG)
            return null;
        GameObject newBullet = poolScript.GetPoolObject(bulletG);
        newBullet.transform.position = shootPos.transform.position;

        BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();
        newBulletScript.target = target;
        newBulletScript.attackDamage = attackDamage;
        newBulletScript.moveSpeed = bulletSpeed;
        return newBullet;
    }

    private void IncreaseKillCount(int nbKilled)
    {
        killCount += nbKilled;
        MissileBullet.enemyKilled -= IncreaseKillCount;
    }

    private void IncreaseDamageDealtMissile(float damage)
    {
        damageDealt += damage;
        BulletScript.damageEvent -= IncreaseDamageDealtMissile;
    }

    private void IncreaseDamageDealt(float damage)
    {
        if (!target)
            return;
        float enemyHealth = target.GetComponent<ProgressBarScript>().currentHealth - damage;
        if (enemyHealth < 0)
            damage += enemyHealth;
        damageDealt += damage;
        BulletScript.damageEvent -= IncreaseDamageDealt;
    }

    private void IncreaseDamageDealt(float damage, GameObject target)
    {
        if (!target)
            return;
        float damageDealtOnEnemy = target.GetComponent<ProgressBarScript>().currentHealth - damage;
        if (damageDealtOnEnemy < 0)
            damageDealtOnEnemy += damage;
        damageDealt += damage;
        BulletScript.damageEvent -= IncreaseDamageDealt;
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

    public GameObject DisplayImpactEffect(GameObject effect)
    {
        if (effect == null)
            return null;

        GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
        return newEffect;
    }

    void ThrowLightning()
    {
        if (!IsReloadOver())
            return;
        List<Vector3> lightningPositions = new List<Vector3>();
        List<GameObject> enemiesTouched = new List<GameObject>();
        GameObject enemy;
        if (!(enemy = BounceOnFirstEnemy(lightningPositions, enemiesTouched)))
            return;
        nextAttackTime = Time.time + attackSpeed;
        BounceOnOtherCloseEnemy(enemy, lightningPositions, enemiesTouched);
        UpdateLineRenderer(lightningPositions);
        PlayLightningEffect();
    }
    GameObject BounceOnFirstEnemy(List<Vector3> lightningPositions, List<GameObject> enemiesTouched)
    {
        GameObject enemy;
        if (!(enemy = GetEnemyWithinRangeClosestToBase()))
        {
            StopLightningEffect();
            return null;
        }
        lightningPositions.Add(transform.position);

        AttackEnemy(enemy);
        IncreaseDamageDealt(attackDamage, enemy);
        enemiesTouched.Add(enemy);
        lightningPositions.Add(enemy.transform.position);
        return enemy;
    }

    void BounceOnOtherCloseEnemy(GameObject enemy, List<Vector3> lightningPositions, List<GameObject> enemiesTouched)
    {
        for (int i = 0; i < lightningBounceCount - 1; i++)
        {
            if (!(enemy = GetClosestEnemyFromTargetNotIn(enemy, enemiesTouched)))
                break;
            StartCoroutine(ElectricBounceAfterTime(enemy, i));
            enemiesTouched.Add(enemy);
        }
    }

    void StopLightningEffect()
    {
        GetComponent<Animator>().SetBool("Attacking", false);
        lineRenderer.positionCount = 0;
    }

    void PlayLightningEffect()
    {
        GetComponent<Animator>().SetBool("Attacking", true);
        GetComponent<Animator>().Play("ElectricTurretAttack", 0);
    }

    void AttackEnemy(GameObject target)
    {
        if (target.GetComponent<ProgressBarScript>().GetDamage((int)attackDamage))
            killCount++;
        IncreaseDamageDealt(attackDamage);
        GameObject newEffect = PoolObject.instance.GetPoolObject(lightningEffect);
        newEffect.transform.position = target.transform.position;
        AudioManager.instance.Play("Punch", true);
    }

    bool IsReloadOver()
    {
        return Time.time > nextAttackTime;
    }

    void UpdateLineRenderer(List<Vector3> _list)
    {
        lineRenderer.positionCount = _list.Count;
        foreach (Vector3 current in _list)
        {
            lineRenderer.SetPosition(_list.IndexOf(current), current);
        }
    }

    GameObject GetClosestEnemyFromTargetNotIn(GameObject _target, List<GameObject> _enemies)
    {
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in poolScript.enemies)
        {
            if (_enemies.Contains(enemy))
                continue;
            float distance = Vector2.Distance(enemy.transform.position, _target.transform.position);
            if (distance < closestDistance && distance < lightningBounceRange)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    IEnumerator ElectricBounceAfterTime(GameObject enemy, float time)
    {
        yield return new WaitForSeconds(lightningBounceDelay * time);

        AddPositionToLineRenderer(enemy.transform.position);
        //if (i < lightningBounceCount - 1)
        AddPositionToLineRenderer(enemy.transform.position + new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0));
        AttackEnemy(enemy);
        IncreaseDamageDealt(attackDamage, enemy);
    }

    void AddPositionToLineRenderer(Vector3 pos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    void LaserBeam()
    {
        List<GameObject> enemies = FindAllNearEnemy();
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

            if (enemy.GetComponent<ProgressBarScript>().GetDamage((int)attackDamage))
                killCount++;
        }
        lineRenderer.positionCount = lightningPositions.Count;
        int i = 0;
        foreach (Vector3 pos in lightningPositions)
        {
            
            lineRenderer.SetPosition(i, pos);
            i++;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
