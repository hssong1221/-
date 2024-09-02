using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoManager : MonoBehaviour
{
    public Canvas logocanvas;
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
    }


    public void OnClickDel()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("ALL DELETE");
    }

}
