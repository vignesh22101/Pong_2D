using UnityEngine;

public class ColorGradient_Handler : MonoBehaviour
{
    #region Variables
    internal static ColorGradient_Handler instance;

    [SerializeField] private Gradient brickGradient;
    [SerializeField] private int maxHealth;//this is maximum health that a brick should ever get 
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
