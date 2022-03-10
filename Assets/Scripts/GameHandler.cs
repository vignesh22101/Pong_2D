using System.Collections;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    #region Variables
    internal static GameHandler instance;
    internal bool isBallMoving;
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
                LevelFinished();
            }
        }
    }

    [SerializeField] Transform bricksParentTransform;
    [SerializeField] Ball ball;
    [SerializeField] int playerLives, ballCount;

    private int brickCount;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        brickCount = bricksParentTransform.childCount;
        StartCoroutine(WaitForInitialTap_Routine());
    }

    private void Respawn_Ball()
    {
        if (--playerLives > 0)
        {
            ballCount++;
            ball.Respawn();
            StartCoroutine(WaitForInitialTap_Routine());
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        print("GameOver");
    }

    private void LevelFinished()
    {
        print("LevelFinished");
    }

    private void Retry()
    {
        //reload the scene here
    }

    private IEnumerator WaitForInitialTap_Routine() //Tap to start => ball starts moving from the player 
    {
        int touchCount = 0;

        while (true)
        {
            touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isBallMoving = true;
                    ball.StartMoving();
                    yield break;
                }
            }

            yield return null;
        }
    }
}
