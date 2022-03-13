using UnityEngine;

public class BrickHealth_Handler : MonoBehaviour
{
    #region Variables

    internal static BrickHealth_Handler instance;

    [Header("Bricks' health data")]
    [SerializeField] [Range(1, 10)] private int minHealth;
    [SerializeField] [Range(1, 10)] private int maxHealth;
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
