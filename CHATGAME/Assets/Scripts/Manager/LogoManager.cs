using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class LogoManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public Canvas logocanvas;
    //ads ���� test 
    public string aosGameID;
    public bool testMode = true;
    public string gameID;

    [SerializeField]
    InterstitialAdsBtn interstitialAdsBtn;
    [SerializeField]
    RewardedAdsButton rewardedAdsBtn;
    

    private void Awake()
    {
        InitialzeAds();
    }

    public void InitialzeAds()
    {
#if UNITY_EDITOR
        gameID = aosGameID;
#elif UNITY_ANDROID
        gameID = aosGameID:  
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameID, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("unity ads init complete");

        // unity ads �ʱ�ȭ ��Ų�Ŀ� ���� load����
        interstitialAdsBtn.LoadAd();
        rewardedAdsBtn.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"unity ads failed {error.ToString()} - {message}");
    }

    public void OnClickNewBtn()
    {
        PlayerPrefs.DeleteKey("PlayerData");
        GameManager.Instance.LoadData();
        PreCalculateAffectionModule();

        Invoke("StartGame", 0.5f);
    }


    public void OnClickLoadBtn()
    {
        Invoke("StartGame", 0.5f);
    }

    public void OnClickGalleryBtn()
    {
        //Debug.Log("���� ������");
        if (!PlayerPrefs.HasKey("PlayerData"))
        {
            Debug.Log("���� �÷��̾� �����Ͱ� �����ϴ� ��� alertâ�� ��� ��ȹ");
            return;
        }

        UICtrl.Instance.ShowPanel("image/UI/UI_GalleryPanel", logocanvas.transform);
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    // _interact �̸� ��� �ٽ�
    public void PreCalculateAffectionModule()
    {
        Waifu.Instance.Interact_Init();
        AffectionTwt.Instance.Interact_Init();
        AffectionPat.Instance.Interact_Init();
        AffectionDate.Instance.Interact_Init();
        Waifu.Instance.Gallery_Index_Init();
        AffectionTwt.Instance.Gallery_Index_Init();
        AffectionPat.Instance.Gallery_Index_Init();
        //AffectionDate.Instance.Gallery_Index_Init();
    }


    public void OnClickDel()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("ALL DELETE");
    }

}
