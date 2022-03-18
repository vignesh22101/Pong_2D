using UnityEngine;


public class SafeArea : MonoBehaviour
{
    public static SafeArea instance;

    [SerializeField] float bannerAdHeight;
    [SerializeField] bool is_Bottom_BannerAd;
    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Panel = GetComponent<RectTransform>();
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        Debug.Log($"safeArea:{Screen.safeArea}");
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Adjustment based on banner ad height
        float dp = bannerAdHeight * (Screen.dpi / 160f); //densityPixels

        if (is_Bottom_BannerAd)
            anchorMin.y = dp / Screen.height;
        else
            anchorMin.x = dp / Screen.height;

        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;

        /*Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);*/
    }
}
