using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    #region Variables
    internal static GameHandler instance;
    internal bool isBallMoving;
    internal List<Brick> bricksList;

    internal int BallCount
    {
        get => ballCount;
        set
        {
            ballCount = value;

            if (value == 0)
            {
                isBallMoving = false;
                Invoke("Respawn_Ball", 0.3f);
            }
        }
    }

    [SerializeField] internal bool randomize_BrickHealth = false;
    [SerializeField] internal int playerLives;

    internal bool isLevelComplete;

    [SerializeField] private Transform bricksParentTransform;
    [SerializeField] private GameObject brickDeath_PS_Prefab;

    private Player player;
    private Ball ball;
    private int ballCount = 1;
    private GameObject tapToContinue_Panel;
    #endregion

    private void Awake()
    {
        instance = this;
        tapToContinue_Panel = PanelsHandler.instance.tapToContinue_Panel;
    }

    private void Start()
    {
        AdManager.instance.Pre_LoadAds();

        Time.timeScale = 1f;
        GameObject.Find("Ball").TryGetComponent<Ball>(out ball);
        GameObject.Find("Player").TryGetComponent<Player>(out player);
        bricksList = bricksParentTransform.GetComponentsInChildren<Brick>().ToList();
        tapToContinue_Panel.SetActive(false);
        PanelsHandler.instance.instructionPanel.SetActive(!PlayerPrefs.HasKey("FinishedLevel_Max"));//instruction panel will get being enabled until user finishes first level

        UpdateLives_UI();
        StartCoroutine(WaitForInitialTap_Routine());
        StartCoroutine(SpawnPowerups_Routine());
    }

    private IEnumerator WaitForInitialTap_Routine() //Tap to start => ball starts moving from the player 
    {
        int touchCount = 0;

        while (PanelsHandler.instance.instructionPanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);
        tapToContinue_Panel.SetActive(true);
        PanelsHandler.instance.Modify_PauseBtn(false);

        while (true)
        {
            touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ApplyVelocity_Ball();
                    yield break;
                }
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                ApplyVelocity_Ball();
                yield break;
            }

            yield return null;
        }

        void ApplyVelocity_Ball()
        {
            print("applying velocity to ball");
            isBallMoving = true;
            ball.ApplyVelocity();
            tapToContinue_Panel.SetActive(false);
            PanelsHandler.instance.Modify_PauseBtn(true);
        }
    }

    private IEnumerator SpawnPowerups_Routine()
    {
        while (!isBallMoving)
        {
            yield return new WaitForSeconds(0.1f);
        }

        while (true)
        {
            yield return new WaitForSeconds(8f);

            if (isBallMoving)
            {
                //let's spawn some powerups to help the user
                Powerups_Handler.instance.Spawn_RandomPowerup();
            }
        }
    }

    private void Respawn_Ball()
    {
        if (--playerLives > 0)
        {
            ballCount++;
            ball.ResetPose();
            StartCoroutine(WaitForInitialTap_Routine());
        }
        else
        {
            GameOver();
        }

        UpdateLives_UI();
    }

    private void UpdateLives_UI()
    {
        LifePanel_Handler._transform.gameObject.SetActive(true);
        LifePanel_Handler.UpdateLives_UI(playerLives);
    }

    private void Enable_BrickDeathPS(Brick brick)
    {
        GameObject brickDeath_PS = Instantiate(brickDeath_PS_Prefab, null);
        brickDeath_PS.transform.position = brick.transform.position;

        ParticleSystem.MainModule mainModule = brickDeath_PS.GetComponent<ParticleSystem>().main;
        mainModule.startColor = brick.GetComponent<SpriteRenderer>().color;

        Destroy(brickDeath_PS, 1);
    }

    internal void RemoveBrick(Brick brick)
    {
        Enable_BrickDeathPS(brick);

        if (bricksList.Contains(brick))
            bricksList.Remove(brick);

        if (bricksList.Count == 0)
        {
            isLevelComplete = true;
            Invoke("LevelCompleted", 1f);
        }
    }

    private void GameOver()
    {
        if (!isLevelComplete)
        {
            AdManager.instance.ShowAds(show_BannerAd: true, show_InterstitialAd: true);
            Time.timeScale = 0f;
            player.GetComponent<Collider2D>().enabled = false;
            PanelsHandler.instance.SetPanel(PanelTypes.Gameover_Panel);
        }
    }

    //Invoked function
    private void LevelCompleted()
    {
        AdManager.instance.ShowAds(show_BannerAd: true, show_InterstitialAd: true);
        Time.timeScale = 0f;

        PanelsHandler.instance.SetPanel(PanelTypes.LevelComplete_Panel);

        int finishedLevel_Max = PlayerPrefs.GetInt("FinishedLevel_Max", 0);
        int currentLevel = Scenes_Handler.instance.Get_CurrentLevel();

        if (finishedLevel_Max < currentLevel)
            PlayerPrefs.SetInt("FinishedLevel_Max", currentLevel);
    }

    private void OnDestroy()
    {
        AdManager.instance.DestroyAds();
        PanelsHandler.instance.Modify_PauseBtn(false);
        tapToContinue_Panel.SetActive(false);
        LifePanel_Handler._transform.gameObject.SetActive(false);
    }
}
