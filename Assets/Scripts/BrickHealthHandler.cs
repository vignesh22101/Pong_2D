using UnityEngine;

public class BrickHealthHandler : MonoBehaviour
{
    #region Variables

    internal static BrickHealthHandler instance;

    [Header("Bricks' health data")]
    [SerializeField] [Range(1, 100)] int minHealth;
    [SerializeField] [Range(1, 100)] int maxHealth;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    internal int GetRandomHealth()
    {
        return Random.Range(minHealth, maxHealth);
    }
}
