using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public Animator fadeAnimator;
    public Transform loadingBarTransform;
    public Animator creditsAnimator;
    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void PlaySfx(string sfxName)
    {
        AudioManager.instance.Play(sfxName, false);
    }

    public void PlaySfxWithPitch(string sfxName)
    {
        AudioManager.instance.Play(sfxName, true);
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

    public void Quit()
    {
        LoadScene(0);
    }

    public void DisplayCredits()
    {
        creditsAnimator.Play("DisplayIn");
    }

    public void HideCredits()
    {
        creditsAnimator.Play("DisplayOut");
    }
}
