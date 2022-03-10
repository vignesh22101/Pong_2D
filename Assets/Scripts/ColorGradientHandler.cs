using UnityEngine;

public class ColorGradientHandler : MonoBehaviour
{
    #region Variables
    internal static ColorGradientHandler instance;

    [SerializeField] Gradient healthGradient;
    [SerializeField] int maxHealth;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    internal Color GetColor(int currentHealth)
    {
        return healthGradient.Evaluate(currentHealth / (float)maxHealth);
    }
}
