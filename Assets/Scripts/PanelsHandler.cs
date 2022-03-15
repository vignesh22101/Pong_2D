using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PanelTypes { Gameover_Panel, Pause_Panel, LevelComplete_Panel, SettingsPanel, AllLevelsComplete_Panel };

public class PanelsHandler : MonoBehaviour
{
    #region Variables
    internal static PanelsHandler instance;

    [SerializeField] internal GameObject instructionPanel;

    [SerializeField] private GameObject pauseBtn, panel, headerBG;
    [SerializeField] private PanelData[] panelDatas;
    [SerializeField] private GameObject[] allBtns, allHeaders;
    [SerializeField] private TextMeshProUGUI levelComplete_Txt;//displayed at level completion

    [Header("Data for toggling sprites in a button")]
    [SerializeField] private Button music_Btn;
    [SerializeField] private Button SFX_Btn;
    [SerializeField] private Sprite[] music_Sprites, SFX_sprites;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instructionPanel.SetActive(false);
        Modify_PauseBtn(false);
        Clean_Panel();
    }

    #region ButtonFunctions
    public void Clicked_HomeBtn()
    {
        ModifyPanelState(false);
        Scenes_Handler.instance.Load_HomeScene();
    }

    public void Clicked_ResumeBtn()
    {
        ModifyPanelState(false);
        StartCoroutine(ResumeGame_Routine());
    }

    public void Clicked_PauseBtn()
    {
        StopAllCoroutines();
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
        ToggleSprites(music_Btn, music_Sprites);
    }

    public void Clicked_SFXBtn()
    {
        AudioPlayer.instance.Change_SFX_State();
        ToggleSprites(SFX_Btn, SFX_sprites);
    }

    public void Clicked_ClosePanelBtn()
    {
        ModifyPanelState(false);
    }

    private IEnumerator ResumeGame_Routine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }
    #endregion

    private void ToggleSprites(Button button, Sprite[] sprites)
    {
        if (button.GetComponent<Image>().sprite == sprites[0])
            button.GetComponent<Image>().sprite = sprites[1];
        else
            button.GetComponent<Image>().sprite = sprites[0];
    }

    internal void ModifyPanelState(bool targetState)
    {
        panel.SetActive(targetState);
        headerBG.SetActive(targetState);
    }

    internal void Modify_PauseBtn(bool targetState)
    {
        pauseBtn.SetActive(targetState);
    }

    internal void SetPanel(PanelTypes targetPanel)
    {
        Clean_Panel();
        ModifyPanelState(true);

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

        levelComplete_Txt.text = $"Level  {Scenes_Handler.instance.Get_CurrentLevel()} Completed";
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
