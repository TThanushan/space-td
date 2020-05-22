using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour
{

    private void Start()
    {
        PlayerStatsScript.instance.pause = true;
        if (transform.Find("TurretIntroductionPanel"))
            transform.Find("TurretIntroductionPanel").gameObject.SetActive(true);
    }
    
    public void ClosePanel()
    {
        GameObject introPanel = transform.Find("TurretIntroductionPanel").gameObject;
        if (!introPanel)
            return;
        Animator introPanelAnimator = introPanel.GetComponent<Animator>();
        introPanelAnimator.Play("FadeOut");
        //introPanel.gameObject.SetActive(false);
    }

    public void UnpauseGame()
    {
        PlayerStatsScript.instance.pause = false;
    }
}
