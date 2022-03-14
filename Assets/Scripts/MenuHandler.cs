using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject confirmExit_Panel, levelSelection_Panel, settingsBtn;
    #endregion

    private void Start()
    {
        confirmExit_Panel.SetActive(false);
        levelSelection_Panel.SetActive(false);
        Update_LevelSelection_Panel();
    }

    private void Update_LevelSelection_Panel()
    {
        int finishedLevel_Max = PlayerPrefs.GetInt("FinishedLevel_Max", 0);

        for (int i = 0; i < finishedLevel_Max + 1; i++)
        {
            if (i <= levelSelection_Panel.transform.childCount)
            {
                Transform childTransform = levelSelection_Panel.transform.GetChild(i);

                if (childTransform.childCount > 0) 
                    childTransform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    #region Button Functions
    public void Clicked_StartBtn()
    {
        settingsBtn.SetActive(false);
        levelSelection_Panel.SetActive(true);
    }

    public void Clicked_ExitBtn()
    {
        confirmExit_Panel.SetActive(true);
    }

    public void Clicked_Exit_Yes()
    {
        Application.Quit();
    }

    public void Clicked_Exit_No()
    {
        confirmExit_Panel.SetActive(false);
    }

    public void Clicked_SettingsBtn()
    {
        confirmExit_Panel.SetActive(false);
        PanelsHandler.instance.SetPanel(PanelTypes.SettingsPanel);
    }

    public void LoadLevel()
    {
        GameObject curr_SelectedObj = EventSystem.current.currentSelectedGameObject;
        Scenes_Handler.instance.LoadLevel(curr_SelectedObj.transform.GetSiblingIndex() + 1);
    }
    #endregion
}
