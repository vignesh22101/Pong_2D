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
                Respawn_Ball();
            }
        }
    }

    [SerializeField] internal bool randomize_BrickHealth = true;
    [SerializeField] internal int playerLives;

    internal bool isLevelComplete;

    [SerializeField] private Transform bricksParentTransform, lifesParentTransform;
    [SerializeField] private Ball ball;
    [SerializeField] private GameObject brickDeath_PS_Prefab;
    [SerializeField] private GameObject tapToContinue_Panel;
    [SerializeField] private Player player;

    private int ballCount = 1;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        bricksList = bricksParentTransform.GetComponentsInChildren<Brick>().ToList();

        PanelsHandler.instance.Modify_PauseBtn(true);
        StartCoroutine(WaitForInitialTap_Routine());
    }

    private IEnumerator WaitForInitialTap_Routine() //Tap to start => ball starts moving from the player 
    {
        int touchCount = 0;

        tapToContinue_Panel.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        tapToContinue_Panel.SetActive(true);

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
            isBallMoving = true;
            ball.ApplyVelocity();
            tapToContinue_Panel.SetActive(false);
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
        for (int i = 0; i < lifesParentTransform.childCount; i++)
        {
            lifesParentTransform.GetChild(i).gameObject.SetActive(i < playerLives);
        }
    }

    internal void Enable_BrickDeathPS(Vector3 instantiatePos)
    {
        GameObject brickDeath_PS = Instantiate(brickDeath_PS_Prefab, null);
        brickDeath_PS.transform.position = instantiatePos;
        Destroy(brickDeath_PS, 1);
    }

    internal void RemoveBrick(Brick brick)
    {
        if (bricksList.Contains(brick)) bricksList.Remove(brick);

        if (bricksList.Count == 0) GameOver();
    }

    private void GameOver()
    {
        if (!isLevelComplete)
        {
            Time.timeScale = 0f;
            player.GetComponent<Collider2D>().enabled = false;
            PanelsHandler.instance.SetPanel(PanelTypes.Gameover_Panel);
        }
    }

    private void LevelCompleted()
    {
        Time.timeScale = 0f;
        isLevelComplete = true;
        PanelsHandler.instance.SetPanel(PanelTypes.LevelComplete_Panel);
    }


    private void OnDestroy()
    {
        PanelsHandler.instance.Modify_PauseBtn(false);
    }
}
