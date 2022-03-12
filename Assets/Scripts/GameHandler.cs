using System.Collections;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    #region Variables
    internal static GameHandler instance;
    internal bool isBallMoving;
    internal Brick[] bricks;
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

    internal int BrickCount
    {
        get => brickCount;
        set
        {
            brickCount = value;

            if (value == 0)
            {
                LevelCompleted();
            }
        }
    }

    [SerializeField] private Transform bricksParentTransform, lifesParentTransform;
    [SerializeField] private Ball ball;
    [SerializeField] private int playerLives;

    private bool isLevelComplete;
    private int brickCount, ballCount = 1;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bricks = bricksParentTransform.GetComponentsInChildren<Brick>();
        brickCount = bricks.Length;

        PanelsHandler.instance.Modify_PauseBtn(true);
        StartCoroutine(WaitForInitialTap_Routine());
    }

    private IEnumerator WaitForInitialTap_Routine() //Tap to start => ball starts moving from the player 
    {
        int touchCount = 0;

        yield return new WaitForSeconds(1f);

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

    private void GameOver()
    {
        if (!isLevelComplete)
        {
            isBallMoving = false;
            PanelsHandler.instance.SetPanel(PanelTypes.Gameover_Panel);
        }
    }

    private void LevelCompleted()
    {
        isLevelComplete = true;
        PanelsHandler.instance.SetPanel(PanelTypes.LevelComplete_Panel);
    }


    private void OnDestroy()
    {
        PanelsHandler.instance.Modify_PauseBtn(false);
    }
}
