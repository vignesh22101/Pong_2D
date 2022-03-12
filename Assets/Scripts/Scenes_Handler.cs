using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes_Handler : MonoBehaviour
{
    #region Variables
    internal static Scenes_Handler instance;

    [SerializeField] private int homeScene_Index, firstLevel_Index, lastLevel_Index;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadScene(homeScene_Index);
    }

    internal void Load_NextLevel()
    {
        int next_SceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        PlayerPrefs.SetInt("Level", next_SceneIndex);

        LoadLevel_If_Available(next_SceneIndex);
    }

    private void AllLevel_Finished()
    {
        print("Congratulations, you have finished all the levels,Let's play again from the start");
        LoadScene(firstLevel_Index);
    }

    internal void Load_HomeScene()
    {
        LoadScene(homeScene_Index);
    }

    internal void Reload_CurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the latest level user has reached
    /// </summary>
    internal void Load_LatestLevel()
    {
        int latest_SceneIndex = PlayerPrefs.GetInt("Level", firstLevel_Index);

        LoadLevel_If_Available(latest_SceneIndex);
    }

    private void LoadLevel_If_Available(int targetSceneIndex)
    {
        if (targetSceneIndex <= lastLevel_Index)
            LoadScene(targetSceneIndex);
        else
            AllLevel_Finished();
    }

    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
