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

    public event System.Action<GameObject> OnDeath;
    public GameObject deathEffect;

    public Gradient gradient;

    private Animator animator;

    void OnEnable()
    {
        Healthbar.transform.localScale = new Vector3(0, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);

        currentHealth = maxHealth;
        //DisableHealthBarFullLife();
    }

    void Start () {
        
        currentHealth = maxHealth;
        healthText = transform.Find("HealthBody/Body/HealthText").GetComponent<TextMeshProUGUI>();
        animator = transform.Find("HealthBody").GetComponent<Animator>();
    }

	
	void Update () {
        if (Healthbar != null)
            SetHealthBarFunc();
		if (healthText)
			healthText.text = (System.Math.Round(currentHealth,1)).ToString();
        Death();
        DecreaseHitBarSize();
        UpdateHealthBarColor();
    }

    void DisableHealthBarFullLife()
    {
        Transform healthBody = transform.Find("HealthBody");
        healthBody.gameObject.SetActive(currentHealth != maxHealth);
    }

    public void Death()
    {
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(gameObject);
            if (type == Type.basic)
            {
                if (moneyGiven > 0)
                {
					float randomXPos = transform.position.x + Random.Range(-0.4f, 0.4f);
                    UIScript.instance.DisplayText("+" + moneyGiven + " $", new Vector2(randomXPos, transform.position.y), 1f);
                    UIScript.instance.PlayGainMoneyAnimation();
                    PlayerStatsScript.instance.money += moneyGiven;
                }
                AudioManager.instance.Play("CoinsSlimeDeath", true);
            }       
            gameObject.SetActive(false);
        }
    }

    public bool GetDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        PlayHitAnimation();
        return currentHealth <= 0;
    }

    private void PlayHitAnimation()
    {
        animator.Play("Hit");
    }

    private void DecreaseHitBarSize()
    {
        Transform hitBar = GetHitBar();
        float decreaseSpeed = 0.5f;
        float lengthGoal = currentHealth / maxHealth;
        float newLength = hitBar.localScale.x - Time.deltaTime * decreaseSpeed;
        if (newLength < lengthGoal)
        {
            SetHealthBarLocalScaleX(lengthGoal);
            return;
        }
        else if (newLength == lengthGoal)
            return;
        SetHealthBarLocalScaleX(newLength);
    }

    private void UpdateHealthBarColor()
    {
        float value = (currentHealth / maxHealth);
        Healthbar.GetComponent<Image>().color = new Color(gradient.Evaluate(value).r, gradient.Evaluate(value).g, gradient.Evaluate(value).b);
    }

    private void SetHealthBarLocalScaleX(float x)
    {
        Transform hitBar = GetHitBar();
        hitBar.transform.localScale = new Vector3(x, hitBar.transform.localScale.y, hitBar.transform.localScale.z);

    }

    private Transform GetHitBar()
    {
        return transform.Find("HealthBody/Body/HitBar");
    }

    public bool IsKilled(float damage)
    {
        return currentHealth - damage <= 0;
    }



    void SetHealthBarFunc()
    {
        
        float barLenght = currentHealth / maxHealth;

        Healthbar.transform.localScale = new Vector3(barLenght, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);

    }
}
