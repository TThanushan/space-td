using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TowerScript : MonoBehaviour {

	public enum TowerTargetMod { singleTarget, multipleTarget, special }
	public enum TowerEffect { noEffect, slowTarget, BurnTarget, Electric, LaserBeam }

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

	Transform partToRotate;

	GameObject target;

	GameObject[] enemies;

	PoolObjectScript poolScript;

	float nextAttackTime;

	GameObject[] newEnemie;


	[Header("Use Laser")]
	[HideInInspector]
	public LineRenderer lineRenderer;
	//Particle System

	public GameObject burnTurretEffectG;

	ParticleSystem particleSystem;

	LineRenderer targetLineRenderer;


	void Awake() {
		poolScript = GameObject.FindGameObjectWithTag("Data").GetComponent<PoolObjectScript>();
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

		targetLineRenderer = GetComponent<LineRenderer>();
		partToRotate = transform.Find("Sprite").transform;

		InvokeRepeating("LookAtEnemy", 0f, 1 * Time.deltaTime);

	}


	void Update() {
		if (isDisable)
			return;
		DrawLineToTarget();
		TowerManager();
	}


	void TowerManager()
	{
		if (towerEffect == TowerEffect.slowTarget)
		{
			SlowEnemy();
		}

		if (towerEffect == TowerEffect.BurnTarget)
		{
			BurnEnemy();
		}

		if (towerEffect == TowerEffect.Electric)
		{
			ThrowLightning();

		}

		if (towerEffect == TowerEffect.LaserBeam)
		{
			LaserBeam();

		}

		if (towerTargetMod == TowerTargetMod.singleTarget)
		{
			FindEnemies();
			LookAtEnemy();
			AttackEnemy();

			IsTheEnemyStillAlive();
		}
		else if (towerTargetMod == TowerTargetMod.multipleTarget)
		{
			AttackAllNearEnemy();
		}

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
			AudioManager.instance.Play("Tower Shoot", true);

		}

	}



	void FindEnemies()
	{
		enemies = poolScript.enemies;

		GameObject closestEnemy = null;

		float lowestDistance = Mathf.Infinity;

		foreach (GameObject enemy in enemies)
		{

			float Distance = Vector2.Distance(transform.position, enemy.transform.position);
			if (Distance < lowestDistance)
			{
				lowestDistance = Distance;
				closestEnemy = enemy;
			}

			if (lowestDistance < attackRange && closestEnemy != null && closestEnemy.activeSelf)
			{
				target = closestEnemy;
			}
			else
			{
				target = null;
			}
		}



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
            UIScript.instance.DisplayImpactEffect(UIScript.instance.slowEnemyEffect).transform.position = enemy.transform.position;

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

    void ThrowLightning()
    {
        List<GameObject> enemies = FindAllNearEnemy();

        ActivateEffect();

        List<Vector3> lightningPositions = new List<Vector3>();

        //Null means that the turret is reloading.
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

//            lineRenderer.SetPosition(1, enemy.transform.position);w²

            lightningPositions.Add(enemy.transform.position);
            lightningPositions.Add(enemy.transform.position + new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), 0));

//            print(enemy.transform.position +new Vector3(Random.Range(0, 2), Random.Range(0, 2), 0));

            ProgressBarScript progressBarScript = enemy.GetComponent<ProgressBarScript>();

            progressBarScript.currentHealth -= attackDamage;
            UIScript.instance.DisplayImpactEffect(UIScript.instance.electricEnemyEffect).transform.position = enemy.transform.position;

            AudioManager.instance.Play("Punch", true);
        }


        lineRenderer.positionCount = lightningPositions.Count;
        foreach (Vector3 pos in lightningPositions)
        {
            lineRenderer.SetPosition(lightningPositions.IndexOf(pos), pos);
        }

        // ITS WORKING , ATTACK ONLY ONE ENEMY AT THE TIME.
//        FindEnemies();
//
//        if (target == null)
//        {
//            lineRenderer.SetPosition(0, shootPos.transform.position);
//            lineRenderer.SetPosition(1, shootPos.transform.position);
//                
//            return;
//        }
//        target.GetComponent<ProgressBarScript>().currentHealth -= attackDamage;
//
//        lineRenderer.SetPosition(0, shootPos.transform.position);
//        lineRenderer.SetPosition(1, target.transform.position);
//
//        UIScript.instance.DisplayImpactEffect(UIScript.instance.electricEnemyEffect).transform.position = target.transform.position;

    }

    void LaserBeam()
    {


        List<GameObject> enemies = FindAllNearEnemy();

        ActivateEffect();

        List<Vector3> lightningPositions = new List<Vector3>();

        //Null means that the turret is reloading.
        if (enemies == null)
        {
            return;
        }

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

        

        if (particleSystem.isPlaying == true && SpawnerScript.instance.enemiesRemainingAlive <= 0 && SpawnerScript.instance.enemiesRemainingToSpawn <= 0)
            particleSystem.Stop();

        else if(particleSystem.isPlaying == false)
            particleSystem.Play();
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

            AudioManager.instance.Play("Tower Shoot", true);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
