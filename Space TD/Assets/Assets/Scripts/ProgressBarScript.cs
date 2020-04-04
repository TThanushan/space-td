using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProgressBarScript : MonoBehaviour {

    public enum Type{basic, Boss}

    public Type type = Type.basic;

    public float maxHealth;

    public float currentHealth;

	public GameObject Healthbar;

	public Text healthText;

	public float moneyGiven;

    
    public event System.Action OnDeath;

    void OnEnable()
    {
        Healthbar.transform.localScale = new Vector3(0, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);

        currentHealth = maxHealth;
    }

    void Start () {
        
        currentHealth = maxHealth;
		
    }

	
	void Update () {

        if(Healthbar != null)
            SetHealthBarFunc();
		if (healthText)
			healthText.text = (System.Math.Round(currentHealth,1)).ToString();
        Death();
	}

    void Death()
    {
        if (currentHealth <= 0)
        {
            
            if (OnDeath != null)
            {
                OnDeath();
                OnDeath = null;
            }

            gameObject.SetActive(false);
            if (type == Type.basic)
            {
                //Given money When i die.
                if (moneyGiven > 0)
                {
                    UIScript.instance.DisplayText("+" + moneyGiven + " $", transform.position, 6, Color.green);
                    PlayerStatsScript.instance.money += moneyGiven;
                }
                
                UIScript.instance.DisplayImpactEffect(UIScript.instance.deathEffect).transform.position = transform.position;
                AudioManager.instance.Play("Slime Splash", true);
            }
            else if (type == Type.Boss)
            {
                //Given money When i die.
                if (moneyGiven > 0)
                {
                    UIScript.instance.DisplayText("+" + moneyGiven + " $", transform.position, 18, Color.green);
                    PlayerStatsScript.instance.money += moneyGiven;
                }

                UIScript.instance.DisplayImpactEffect(UIScript.instance.bossDeathEffect).transform.position = transform.position;

                AudioManager.instance.Play("Boom", true);
                ShakeCamera.instance.Shake(0.5f, 1f);
            }
        
        }
    }

    void OnDisable()
    {
        if (OnDeath != null)
        {
            OnDeath();
            OnDeath = null;
        }
        
    }

    void SetHealthBarFunc()
    {
        
        float barLenght = currentHealth / maxHealth;

        Healthbar.transform.localScale = new Vector3(barLenght, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);

    }
}
