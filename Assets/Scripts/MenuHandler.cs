using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject confirmExit_Panel;
    #endregion

    private void Start()
    {
        confirmExit_Panel.SetActive(false);
    }

    #region Button Functions
    public void Clicked_StartBtn()
    {
        Scenes_Handler.instance.Load_LatestLevel();
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
    #endregion
}
