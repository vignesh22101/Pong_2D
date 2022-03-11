using UnityEngine;

public class Brick : MonoBehaviour
{
    #region Variables
    private SpriteRenderer spriteRenderer;
    private int health;
    #endregion

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = BrickHealthHandler.instance.GetRandomHealth();
        SetColor();
    }

    internal void DamageOccured(int damage)
    {
        health -= damage;
        SetColor();

        if (health <= 0)
        {
            Destroy(gameObject);
            //IMplement Particle effects Here
        }
    }

    private void SetColor()
    {
        spriteRenderer.color = ColorGradientHandler.instance.GetColor(health);
    }

    private void OnDestroy()
    {
        GameHandler.instance.BrickCount--;
    }
}
