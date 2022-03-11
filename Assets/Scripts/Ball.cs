using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables
    [SerializeField] bool isMainBall;//doesn't gets to destroyed
    [SerializeField] float initialVelocityStrength;

    [Header("PowerUps' Lifetime")]
    [SerializeField] float randomDamage_Lifetime;
    [SerializeField] float ballGeneration_Lifetime;

    [SerializeField] int damagePower = 1;

    private bool isMovementRepeating;//ball moves in only x axis/y axis continuously
    private bool generateBallOnCollision;//when collided with player
    private Rigidbody2D ball_Rigidbody2D;
    private SpriteRenderer ball_SpriteRenderer;
    private Coroutine randomDamage_Cor, ballGeneration_Cor;
    #endregion

    private void Awake()
    {
        ball_Rigidbody2D = GetComponent<Rigidbody2D>();
        ball_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (!isMainBall) ApplyVelocity();

        StartCoroutine(VelocityMonitor_Routine());
        SetColor();
    }

    private void OnEnable()
    {
        Player.OnPowerup += Player_OnPowerUp;
    }

    private void OnDisable()
    {
        Player.OnPowerup -= Player_OnPowerUp;
        StopAllCoroutines();
    }

    #region Powerups
    private void Player_OnPowerUp(Powerups power)
    {
        if (power == Powerups.MultiBall)
        {
            GenerateBalls(3);
        }
        else if (power == Powerups.BallGenerator)
        {
            CheckAndStopCoroutine(ballGeneration_Cor);
            ballGeneration_Cor = StartCoroutine(BallGeneration_Routine());
        }
        else if (power == Powerups.DamageRandomizer)
        {
            CheckAndStopCoroutine(randomDamage_Cor);
            randomDamage_Cor = StartCoroutine(RandomizeDamage_Routine());
        }

        void CheckAndStopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
    }

    private IEnumerator RandomizeDamage_Routine()
    {
        float waitSeconds, timePassed = 0f;

        while (true)
        {
            damagePower = Random.Range(1, BrickHealthHandler.instance.GetRandomHealth());
            SetColor();

            waitSeconds = Random.Range(0f, 1f);
            timePassed += waitSeconds;
            yield return new WaitForSeconds(waitSeconds);

            if (timePassed >= randomDamage_Lifetime)
            {
                damagePower = 1;
                SetColor();
                yield break;
            }
        }
    }

    private IEnumerator BallGeneration_Routine()
    {
        while (true)
        {
            generateBallOnCollision = true;
            yield return new WaitForSeconds(ballGeneration_Lifetime);
            generateBallOnCollision = false;
        }
    }

    private void GenerateBalls(int ballCount)
    {
        Vector2 leftPos, rightPos;
        leftPos = rightPos = transform.localPosition;

        while (ballCount > 0)
        {
            MovePositions();

            GenerateBall(leftPos);
            ballCount--;

            if (ballCount > 0)
            {
                GenerateBall(rightPos);
                ballCount--;
            }
        }

        void MovePositions()
        {
            leftPos += Vector2.left * transform.localScale.x / 1.5f;
            rightPos += Vector2.right * transform.localScale.x / 1.5f;
        }
    }

    private void GenerateBall(Vector2 position)
    {
        Ball ball = (Instantiate(gameObject, transform.parent)).GetComponent<Ball>();
        ball.transform.localPosition = position;
        ball.isMainBall = false;

        ball.ApplyVelocity();
        GameHandler.instance.BallCount++;
    }
    #endregion 

    internal void ApplyVelocity()
    {
        ball_Rigidbody2D.velocity = Vector2.one * initialVelocityStrength;
    }

    /// <summary>
    /// Sometimes the ball moves in a straight line => x or y axis
    /// This movement gets repeated over and over even after number of collisions with bounds/player
    /// To resolve this Routine was created
    /// </summary>
    /// <returns></returns>
    private IEnumerator VelocityMonitor_Routine()
    {
        while (true)
        {
            if (ball_Rigidbody2D.velocity.x == 0 || ball_Rigidbody2D.velocity.y == 0)
            {
                isMovementRepeating = true;
            }

            print($"velocity:{ball_Rigidbody2D.velocity}");
            yield return new WaitForSeconds(5f);
        }
    }


    private void SetColor()
    {
        ball_SpriteRenderer.color = ColorGradientHandler.instance.GetColor(damagePower);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Brick"))
        {
            collision.gameObject.TryGetComponent<Brick>(out Brick brick);
            if (brick != null) brick.DamageOccured(damagePower);

            ApplyVelocity();
        }
        else if (collision.transform.CompareTag("Ground"))
        {
            if (!isMainBall)
            {
                Destroy(gameObject);
            }
            else
            {
                ball_Rigidbody2D.velocity = Vector2.zero;
                transform.localScale = Vector3.zero;
            }

            GameHandler.instance.BallCount--;
        }
        else if (collision.transform.CompareTag("Player"))
        {
            if (generateBallOnCollision)
            {
                //Generate a ball here
                GenerateBalls(1);
            }
        }

        if (isMovementRepeating)
        {
            ApplyVelocity();
            isMovementRepeating = false;
        }
    }

    internal void ResetPose()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}
