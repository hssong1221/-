using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour
{
    public void OnClickNewBtn()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.LoadData();
        PreCalculateAffectionModule();
        SceneManager.LoadScene("Game");
    }

    public void OnClickLoadBtn()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickGalleryBtn()
    {
        Debug.Log("���� ������");
    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }

    // _interact �̸� ��� �ٽ�
    public void PreCalculateAffectionModule()
    {
        Waifu.Instance.Interact_Init();
        AffectionTwt.Instance.Interact_Init();
        AffectionPat.Instance.Interact_Init();
        AffectionDate.Instance.Interact_Init();
    }
}
