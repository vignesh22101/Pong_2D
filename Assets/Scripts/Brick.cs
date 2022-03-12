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
        health = BrickHealth_Handler.instance.GetRandomHealth();

        SetColor();
    }

    internal void DamageOccured(int damage)
    {
        health -= damage;
        SetColor();

        if (health <= 0)
        {
            AudioPlayer.instance.PlayOneShot(Audios.Brick_Death);
            GameHandler.instance.BrickCount--;

            EnablePowerup();
            Destroy(gameObject);
            //IMplement Particle effects Here
        }
        else
        {
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
