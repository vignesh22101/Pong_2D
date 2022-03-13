using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private void Start()
    {
        Apply_SafeArea();
    }

    private void Apply_SafeArea()
    {
        Rect safeArea = Screen.safeArea;
        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMax = new Vector2(safeArea.width / Screen.width, safeArea.height / Screen.height);
    }
}
