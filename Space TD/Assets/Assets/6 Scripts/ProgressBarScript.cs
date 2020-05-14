using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
public class ProgressBarScript : MonoBehaviour {

    public enum Type{basic, Boss}

    public Type type = Type.basic;

    public float maxHealth;

    public float currentHealth;

	public GameObject Healthbar;

	public TextMeshProUGUI healthText;

	public float moneyGiven;

    public event System.Action OnDeath;
    public GameObject deathEffect;

    void OnEnable()
    {
        Healthbar.transform.localScale = new Vector3(0, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);

        currentHealth = maxHealth;
    }

    void Start () {
        
        currentHealth = maxHealth;
        healthText = transform.Find("HealthBody/HealthText").GetComponent<TextMeshProUGUI>();
        InvokeRepeating("DisableHealthBarFullLife", 0f, 1f);
    }

	
	void Update () {

        if(Healthbar != null)
            SetHealthBarFunc();
		if (healthText)
			healthText.text = (System.Math.Round(currentHealth,1)).ToString();
        Death();

        DisableHealthBarFullLife();
	}

    void DisableHealthBarFullLife()
    {
        Transform healthBody = transform.Find("HealthBody");
        if (currentHealth == maxHealth)
            healthBody.gameObject.SetActive(false);
        else
            healthBody.gameObject.SetActive(true);
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
            if (type == Type.basic)
            {
                //Given money When i die.
                if (moneyGiven > 0)
                {
					float randomXPos = transform.position.x + Random.Range(-0.4f, 0.4f);
                    UIScript.instance.DisplayText("+" + moneyGiven + " $", new Vector2(randomXPos, transform.position.y), 1f);
                    PlayerStatsScript.instance.money += moneyGiven;
                }
                AudioManager.instance.Play("CoinsSlimeDeath", true);
            }       
            gameObject.SetActive(false);
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
