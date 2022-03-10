using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brick : MonoBehaviour
{
    #region Variables
    private SpriteRenderer spriteRenderer;

    [SerializeField]private int health;
    #endregion

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    internal void DamageOccured(int damage)
    {
        health -= damage;
        spriteRenderer.color = ColorGradientHandler.instance.GetColor(health);
        
        if (health == 0)
        {
            Destroy(gameObject);
            //IMplement Particle effects Here
        }
    }

    private void OnDestroy()
    {
        GameHandler.instance.BrickCount--;
    }
}
