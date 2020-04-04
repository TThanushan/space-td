using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float attackRange = 1;

    public float moveSpeed = 1;

    public float attackDamage = 1;

    public GameObject target;



	void Start () {
		
	}
    void FixedUpdate()
    {
        
        AttackTarget();
    }
    
    void Update () {
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
            target.GetComponent<ProgressBarScript>().currentHealth -= attackDamage;
            gameObject.SetActive(false);
//            UIScript.instance.DisplayImpactEffect(UIScript.instance.impactEffect).transform.position = transform.position;
//            AudioManager.instance.Play("HitSFX2", true);
        }


        if(target == null)
        {
            
            gameObject.SetActive(false);

//            UIScript.instance.DisplayImpactEffect(UIScript.instance.impactEffect).transform.position = transform.position;

        }

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
