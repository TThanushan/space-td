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
    public Animator announceWaveAnimator;

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

    private void Start()
    {
        SpawnerScript.instance.OnWaveOver += PlayWaveStartButtonShowAnimation;
    }

    void Update() {
        lifeText.text = PlayerStatsScript.instance.life.ToString();
        moneyText.text = PlayerStatsScript.instance.money.ToString() + " $";
        int waveNb = SpawnerScript.instance.currentWaveNumber + 1;
        int numberOfWaves = SpawnerScript.instance.numberOfWaves;

        if (waveNb > numberOfWaves)
            waveNb = numberOfWaves;
        //waveNumberText.text = "Wave " + waveNb.ToString() + " / " + numberOfWaves.ToString();
        waveNumberText.text = "Wave " + waveNb.ToString();

        PlayerInputs();
        IsLevelComplete();
        HasPlayerLose();
    }


    private void PlayWaveStartButtonShowAnimation()
    {
        waveTime.GetComponent<Animator>().Play("Show");
    }

    public void MuteMusic(GameObject sprite)
    {
        TrackPlayer.instance.MuteMusic();
        sprite.SetActive(!sprite.activeSelf);
    }

    public void PlayRandomMusic()
    {
        TrackPlayer.instance.PlayRandomMusic();
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

    public void PlayGainMoneyAnimation()
    {
        moneyText.GetComponent<Animator>().Play("GainMoney");
    }

    public void SetTimeSpeed(float _speed)
    {
        Time.timeScale = _speed;
    }

    public void SetImageEnableColor(Image image)
    {
        image.color = new Color(0f, 0.7193136f, 1f, 1f);
    }
    public void SetImageDisabledColor(Image image)
    {
        image.color = new Color(0f, 0.1110315f, 0.2735849f, 1f);
    }

    public void MuteSfxVolume(GameObject _image)
    {
        AudioManager.instance.MuteSfxVolume(_image);
    }

    public void MuteSfxVolume(bool value)
    {
        AudioManager.instance.MuteSfxVolume(value);
    }

    public void SkipWaveTime()
    {
        SpawnerScript.instance.StartWave();
        waveTime.GetComponent<Animator>().Play("Hide");
    }

    public void PlayAnnounceWaveAnimation()
    {
        announceWaveAnimator.gameObject.transform.Find("Text (TMP)")
            .GetComponent<TextMeshProUGUI>().text 
            = "Wave " + (SpawnerScript.instance.currentWaveNumber + 1);
        StartCoroutine(UnpauseAtAnnounceAnimationEnd());
    }

    IEnumerator UnpauseAtAnnounceAnimationEnd()
    {
        PlayerStatsScript.instance.pause = true;
        announceWaveAnimator.Play("Show");
        //yield return new WaitUntil(() => announceWaveAnimator.GetCurrentAnimatorStateInfo(0).IsName("New State") == false);
        yield return new WaitForSeconds(1.75f);
        PlayerStatsScript.instance.pause = false;
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

    public void PauseGame()
    {
        PlayerStatsScript.instance.PauseGame();
    }

}
