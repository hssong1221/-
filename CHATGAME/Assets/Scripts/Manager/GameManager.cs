using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<GameManager>();
                singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                DontDestroyOnLoad(singletonObject);
                SingletonManager.Instance.RegisterSingleton(_instance);
            }
            return _instance;
        }
    }

    public PlayerData playerData;

    /*private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TempAction();
        }
    }*/

    public enum Language
    {
        Kor,
        Eng,
        China,
    }
    public Language language;

    public static Action CheckProgAction;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this as GameManager;
            DontDestroyOnLoad(gameObject);
            SingletonManager.Instance.RegisterSingleton(_instance);
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("PlayerData"))
            LoadData();
        else
            playerData = new PlayerData();
    }

    void Start()
    {
        CheckProgAction += CheckProgress;
    }

    // 호감도 시스템이랑 연동 될 곳
    public void CheckProgress()
    {
        //Debug.Log("progress check");
    }

    #region Language

    public void SetLanguage(Language language)
    {
        this.language = language;
    }

    public string GetText(Dictionary<string, string> data)
    {
        string selecter;
        switch (language)
        {
            case Language.Kor:
                selecter = "text";
                break;
            case Language.Eng:
                selecter = "entext";
                break;
            case Language.China:
                selecter = "china";
                break;
            default:
                selecter = "text";
                break;
        }
        if (data.TryGetValue(selecter, out var result))
            return result;
        else
            return "...";
    }

    #endregion

    #region Data SAVE LOAD

    public void SaveData(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    public PlayerData LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerData", "{}");
        playerData = JsonConvert.DeserializeObject<PlayerData>(json);
        return playerData;
    }

#if UNITY_EDITOR
    [MenuItem("MyTools/Delete PlayerPrefs")]
    private static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("EXECUTE");
    }
#endif

    #endregion

}
