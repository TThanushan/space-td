using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour {

    public static UIScript instance;

    public GameObject printedText;
    public GameObject impactEffect;
    public GameObject deathEffect;
    public GameObject bossDeathEffect;

    public GameObject skipButton;

    public Color orangeColor;

    public GameObject restartSprite;
    public GameObject gameoverPanel;
    public GameObject looseEffect;
    public GameObject endNode;
    public GameObject burnEnemyEffect;
    public GameObject electricEnemyEffect;
    public GameObject slowEnemyEffect;
    public GameObject gameCompleteEffect;

    public bool playerLoose = false;

    public float newMainVolume = 0.01f;

    public Camera camera;

    //Fade Restart.

    public SpriteRenderer black;
    public Animator anim;

    //Fade Restart.

    //GameOver Panel.

    public Animator gameOverAnim;
    public GameObject gameOverPanel;

    //GameOver Panel.

    //Option
    public Animator optionAnim;
    public GameObject optionPanel;

    //FPS Var.
    public float fps;
    float deltaTime;
    bool showFPS = false;


    //Button image
    public Image standardTurretImage;
    public Image fastTurretImage;
    public Image multiTurretImage;
    public Image slowTurretImage;
    public Image burnTurretImage;
    public Image electricTurretImage;
    public Image superMultiTurretImage;
    public Image laserTurretImage;
    [HideInInspector]
	public Text lifeText;
    [HideInInspector]
    public Text moneyText;
	Text waveNumberText;
	//Text waveTimeText;
	public Transform waveTimeBar;
	public GameObject waveTime;

	float cameraSizeSave;
    bool gameComplete = false;
	Dictionary<string, string> animationsDict;

	void Awake() {
		animationsDict = new Dictionary<string, string>();
		animationsDict.Add("Normal", "PrintedTextCanvas");
		animationsDict.Add("Fast", "Fast PrintedTextCanvas");
		animationsDict.Add("Slow", "Slow PrintedTextCanvas");
		animationsDict.Add("Special", "PrintedTextCanvas Special");

		lifeText = gameObject.transform.Find("lifeText").GetComponent<Text>();
        moneyText = gameObject.transform.Find("moneyText").GetComponent<Text>();
        waveNumberText = gameObject.transform.Find("waveNumber").GetComponent<Text>();
        //waveTimeText = gameObject.transform.Find("waveTime").GetComponent<Text>();
        endNode = GameObject.FindGameObjectWithTag("End").gameObject;
        if (gameOverPanel == null)
            gameoverPanel = GameObject.Find("GameOver Panel").gameObject;
        if (gameOverAnim == null)
            gameoverPanel.GetComponent<Animator>();
            

        if (instance == null)
            instance = this;
    }

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
	


	void Update () {

        lifeText.text = PlayerStatsScript.instance.life.ToString();
        moneyText.text = PlayerStatsScript.instance.money.ToString() + " $";
        waveNumberText.text = "Wave : " + SpawnerScript.instance.currentWaveNumber.ToString();

        if (SpawnerScript.instance.nextWaveTime > 0 && SpawnerScript.instance.enemiesRemainingAlive <= 0 && SpawnerScript.instance.enemiesRemainingToSpawn <= 0)
        {
            //skipButton.SetActive(true);
			waveTime.SetActive(true);
			//waveTimeText.text = "Next Wave : " + SpawnerScript.instance.nextWaveTime.ToString("0");
			UpdateWaveTimerBar();
		}
		else
        {
            //skipButton.SetActive(false);
			waveTime.SetActive(false);
			//waveTimeText.text = "";

		}

		MainGameOverFunc();

        PlayerInputs();

        HighlightTowerButton(standardTurretImage, ShopScript.instance.standardTurret.cost);
        HighlightTowerButton(fastTurretImage, ShopScript.instance.fastTurret.cost);
        HighlightTowerButton(multiTurretImage, ShopScript.instance.multiTurret.cost);
        HighlightTowerButton(slowTurretImage, ShopScript.instance.slowTurret.cost);
        HighlightTowerButton(burnTurretImage, ShopScript.instance.burnTurret.cost);
        HighlightTowerButton(electricTurretImage, ShopScript.instance.electricTurret.cost);
        HighlightTowerButton(superMultiTurretImage, ShopScript.instance.superMultiTurret.cost);
        HighlightTowerButton(laserTurretImage, ShopScript.instance.laserTurret.cost);


        if (SpawnerScript.instance.currentWaveNumber >= SpawnerScript.instance.numberOfWaves + 1 && gameComplete == false)
            StartCoroutine(GameComplete());
    }

	void UpdateWaveTimerBar()
	{
		
		float barLenght = SpawnerScript.instance.nextWaveTime / 10;

		waveTimeBar.localScale = new Vector3(barLenght, waveTimeBar.localScale.y, waveTimeBar.localScale.z);

	}

	void PlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
            showFPS = !showFPS;
    }

    void HighlightTowerButton(Image _image, float _cost)
    {
        if (_image == null)
            return;
        
        if (_cost <= PlayerStatsScript.instance.money)
            _image.color = new Color(0f, 0.352f, 0.176f);
        else
            _image.color = Color.black;
        
    }

    public GameObject DisplayImpactEffect(GameObject effect)
    {
        if (effect == null)
            return null;

        GameObject newEffect = PoolObjectScript.instance.GetPoolObject(effect);
        return newEffect;
    }

    void MainGameOverFunc()
    {
        
        //If the player got 0 or less life then instantiate effect and display the gameOver panel.
        if (PlayerStatsScript.instance.life <= 0 && !playerLoose)
        {
            PoolObjectScript.instance.GetPoolObject(looseEffect).transform.position = endNode.transform.position;
            
            AudioManager.instance.Play("Boom3", false);

            Time.timeScale = 0.2f;
            Invoke("GameOverFunc", 1f);
            playerLoose = true;
            ShakeCamera.instance.Shake(0.2f, 0.5f);
        }
    }

    public void ChangeMainVolume(float _volume)
    {
        AudioManager.instance.ChangeMainVolume(_volume);
    }

    void HideUI(bool value)
    {
        if(value)
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        
    }

    public void SpeedButton(Transform spriteGroup)
    {
        if (Time.timeScale == 1)
        {
            
            spriteGroup.Find("Sprite1").gameObject.SetActive(true);
            spriteGroup.Find("Sprite2").gameObject.SetActive(true);
            spriteGroup.Find("Sprite3").gameObject.SetActive(false);

            Time.timeScale = 1.7f;
        }
        else if (Time.timeScale == 1.7f)
        {
            spriteGroup.Find("Sprite1").gameObject.SetActive(true);
            spriteGroup.Find("Sprite2").gameObject.SetActive(true);
            spriteGroup.Find("Sprite3").gameObject.SetActive(true);

            Time.timeScale = 2.3f;
        }
        else
        {
            spriteGroup.Find("Sprite1").gameObject.SetActive(true);
            spriteGroup.Find("Sprite2").gameObject.SetActive(false);
            spriteGroup.Find("Sprite3").gameObject.SetActive(false);

            Time.timeScale = 1;
        }
    }

    public void MuteSfxVolume(GameObject _image)
    {
        AudioManager.instance.MuteSfxVolume(_image);
    }

    public void MuteMusicVolume(GameObject _image)
    {
        AudioManager.instance.MuteMusicVolume(_image);
    
    }



    void GameOverFunc()
    {
        
        gameoverPanel.transform.Find("waveNumber").GetComponent<Text>().text = waveNumberText.text;
        SpecialFuncScript.DisplayPanel(gameOverAnim, gameoverPanel);
    }
   
    public void SkipWaveTime()
    {
        int moneyGiven = (int)(SpawnerScript.instance.nextWaveTime) * 2;
        PlayerStatsScript.instance.money += moneyGiven;
        DisplayText("+" + moneyGiven.ToString() + "$", new Vector2(0, 8), 10, Color.green);

        SpawnerScript.instance.nextWaveTime = 0;
        AudioManager.instance.Play("Tiny Button", true);

    }

    public void StartOption()
    {
        StartCoroutine(Option());
        AudioManager.instance.Play("Jump", true);
    }
	
    IEnumerator Option()
    {
        if (Time.timeScale != 0)
        {
            SpecialFuncScript.DisplayPanel(optionAnim, optionPanel);
            yield return new WaitUntil(() => optionAnim.GetCurrentAnimatorStateInfo(0).IsName("Over"));
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            SpecialFuncScript.DisplayPanel(optionAnim, optionPanel);

        }
    }

    public void StartRestart(int sceneIndex)
    {
        Time.timeScale = 1;
        StartCoroutine(Restart(SceneManager.GetActiveScene().buildIndex));
        AudioManager.instance.Play("Jump", true);

    }

    public void Quit(int sceneIndex)
    {
        Time.timeScale = 1;
        StartCoroutine(Restart(sceneIndex));
        AudioManager.instance.Play("Jump", true);

    }

    IEnumerator GameComplete()
    {
        Time.timeScale = 1;

        HideUI(true);

        gameComplete = true;
        PoolObjectScript.instance.GetPoolObject(gameCompleteEffect).transform.position = GameObject.FindGameObjectWithTag("SpawnerNode").gameObject.transform.position;

        AudioManager.instance.Play("Boom3", false);

        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(2);
        
    }


    IEnumerator Restart(int sceneIndex)
    {
        HideUI(true);

        anim.SetBool("Fade", true);
        yield return new WaitUntil(()=>black.color.a == 1);
        SceneManager.LoadScene(sceneIndex);
        
    }


	
	

	//PrintedTextCanvas, PrintedTextCanvas Special, Slow PrintedTextCanvas, Fast PrintedTextCanvas
	public void DisplayText(string text, Vector2 textPosition, int textSize, Color textColor, string type="Normal") 
    {
		printedText = Resources.Load(animationsDict[type]) as GameObject;
        GameObject newCanvasTextG = PoolObjectScript.instance.GetPoolObject(printedText);
        GameObject newTextG = newCanvasTextG.transform.Find("Text").gameObject;
		newTextG.GetComponent<Text>().text = text;
        newTextG.GetComponent<Text>().color = textColor;
        newCanvasTextG.transform.position = textPosition;
        newTextG.transform.localScale = new Vector2(textSize, textSize);
        
//        newTextG.GetComponent<Text>().color = textColor;
    }

    void OnGUI()
    {
        if (!showFPS || Time.timeScale == 0)
            return;

        int w = Screen.width, h = Screen.height;

        Rect rect = new Rect(0, 700, w, h);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1f);

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);


    }
}
