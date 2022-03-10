using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables
    [SerializeField] bool isMainBall;//doesn't gets to destroyed
    [SerializeField] float intitialVelocity;

    private Rigidbody2D ball_Rigidbody2D;
    #endregion

    private void Start()
    {
        ball_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Brick"))
        {
            collision.gameObject.TryGetComponent<Brick>(out Brick brick);
            brick?.DamageOccured(1);
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
    }

    internal void Respawn()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    internal void StartMoving()
    {
        ball_Rigidbody2D.velocity = Vector2.one * intitialVelocity;
    }
}
