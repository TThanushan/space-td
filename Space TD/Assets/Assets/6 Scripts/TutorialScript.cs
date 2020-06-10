using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour
{
    public TutoPanel[] tutoPanels;

    private void Update()
    {
        ShowTutoPanel();
    }

    private void ShowTutoPanel()
    {
        int currentWaveNumber = SpawnerScript.instance.currentWaveNumber;
        if (currentWaveNumber >= tutoPanels.Length)
            return;

        if (!tutoPanels[currentWaveNumber].HasBeenShown)
        {
            Time.timeScale = 1;
            //PlayerStatsScript.instance.pause = true;
            tutoPanels[currentWaveNumber].panelRef.SetActive(true);
            tutoPanels[currentWaveNumber].turretButton.SetActive(true);
            tutoPanels[currentWaveNumber].HasBeenShown = true;
        }
    }

    public void ClosePanel()
    {
        GameObject introPanel = tutoPanels[SpawnerScript.instance.currentWaveNumber].panelRef;
        Animator introPanelAnimator = introPanel.GetComponent<Animator>();
        introPanelAnimator.Play("FadeOut");
        //PlayerStatsScript.instance.pause = false;
    }

    public void UnpauseGame()
    {
        PlayerStatsScript.instance.PauseGame(false);
    }

    [System.Serializable]
    public class TutoPanel
    {
        public string name;
        public int waveWhenShowedNumber;
        public GameObject panelRef;
        public GameObject turretButton;
        private bool hasBeenShown = false;
        public bool HasBeenShown { get => hasBeenShown; set => hasBeenShown = value; }
    }
}
