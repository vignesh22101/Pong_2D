using UnityEngine;

public class Brick : MonoBehaviour
{
    #region Variables
    private SpriteRenderer spriteRenderer;

    [SerializeField] private int health;
    #endregion

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GameHandler.instance.randomize_BrickHealth)
            health = BrickHealth_Handler.instance.GetRandomHealth();

        SetColor();
    }

    internal void DamageOccured(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            AudioPlayer.instance.PlayOneShot(Audios.Brick_Death);

            GameHandler.instance.RemoveBrick(this);

            EnablePowerup();
            Destroy(gameObject);
        }
        else
        {
            SetColor();
            AudioPlayer.instance.PlayOneShot(Audios.Ball_Hit);
        }
    }

    private void EnablePowerup()
    {
        if (transform.childCount > 0)
        {
            GameObject powerup = transform.GetChild(0).gameObject;
            powerup.transform.parent = transform.parent;
            powerup.SetActive(true);
        }
    }

    private void SetColor()
    {
        spriteRenderer.color = ColorGradient_Handler.instance.GetColor(health);
    }
}
