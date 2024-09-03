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

    //public PlayerData playerData; 
    //public NonInitData nonInitData;
    
    public SoundManager soundManager;

    public enum Language
    {
        Kor,
        Eng,
        China,
    }
    public Language language;

    #region Affection Logic Var

    public int affection_exp;//ȣ���� ����ġ
    public int affection_lv;//ȣ���� ����
    public int Correction_number;/// <summary>
    /// �̹��� ��θ� Interact_idx ���� �����ϱ� ���� ��, ȣ���� ������ ���������� ���� �������� ������ �ʿ� ����ġ�� ����
    /// </summary>
    public int Interact_idx;/// <summary>
                            /// �̹��� ��ο� �ؽ�Ʈ ��� �ε���
                            /// </summary>
    public bool isDate;/// <summary>
    /// date ���� ����
    /// </summary>
    public int date_sequence;/// <summary>
    /// random date situation �� ���൵
    /// </summary>
    public List<int> affection_interact = new List<int>();/// <summary>
    /// �����ִ� poke, event ��ȣ�ۿ� �ε��� ����Ʈ
    /// </summary>
    public List<int> twt_interact = new List<int>();/// <summary>
    /// �����ִ� Twitter ��ȣ�ۿ� �ε��� ����Ʈ
    /// </summary>
    public List<int> pat_interact = new List<int>();/// <summary>
    /// �����ִ� pat ��ȣ�ۿ� �ε��� ����Ʈ
    /// </summary>
    public List<int> date_interact = new List<int>();/// <summary>
    /// �����ִ� date ��ȣ�ۿ� situation ���� �ε��� ����Ʈ
    /// </summary>
    public Dictionary<string,int> unlockBtnCnt = new Dictionary<string,int>() { { "Twitter", 0 }, { "Pat", 0 }, { "Date", 0 } };/// <summary>
                                                                                                                                /// ��ư �׼� Ȱ��ȭ ��
                                                                                                                                /// </summary>
    #endregion

    #region NON_INIT Var
    public string temp; // �ӽ� ������, ����� ���ϴ°ŷ� �ٲٱ�

    #endregion


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
    }

    void Start()
    {
        LoadData();

        /*if (PlayerPrefs.HasKey("PlayerData"))
           LoadData() 
        else
            playerData = new PlayerData();*/
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

    // GameManager�� �ִ� ������� Ŭ������ ����
    // ������ �ʿ��� �������� ��� �߰����ָ� �ȴ�.
    public class PlayerData
    {
        public int affection_exp = 0;//ȣ���� ����ġ
        public int affection_lv = 0;//ȣ���� ����
        public int Correction_number = 0;
        public int Interact_idx = 0;
        public bool isDate = false;
        public int date_sequence = 0;
        public List<int> affection_interact = new List<int>();//��ȣ�ۿ� �ε��� ����
        public List<int> twt_interact = new List<int>();
        public List<int> pat_interact = new List<int>();
        public List<int> date_interact = new List<int>();
        public Dictionary<string, int> unlockBtnCnt = new Dictionary<string, int>() { { "Twitter", 0 }, { "Pat", 0 }, { "Date", 0 } };
    }
    public class NonInitData
    {
        public string temp;
    }

    // ������ ������ ���� Ŭ�����ȿ� ���� ������ ����
    public PlayerData GetPlayerData()
    {
        return new PlayerData
        {
            affection_exp = this.affection_exp,
            affection_lv = this.affection_lv,
            Correction_number = this.Correction_number,
            Interact_idx = this.Interact_idx,
            isDate = this.isDate,
            date_sequence = this.date_sequence,
            affection_interact = this.affection_interact,
            twt_interact = this.twt_interact,
            pat_interact = this.pat_interact,
            date_interact = this.date_interact,
            unlockBtnCnt = this.unlockBtnCnt,
        };
    }

    // ������ �ε带 ���� Ŭ���� �ȿ� �ִ� ������ ������ GameManager ���� �ʱ�ȭ
    public void SetPlayerData(PlayerData data)
    {
        this.affection_exp = data.affection_exp;
        this.affection_lv = data.affection_lv;
        this.Correction_number = data.Correction_number;
        this.Interact_idx = data.Interact_idx;
        this.isDate = data.isDate;
        this.date_sequence = data.date_sequence;
        this.affection_interact = data.affection_interact;
        this.twt_interact = data.twt_interact;
        this.pat_interact = data.pat_interact;
        this.date_interact = data.date_interact;
        this.unlockBtnCnt = data.unlockBtnCnt;
    }

    public NonInitData GetNonInitData()
    {
        return new NonInitData
        {
            temp = this.temp,
        };
    }
    public void SetNonInitData(NonInitData data)
    {
        this.temp = data.temp;
    }

    // ������ ������ �Ϸ��� �θ��ÿ�
    public void SaveData()
    {
        PlayerData data = GetPlayerData();
        NonInitData data2 = GetNonInitData();
        string json = JsonConvert.SerializeObject(data);
        string json2 = JsonConvert.SerializeObject(data2);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.SetString("NonInitData", json2);
        PlayerPrefs.Save();
    }

    // ������ �ε带 �Ϸ��� �θ��ÿ�(�� ���� �� �ѹ� �θ��� �ɰ� ����)
    public void LoadData()
    {
        if (!PlayerPrefs.HasKey("PlayerData"))
        {
            PlayerData data = new PlayerData();
            SetPlayerData(data);
        }
        else
        {
            string json = PlayerPrefs.GetString("PlayerData", "{}");
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            SetPlayerData(playerData);
        }

        if(!PlayerPrefs.HasKey("NonInitData"))
        {
            NonInitData data2 = new NonInitData();
            SetNonInitData(data2);
        }
        else
        {
            string json2 = PlayerPrefs.GetString("NonInitData", "{}");
            NonInitData niData = JsonConvert.DeserializeObject<NonInitData>(json2);
            SetNonInitData(niData);
        }
    }

#if UNITY_EDITOR
    // ����� ��� - playerprefs�� ����� ������ ������ �Լ�
    [MenuItem("MyTools/Delete PlayerPrefs")]
    private static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("EXECUTE");
    }
#endif

    #endregion

}
