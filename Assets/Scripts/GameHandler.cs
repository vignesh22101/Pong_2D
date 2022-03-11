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
    [SerializeField] int playerLives;

    private int brickCount, ballCount = 1;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        brickCount = bricksParentTransform.GetComponentsInChildren<Brick>().Length;
        StartCoroutine(WaitForInitialTap_Routine());
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
}
