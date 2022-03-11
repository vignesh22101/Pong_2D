using UnityEngine;

enum Powerups { MultiBall, BallGenerator, DamageRandomizer, None };

public class Player : MonoBehaviour
{
    #region Variables
    internal static Player instance;

    internal delegate void PowerUpDelegate(Powerups power);
    internal static event PowerUpDelegate OnPowerup;

    [SerializeField] private float playerSpeed = 3f;

    private int touchCount;
    private int screenWidth_Half;
    private Rigidbody2D player_Rigidbody2D;
    private bool isMobileDevice;
    private float input;
    #endregion

    private void Awake()
    {
        instance = this;

        player_Rigidbody2D = GetComponent<Rigidbody2D>();
        screenWidth_Half = Screen.width / 2;
    }

    private void Start()
    {
#if UNITY_EDITOR
        isMobileDevice = false;
#elif UNITY_ANDROID
        isMobileDevice=true;
#endif
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.M))
        {
            //multiball
            OnPowerup?.Invoke(Powerups.MultiBall);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            //ball generator
            OnPowerup?.Invoke(Powerups.BallGenerator);
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            //DamageRandomizer
            OnPowerup.Invoke(Powerups.DamageRandomizer);
        }
#endif



        if (!GameHandler.instance.isBallMoving)
        {
            input = 0;
            return;
        }

        if (isMobileDevice)
        {
            touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                Touch latestTouch = Input.GetTouch(touchCount - 1);
                input = latestTouch.position.x > screenWidth_Half ? 1 : -1;
            }
        }
        else
        {
            input = Input.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        player_Rigidbody2D.velocity = input * playerSpeed * Vector2.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Powerups encounteredPowerup;

        if (collision.CompareTag("MultiBall"))
        {
            encounteredPowerup = Powerups.MultiBall;
        }
        else if (collision.CompareTag("BallGenerator"))
        {
            encounteredPowerup = Powerups.BallGenerator;
        }
        else if (collision.CompareTag("DamageRandomizer"))
        {
            encounteredPowerup = Powerups.DamageRandomizer;
        }
        else
        {
            //no powerup is encountered
            return;
        }

        //powerup is encountered
        OnPowerup?.Invoke(encounteredPowerup);
        Destroy(collision.gameObject);
    }
}
