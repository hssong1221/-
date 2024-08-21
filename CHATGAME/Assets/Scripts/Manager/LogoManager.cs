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
}
