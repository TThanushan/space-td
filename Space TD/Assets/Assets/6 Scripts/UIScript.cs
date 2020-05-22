using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class UIScript : MonoBehaviour {

    public static UIScript instance;
    public GameObject skipButton;
    public bool playerLoose = false;

    public Animator fadeAnimator;
    public Transform loadingBarTransform;

    public Animator gameOverAnim;
    public GameObject gameOverPanel;

    public GameObject levelCompletePanel;

    public Animator optionAnim;
    public GameObject optionPanel;

    public float fps;
    float deltaTime;
    public bool showFPS = true;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI waveNumberText;
    public Transform waveTimeBar;
    public GameObject waveTime;
    float cameraSizeSave;
    bool isLevelComplete = false;
    Dictionary<string, string> animationsDict;

    void Awake() {
        animationsDict = new Dictionary<string, string>();
        animationsDict.Add("Red", "RedPrintedTextCanvas");
        animationsDict.Add("Green", "GreenPrintedTextCanvas");
        if (instance == null)
            instance = this;
    }
    void Update() {

        lifeText.text = PlayerStatsScript.instance.life.ToString();
        moneyText.text = PlayerStatsScript.instance.money.ToString() + " $";
        int waveNb = SpawnerScript.instance.currentWaveNumber + 1;
        int numberOfWaves = SpawnerScript.instance.numberOfWaves;

        if (waveNb > numberOfWaves)
            waveNb = numberOfWaves;
        waveNumberText.text = "Wave " + waveNb.ToString() + " / " + numberOfWaves.ToString();

        if (SpawnerScript.instance.nextWaveTime > 0)
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

        PlayerInputs();
        IsLevelComplete();
        HasPlayerLose();
    }

    public void PlayRandomMusic()
    {
        TrackPlayer.instance.PlayRandomMusic();
    }

    void UpdateWaveTimerBar()
    {

        float barLenght = SpawnerScript.instance.nextWaveTime / 10;

        waveTimeBar.localScale = new Vector3(barLenght, waveTimeBar.localScale.y, waveTimeBar.localScale.z);

    }

    public void Disable(Transform obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void Enable(Transform obj)
    {
        obj.gameObject.SetActive(true);
    }

    void PlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
            showFPS = !showFPS;
    }


    public void ChangeMainVolume(float _volume)
    {
        AudioManager.instance.ChangeMainVolume(_volume);
    }

    public void HideUI(bool value)
    {
        if (value)
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

    }

    public void SpeedButton(Transform spriteGroup)
    {
        if (Time.timeScale == 0.5f)
        {
            spriteGroup.Find("Arrow1").gameObject.SetActive(true);
            spriteGroup.Find("Arrow2").gameObject.SetActive(true);
            spriteGroup.Find("Arrow3").gameObject.SetActive(false);
            spriteGroup.Find("Arrow4").gameObject.SetActive(false);

            Time.timeScale = 1f;
        }
        else if (Time.timeScale == 1)
        {

            spriteGroup.Find("Arrow1").gameObject.SetActive(true);
            spriteGroup.Find("Arrow2").gameObject.SetActive(true);
            spriteGroup.Find("Arrow3").gameObject.SetActive(true);
            spriteGroup.Find("Arrow4").gameObject.SetActive(false);

            Time.timeScale = 2f;
        }
        else if (Time.timeScale == 2f)
        {
            spriteGroup.Find("Arrow1").gameObject.SetActive(true);
            spriteGroup.Find("Arrow2").gameObject.SetActive(true);
            spriteGroup.Find("Arrow3").gameObject.SetActive(true);
            spriteGroup.Find("Arrow4").gameObject.SetActive(true);

            Time.timeScale = 4f;
        }
        else
        {
            spriteGroup.Find("Arrow1").gameObject.SetActive(true);
            spriteGroup.Find("Arrow2").gameObject.SetActive(false);
            spriteGroup.Find("Arrow3").gameObject.SetActive(false);
            spriteGroup.Find("Arrow4").gameObject.SetActive(false);

            Time.timeScale = 0.5f;
        }
    }

    public void MuteSfxVolume(GameObject _image)
    {
        AudioManager.instance.MuteSfxVolume(_image);
    }

    public void SkipWaveTime()
    {
        SpawnerScript.instance.nextWaveTime = 0;
    }

    public void PlaySfx(string sfxName)
    {
        if (sfxName == "")
            sfxName = "Usual Button";
        AudioManager.instance.Play(sfxName, false);
    }

    public void PlaySfxWithPitch(string sfxName)
    {
        if (sfxName == "")
            sfxName = "Usual Button";
        AudioManager.instance.Play(sfxName, true);
    }
    public void StartOption()
    {
        StartCoroutine(Option());
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

    public void NextLevel()
    {
        Time.timeScale = 1;
        StartCoroutine(FadeInScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void RestartLevel()
    {
        PlayerStatsScript.instance.pause = false;
        Time.timeScale = 1;
        StartCoroutine(FadeInScene(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator FadeInScene(int sceneIndex)
    {
        fadeAnimator.Play("FadeIn");
        yield return new WaitUntil(() => loadingBarTransform.localPosition.x == 0);
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeInScene(sceneIndex));
    }

    void IsLevelComplete()
    {
        if (SpawnerScript.instance.currentWaveNumber >= SpawnerScript.instance.numberOfWaves && isLevelComplete == false)
        {
            Time.timeScale = 1;
            SpecialFuncScript.DisplayPanel(levelCompletePanel.GetComponent<Animator>(), levelCompletePanel);
            isLevelComplete = true;
            levelCompletePanel.SetActive(true);
        }
    }

    void HasPlayerLose()
    {
        if (!playerLoose && PlayerStatsScript.instance.life <= 0)
        {
            Time.timeScale = 1;
            gameOverPanel.transform.Find("WaveNumberText").GetComponent<TextMeshProUGUI>().text = waveNumberText.text;
            SpecialFuncScript.DisplayPanel(gameOverPanel.GetComponent<Animator>(), gameOverPanel);
            playerLoose = true;
            gameOverPanel.SetActive(true);
        }
    }

	//PrintedTextCanvas, PrintedTextCanvas Special, Slow PrintedTextCanvas, Fast PrintedTextCanvas
	public void DisplayText(string text, Vector2 textPosition, float textSize=1, string type="Green") 
    {
		GameObject printedText = Resources.Load(animationsDict[type]) as GameObject;
        GameObject textCanvas = PoolObject.instance.GetPoolObject(printedText);
        GameObject textG = textCanvas.transform.Find("Text").gameObject;

        TextMeshProUGUI textMesh = textG.GetComponentInChildren<TextMeshProUGUI>();

        textMesh.text = text;
        textCanvas.transform.position = textPosition;
        textCanvas.transform.localScale = new Vector2(textSize, textSize);
        
//        newTextG.GetComponent<Text>().color = textColor;
    }

    void OnGUI()
    {
        if (!showFPS || Time.timeScale == 0)
            return;

        int w = 800, h = 600;
        Rect rect = new Rect(1, 1, w, h);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 15 / 1000;
        style.normal.textColor = new Color(1f, 1f, 1f, 1f);

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);


    }
}
