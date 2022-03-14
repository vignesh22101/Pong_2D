using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes_Handler : MonoBehaviour
{
    #region Variables
    internal static Scenes_Handler instance;

    [SerializeField] private int homeScene_BuildIndex, firstLevel_BuildIndex, lastLevel_BuildIndex;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Load_HomeScene();
    }

    private void AllLevels_Finished()
    {
        PanelsHandler.instance.SetPanel(PanelTypes.AllLevelsComplete_Panel);
    }

    private bool IsLevel_Exists(int targetScene_BuildIndex)
    {
        return targetScene_BuildIndex <= lastLevel_BuildIndex;
    }

    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    internal void Load_NextLevel()
    {
        int nextLevel_BuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (IsLevel_Exists(nextLevel_BuildIndex))
            LoadScene(nextLevel_BuildIndex);
        else
            AllLevels_Finished();
    }

    internal void LoadLevel(int level_Index)
    {
        LoadScene(level_Index + firstLevel_BuildIndex - 1);
    }

    internal void Load_HomeScene()
    {
        LoadScene(homeScene_BuildIndex);
    }

    internal void Reload_CurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal int Get_CurrentLevel()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        return (currentBuildIndex - firstLevel_BuildIndex) + 1;
    }
}
