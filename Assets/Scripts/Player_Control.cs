using UnityEngine;

public class Player_Control : MonoBehaviour
{
    #region Variables
    [SerializeField] private float player_Speed;

    private int touchCount;
    private int screenWidth_Half;
    private Rigidbody2D player_Rigidbody2D;
    private bool isMobileDevice = true;
    private float input;
    #endregion

    private void Awake()
    {
        player_Rigidbody2D = GetComponent<Rigidbody2D>();
        screenWidth_Half = Screen.width / 2;

        /*#if UNITY_EDITOR
                isMobileDevice = false;
        #elif UNITY_ANDROID
                isMobileDevice=true;
        #endif*/

    }

    private void Update()
    {
        if (!GameHandler.instance.isBallMoving)
        {
            player_Rigidbody2D.velocity = Vector2.zero;
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
            else
            {
                input = 0;
            }
        }
        else
        {
            print($"GetAxis:{Input.GetAxis("Horizontal")}");
            MovePlayer(Input.GetAxis("Horizontal"));
        }

        MovePlayer(input);
    }

    void MovePlayer(float direction)
    {
        player_Rigidbody2D.velocity = Vector2.right * player_Speed * direction;
    }
}
