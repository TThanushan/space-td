using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour
{
    public TutoPanel[] tutoPanels;
    private int currentIndex;
    private float oldTimeSpeed = 1f;

    private void Update()
    {
        if (currentIndex >= tutoPanels.Length)
            Invoke("DisableGameobject", 3f);
        ShowTutoPanel();
    }

    private void DisableGameobject()
    {
        Destroy(gameObject);
    }
    private void ShowTutoPanel()
    {
        if (currentIndex >= tutoPanels.Length || !NeedToShowNextPanel()) return;
        if (Time.timeScale != 1f)
            oldTimeSpeed = Time.timeScale;
        Time.timeScale = 1;
        PlayerStatsScript.instance.pause = true;
        SetTutoPanelsProperties();
        BuildManagerScript.instance.DeselectNode();
        NodeUI.instance.ShowRangeUpgrade(false);
    }

    private void SetTutoPanelsProperties()
    {
        tutoPanels[currentIndex].panelRef.SetActive(true);
        tutoPanels[currentIndex].turretButton.SetActive(true);
        tutoPanels[currentIndex].HasBeenShown = true;
    }

    private bool NeedToShowNextPanel()
    {
        return SpawnerScript.instance.currentWaveNumber + 1 == tutoPanels[currentIndex].waveWhenToShow;
    }

    public void ClosePanel()
    {
        GameObject introPanel = tutoPanels[currentIndex].panelRef;
        Animator introPanelAnimator = introPanel.GetComponent<Animator>();
        introPanelAnimator.Play("FadeOut");
        //PlayerStatsScript.instance.pause = false;
        currentIndex++;
        Time.timeScale = oldTimeSpeed;
    }

    public void UnpauseGame()
    {
        PlayerStatsScript.instance.PauseGame(false);
    }

    [System.Serializable]
    public class TutoPanel
    {
        public string name;
        public int waveWhenToShow;
        public GameObject panelRef;
        public GameObject turretButton;
        private bool hasBeenShown = false;
        public bool HasBeenShown { get => hasBeenShown; set => hasBeenShown = value; }
    }
}
