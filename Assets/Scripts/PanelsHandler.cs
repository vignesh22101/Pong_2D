using UnityEngine;

public enum PanelTypes { Gameover_Panel, Pause_Panel, LevelComplete_Panel, SettingsPanel };

public class PanelsHandler : MonoBehaviour
{
    #region Variables
    internal static PanelsHandler instance;

    [SerializeField] private GameObject pauseBtn,panel;
    [SerializeField] private PanelData[] panelDatas;
    [SerializeField] private GameObject[] allBtns, allHeaders;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Modify_PauseBtn(false);
        Clean_Panel();
    }

    #region ButtonFunctions
    public void Clicked_HomeBtn()
    {
        Clean_Panel();
        Scenes_Handler.instance.Load_HomeScene();
    }

    public void Clicked_ResumeBtn()
    {
        ModifyPanelState(false);
        Time.timeScale = 1f;
    }

    public void Clicked_PauseBtn()
    {
        Time.timeScale = 0f;
        SetPanel(PanelTypes.Pause_Panel);
    }

    public void Clicked_NextLevelBtn()
    {
        ModifyPanelState(false);
        Scenes_Handler.instance.Load_NextLevel();
    }

    public void Clicked_RetryBtn()
    {
        ModifyPanelState(false);
        Scenes_Handler.instance.Reload_CurrentScene();
    }

    public void Clicked_MusicBtn()
    {
        AudioPlayer.instance.Change_Music_State();
    }

    public void Clicked_SFXBtn()
    {
        AudioPlayer.instance.Change_SFX_State();
    }

    public void Clicked_ClosePanelBtn()
    {
        ModifyPanelState(false);
    }
    #endregion

    internal void ModifyPanelState(bool targetState)
    {
        panel.SetActive(targetState);
    }

    internal void Modify_PauseBtn(bool targetState)
    {
        pauseBtn.SetActive(targetState);
    }

    internal void SetPanel(PanelTypes targetPanel)
    {
        Clean_Panel();
        ModifyPanelState(true);

        AudioPlayer.instance.PlayOneShot(Audios.Pop);

        foreach (var panelData in panelDatas)
        {
            if (panelData.panelType == targetPanel)
            {
                foreach (var obj in panelData.objsToEnable)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    private void Clean_Panel()
    {
        Disable_AllHeaders();
        Disable_AllBtns();
        ModifyPanelState(false);
    }

    private void Disable_AllBtns()
    {
        foreach (var btn in allBtns)
        {
            btn.SetActive(false);
        }
    }

    private void Disable_AllHeaders()
    {
        foreach (var item in allHeaders)
        {
            item.SetActive(false);
        }
    }
}

[System.Serializable]
public class PanelData
{
    public PanelTypes panelType;
    public GameObject[] objsToEnable;
}
