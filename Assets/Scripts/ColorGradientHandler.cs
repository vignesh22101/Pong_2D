using UnityEngine;

public class ColorGradientHandler : MonoBehaviour
{
    #region Variables
    internal static ColorGradientHandler instance;

    [SerializeField] Gradient brickGradient;
    [SerializeField] int maxHealth;//this is maximum health that a brick can ever get
    #endregion

    private void Awake()
    {
        instance = this;
    }

    internal Color GetColor(int currentHealth)
    {
        return brickGradient.Evaluate(currentHealth / (float)maxHealth);
    }
}
