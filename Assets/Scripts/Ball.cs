using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool isMainBall;//doesn't gets to destroyed
    [SerializeField] private float initialVelocityStrength;

    [Header("PowerUps' Lifetime")]
    [SerializeField] private float randomDamage_Lifetime;
    [SerializeField] private float ballGeneration_Lifetime;

    [Header("Powerups' Particle Systems")]
    [SerializeField] private GameObject randomDamage_PS;
    [SerializeField] private GameObject ballGeneration_PS;

    [SerializeField] private int damagePower = 1;

    private bool isMovementRepeating;//ball moves in only x axis/y axis continuously
    private bool generateBallOnCollision;//when collided with player
    private bool isDamageRandomized;
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
        if (!isMainBall) ApplyVelocity();//Generate balls moves automatically after it is instantiated

        StartCoroutine(VelocityMonitor_Routine());
        SetColor();
    }

    private void OnEnable()
    {
        Player.OnPowerup += Player_OnPowerUp;

        Initialize_InheritedPowerups();
    }

    private void OnDisable()
    {
        Player.OnPowerup -= Player_OnPowerUp;
        Player_OnPowerUp(Powerups.None);

        StopAllCoroutines();
    }

    internal void Initialize_InheritedPowerups()
    {
        if (isDamageRandomized)
            Player_OnPowerUp(Powerups.DamageRandomizer);
        if (generateBallOnCollision)
            Player_OnPowerUp(Powerups.BallGenerator);
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
        else if (power == Powerups.None)
        {
            ballGeneration_PS.SetActive(false);
            randomDamage_PS.SetActive(false);

            generateBallOnCollision = isDamageRandomized = false;

            CheckAndStopCoroutine(ballGeneration_Cor);
            CheckAndStopCoroutine(randomDamage_Cor);
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

        randomDamage_PS.SetActive(true);
        isDamageRandomized = true;

        while (true)
        {
            damagePower = Random.Range(1, BrickHealth_Handler.instance.GetRandomHealth());
            SetColor();

            waitSeconds = Random.Range(0f, 1f);
            timePassed += waitSeconds;
            yield return new WaitForSeconds(waitSeconds);

            if (timePassed >= randomDamage_Lifetime)
            {
                randomDamage_PS.SetActive(false);
                damagePower = 1;
                isDamageRandomized = false;
                SetColor();
                yield break;
            }
        }
    }

    private IEnumerator BallGeneration_Routine()
    {
        generateBallOnCollision = true;
        ballGeneration_PS.SetActive(true);
        yield return new WaitForSeconds(ballGeneration_Lifetime);
        ballGeneration_PS.SetActive(false);
        generateBallOnCollision = false;
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

    private void GenerateBall(Vector2 newPosition)
    {
        Ball ball = (Instantiate(gameObject, transform.parent)).GetComponent<Ball>();
        ball.transform.localPosition = newPosition;
        ball.isMainBall = false;
        //Passing the powerups to the generated ball
        ball.isDamageRandomized = isDamageRandomized;
        ball.generateBallOnCollision = generateBallOnCollision;
        ball.Initialize_InheritedPowerups();

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
    /// To resolve the issue, this Routine was created
    /// </summary>
    /// <returns></returns>
    private IEnumerator VelocityMonitor_Routine()
    {
        while (true)
        {
            if (ball_Rigidbody2D.velocity.x == 0 || ball_Rigidbody2D.velocity.y == 0)
                isMovementRepeating = true;

            yield return new WaitForSeconds(5f);
        }
    }

    /// <summary>
    /// Balls' color represents its damage power
    /// </summary>
    private void SetColor()
    {
        ball_SpriteRenderer.color = ColorGradient_Handler.instance.GetColor(damagePower);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Brick"))
        {
            collision.gameObject.TryGetComponent<Brick>(out Brick brick);

            if (brick != null)
                brick.DamageOccured(damagePower);
        }
        else if (collision.transform.CompareTag("Ground"))
        {
            CollidedGround();
        }
        else if (collision.transform.CompareTag("Player"))
        {
            AudioPlayer.instance.PlayOneShot(Audios.Ball_Hit);

            if (generateBallOnCollision)
            {
                GenerateBalls(1);
            }
        }

        if (isMovementRepeating)
        {
            ApplyVelocity();
            isMovementRepeating = false;
        }
    }

    private void CollidedGround()
    {
        if (!isMainBall)
        {
            Destroy(gameObject);
        }
        else
        {
            ball_Rigidbody2D.velocity = Vector2.zero;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
            Player_OnPowerUp(Powerups.None);//disabling the powerups
        }

        GameHandler.instance.BallCount--;
    }

    internal void ResetPose()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}
