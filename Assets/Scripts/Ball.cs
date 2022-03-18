using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool isMainBall;//doesn't gets to destroyed
    [SerializeField] private int damagePower = 1;
    [SerializeField] private float initialVelocityStrength;
    [SerializeField] [Range(0, 1f)] private float velocityAdjustment_Sensitivity;

    [Header("PowerUps' Lifetime")]
    [SerializeField] private float randomDamage_Lifetime;
    [SerializeField] private float ballGeneration_Lifetime;

    [Header("Powerups' Data")]
    [SerializeField] private GameObject ballGeneration_PS;
    [SerializeField] private Gradient normal_Gradient, damageRandomizer_Gradient;

    private bool applyVelocity;
    private bool generateBallOnCollision;//when collided with player
    private bool isDamageRandomized;
    private Rigidbody2D ball_Rigidbody2D;
    private SpriteRenderer ball_SpriteRenderer;
    private Coroutine randomDamage_Cor, ballGeneration_Cor, velocityMonitor_Cor;
    private float target_VelocityMagnitude, current_VelocityMagnitude;
    private TrailRenderer ball_TrailRenderer;
    private int initial_DamagePower;
    #endregion

    private void Awake()
    {
        ball_Rigidbody2D = GetComponent<Rigidbody2D>();
        ball_SpriteRenderer = GetComponent<SpriteRenderer>();
        ball_TrailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        if (isMainBall)
        {
            initial_DamagePower = damagePower;
            ResetPose();
        }
        else
        {
            ApplyVelocity();//Generate balls moves automatically after it is instantiated
        }

        target_VelocityMagnitude = (Vector2.one * initialVelocityStrength).magnitude;

        SetColor();
    }

    private void OnEnable()
    {
        Player.OnPowerup += Player_OnPowerUp;
    }

    private void OnDisable()
    {
        Player.OnPowerup -= Player_OnPowerUp;
        Player_OnPowerUp(Powerups.None);

        StopAllCoroutines();
    }

    #region Powerups Management
    internal void Initialize_InheritedPowerups()
    {
        if (isDamageRandomized)
            Player_OnPowerUp(Powerups.DamageRandomizer);
    }

    private void Player_OnPowerUp(Powerups power)
    {
        if (power == Powerups.MultiBall)
        {
            GenerateBalls(2);
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
            Disable_AllPowerups();
        }

        void CheckAndStopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        void Disable_AllPowerups()
        {
            Disable_BallGeneration_Powerup();
            Disable_RandomizeDamage_Powerup();

            CheckAndStopCoroutine(ballGeneration_Cor);
            CheckAndStopCoroutine(randomDamage_Cor);
        }
    }

    private void Disable_RandomizeDamage_Powerup()
    {
        damagePower = initial_DamagePower;
        isDamageRandomized = false;
        Set_TrailRenderer(false);
        SetColor();
    }

    private void Disable_BallGeneration_Powerup()
    {
        ballGeneration_PS.SetActive(false);
        generateBallOnCollision = false;
    }

    private IEnumerator RandomizeDamage_Routine()
    {
        float waitSeconds, timePassed = 0f;

        Set_TrailRenderer(true);
        isDamageRandomized = true;

        while (true)
        {
            damagePower = Random.Range(2, ColorGradient_Handler.instance.maxHealth);
            SetColor();

            waitSeconds = Random.Range(0f, 1f);
            timePassed += waitSeconds;
            yield return new WaitForSeconds(waitSeconds);

            if (timePassed >= randomDamage_Lifetime)
            {
                Disable_RandomizeDamage_Powerup();
                yield break;
            }
        }
    }

    private IEnumerator BallGeneration_Routine()
    {
        generateBallOnCollision = true;
        ballGeneration_PS.SetActive(true);
        yield return new WaitForSeconds(ballGeneration_Lifetime);
        Disable_BallGeneration_Powerup();
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
        ball.generateBallOnCollision = false;//this power isn't passed
        ball.initial_DamagePower = initial_DamagePower;
        ball.Initialize_InheritedPowerups();
        ball.Disable_BallGeneration_Powerup();

        ball.ApplyVelocity_Random();
        GameHandler.instance.BallCount++;
    }
    #endregion

    #region Velocity Management
    internal void ApplyVelocity()
    {
        ball_Rigidbody2D.velocity = Vector2.one * initialVelocityStrength;

        if (velocityMonitor_Cor != null)
            StopCoroutine(velocityMonitor_Cor);

        velocityMonitor_Cor = StartCoroutine(VelocityMonitor_Routine());
    }

    private void ApplyVelocity_Random()
    {
        ball_Rigidbody2D.velocity = new Vector2(Random.Range(-initialVelocityStrength, initialVelocityStrength), Random.Range(0, initialVelocityStrength));
    }

    /// <summary>
    /// Sometimes the ball moves in a straight line => x or y axis
    /// This movement gets repeated over and over even after number of collisions with bounds/player
    /// This Routine resolves this issue and others regarding ball movement 
    /// </summary>
    private IEnumerator VelocityMonitor_Routine()
    {
        Vector2 ball_Velocity = ball_Rigidbody2D.velocity;

        while (true)
        {
            ball_Velocity = ball_Rigidbody2D.velocity;
            current_VelocityMagnitude = ball_Velocity.magnitude;

            if (Mathf.Abs(ball_Velocity.x) == 0 || Mathf.Abs(ball_Velocity.y) == 0)
                applyVelocity = true;

            if (PercentDifference(current_VelocityMagnitude, target_VelocityMagnitude) > 10f)
            {
                ball_Rigidbody2D.velocity += ball_Velocity * velocityAdjustment_Sensitivity;
            }
            else if (PercentDifference(current_VelocityMagnitude, target_VelocityMagnitude) < -10f)
            {
                ball_Rigidbody2D.velocity -= ball_Velocity * velocityAdjustment_Sensitivity;
            }

            yield return new WaitForSeconds(0.01f);
        }

        float PercentDifference(float currVal, float targetVal)
        {
            return ((targetVal - currVal) / currVal) * 100f;
        }
    }
    #endregion

    /// <summary>
    /// Balls' color represents its damage power
    /// </summary>
    private void SetColor()
    {
        ball_SpriteRenderer.color = ColorGradient_Handler.instance.GetColor(damagePower);
    }

    private void Handle_GroundCollision()
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

    private void Set_TrailRenderer(bool set_DamageRandomizer_Gradient)
    {
        if (set_DamageRandomizer_Gradient)
            ball_TrailRenderer.colorGradient = damageRandomizer_Gradient;
        else
            ball_TrailRenderer.colorGradient = normal_Gradient;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Brick"))
        {
            applyVelocity = false;

            collision.gameObject.TryGetComponent<Brick>(out Brick brick);

            if (brick != null)
                brick.DamageOccured(damagePower);
        }
        else if (collision.transform.CompareTag("Ground"))
        {
            Handle_GroundCollision();
            applyVelocity = false;
        }
        else if (collision.transform.CompareTag("Player"))
        {
            AudioPlayer.instance.PlayOneShot(Audios.Ball_Hit);

            if (generateBallOnCollision)
            {
                Disable_BallGeneration_Powerup();
                GenerateBalls(1);
            }
        }

        if (applyVelocity)
        {
            ApplyVelocity();
            applyVelocity = false;
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