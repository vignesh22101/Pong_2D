using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    internal static AdManager instance;

    [SerializeField] BannerAdGameObject bannerAd;
    [SerializeField] InterstitialAdGameObject interstitialAd;
    [Tooltip("less than this age are considered")]
    [SerializeField] int childTreatment_Age;

    private bool load_BannerAd, load_InterstitialAd;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        // Initialize the Mobile Ads SDK.
        MobileAds.Initialize((initStatus) =>
        {
        }
        );
    }

    private void LoadRespective_Ads()
    {
        print("Load respective ads");
        DestroyAds();

        if (PlayerPrefs.GetInt("Age", 5) < childTreatment_Age)
        {
            TargetedForChildrenAds();
            return;
        }
        if (PlayerPrefs.GetInt("PersonalizedAds", 1) == 0)
        {
            Non_PersonalizedAds();
            return;
        }

        LoadPersonalizedAd(); // default will be personalized ads that the google will be serving
    }

    private void LoadPersonalizedAd()
    {
        Debug.LogWarning("PersonalizedAds");
        LoadAds();
    }

    private void TargetedForChildrenAds()
    {
        Debug.LogWarning("Children Ads");

        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
                                    .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True)
                                    .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        LoadAds();
    }

    private void Non_PersonalizedAds()
    {
        Debug.LogWarning("Non-PersonalizedAds");

        AdRequest request = new AdRequest.Builder()
         .AddExtra("npa", "1")
         .Build();
        LoadAds(request);
    }

    private void LoadAds(AdRequest request = null)
    {
        print($"Loading ads, interstitial:{load_InterstitialAd},banner:{load_BannerAd}");
        if (request == null)
        {
            if (load_BannerAd)
                bannerAd.LoadAd();
            if (load_InterstitialAd)
                interstitialAd.LoadAd();
        }
        else
        {
            if (load_BannerAd)
                bannerAd.LoadAd(request);
            if (load_InterstitialAd)
                interstitialAd.LoadAd(request);
        }

        bannerAd.Hide();
    }

    internal void ShowAds(bool show_InterstitialAd = false, bool show_BannerAd = false)
    {
        print($"Showing ads, interstitial:{show_InterstitialAd},banner:{show_BannerAd}");
        if (show_BannerAd)
            bannerAd.Show();
        if (show_InterstitialAd)
            interstitialAd.ShowIfLoaded();
    }

    internal void LoadSpecific_Ads(bool load_BannerAd = false, bool load_InterstitialAd = false)
    {
        this.load_InterstitialAd = load_InterstitialAd;
        this.load_BannerAd = load_BannerAd;
        LoadRespective_Ads();
    }

    internal void DestroyAds()
    {
        print("DestroyAds");
        interstitialAd.DestroyAd();
        bannerAd.DestroyAd();
    }

    internal void Pre_LoadAds()
    {
        load_BannerAd = load_InterstitialAd = true;
        LoadRespective_Ads();
    }

    public void Loading_Ad_Failed(string reason)
    {
        print($"Loading Ad Failed,{reason}");
    }
}