using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [Header("���� �׼� Ƚ��")]
    public int adsNum;

    [SerializeField]
    private string aosGameID;
    [SerializeField]
    private string iosGameID; // Ios�� ���忡�� ������
    public string gameID;
    public bool testMode = true;

    public bool isAdInit { get; private set; } = false;

    /*[SerializeField]
    InterstitialAdsBtn interstitialAdsBtn;
    [SerializeField]
    RewardedAdsButton rewardedAdsBtn;*/

    void Awake()
    {
        InitialzeAds();
    }

    public void InitialzeAds()
    {
#if UNITY_EDITOR
        gameID = aosGameID;
#elif UNITY_ANDROID
        gameID = aosGameID;
#elif UNITY_IOS
        gameID = iosGameID;  
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameID, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("unity ads init complete");

        isAdInit = true;

        // unity ads �ʱ�ȭ ��Ų�Ŀ� ���� load����
        /*interstitialAdsBtn.LoadAd();
        rewardedAdsBtn.LoadAd();*/
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"unity ads failed {error.ToString()} - {message}");

        Invoke(nameof(InitialzeAds), 1f);
    }
}
