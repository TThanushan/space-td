using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public Animator creditsAnim;
    public GameObject credits;
    public Animator fadeAnim;
    public SpriteRenderer black;

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void StartPlay(int sceneIndex)
    {
        StartCoroutine(Play(sceneIndex));
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    IEnumerator Play(int sceneIndex)
    {
        fadeAnim.SetBool("Fade", true);
        AudioManager.instance.Play("Jump", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartDisplayCredits()
    {
        AudioManager.instance.Play("Jump", true);

        if (credits.activeSelf == false)
            credits.SetActive(true);
    
        if (!creditsAnim.GetBool("Display"))
        {
            creditsAnim.SetBool("Display", true);
        }
        else
        {
            creditsAnim.SetBool("Display", false);
        }

    }
}
