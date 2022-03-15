using UnityEngine;

/// <summary>
/// This script adjusts the borders in a way that borders surround the safeArea
/// </summary>
public class BordersHandler : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform topBorder, bottomBorder, leftBorder, rightBorder;

    [Tooltip("Determines distance between borders and safeArea")]
    [SerializeField] [Range(2f, 10f)] private float borderSensitivity;
    #endregion

    private void Start()
    {
        InitializeBorders();
    }

    private void InitializeBorders()
    {
        float adjustmentFactor_Scale;

        adjustmentFactor_Scale = leftBorder.transform.localScale.x / borderSensitivity;
        Vector3 leftEnd = ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        AdjustBorder(leftBorder, (Vector3.left * adjustmentFactor_Scale) + leftEnd);

        adjustmentFactor_Scale = rightBorder.transform.localScale.x / borderSensitivity;
        Vector3 rightEnd = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        AdjustBorder(rightBorder, (Vector3.right * adjustmentFactor_Scale) + rightEnd);

        adjustmentFactor_Scale = topBorder.transform.localScale.y / borderSensitivity;
        Vector3 upperEnd = ViewportToWorldPoint(new Vector3(0.5f, Screen.safeArea.height / Screen.height, 0f));
        AdjustBorder(topBorder, (Vector3.up * adjustmentFactor_Scale) + upperEnd);

        adjustmentFactor_Scale = bottomBorder.transform.localScale.y / borderSensitivity;
        Vector3 lowerEnd = ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f));
        AdjustBorder(bottomBorder, (Vector3.down * adjustmentFactor_Scale) + lowerEnd);
    }

    private void AdjustBorder(Transform border, Vector3 targetPos)
    {
        targetPos.z = 0f;
        border.transform.position = targetPos;
    }

    private Vector3 ViewportToWorldPoint(Vector3 vectorToConvert)
    {
        return Camera.main.ViewportToWorldPoint(vectorToConvert);
    }
}
