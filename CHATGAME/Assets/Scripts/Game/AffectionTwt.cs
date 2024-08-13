using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectionTwt : MonoBehaviour, ICategory
{
    private static AffectionTwt _instance;

    public static AffectionTwt Instance
    {
        get 
        {
            if( _instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<AffectionTwt>();
                singletonObject.name = typeof(AffectionTwt).ToString() + "(Sington)";
                SingletonManager.Instance.RegisterSingleton(_instance);
            }
            return _instance;
        }
    }

    private int _interact_idx = 0;
    public int Interact_idx
    {
        get { return _interact_idx; }
        set { _interact_idx = value; }
    }
    public int Correction_number { get; set; }
    public List<int> affection_barrel = new List<int>();
    public Dictionary<string, int> affection_increase = new Dictionary<string, int>() { { "Poke", 1 }, { "Event", 1 }, { "Twitter", 2 }, { "Pat", 2 }, { "Date", 2 } };//category 종류별 제공 경험치
    List<Dictionary<string, string>> dialogueData = new List<Dictionary<string, string>>();
    List<Dictionary<string, string>> twtData = new List<Dictionary<string, string>>();

    GameManager gameManager;
    DataManager dataManager;
    SheetData affSheet;

    public Action SheetLoadAction { get; set; }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this as AffectionTwt;
            SingletonManager.Instance.RegisterSingleton(_instance);
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(gameObject);
        }

        SheetLoadAction += SetSheetData;
        SheetLoadAction?.Invoke();
    }

    void Start()
    {
        Interact_idx = 0;
        gameManager = SingletonManager.Instance.GetSingleton<GameManager>();

        int _cnt = 0;

        while (_cnt < 6)
        {
            affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            _cnt++;
        }
    }

    #region EXCEL Data
    public void SetSheetData()
    {
        dataManager = SingletonManager.Instance.GetSingleton<DataManager>();
        affSheet = dataManager.GetSheetData("Dialogue");

        SheetData_Categorize();
    }

    public void SheetData_Categorize()
    {
        var iter = affSheet.Data.GetEnumerator();
        while (iter.MoveNext())
        {
            var cur = iter.Current;
            if (cur["category"].Equals("Twt"))
            {
                twtData.Add(cur);
            }
            else if (cur["category"].Equals("Poke") || cur["category"].Equals("Event"))
            {
                dialogueData.Add(cur);
            }
                
        }
    }

    public List<Dictionary<string , string>> GetDataList(string name)
    {
        return twtData;
    }

    #endregion

    #region Player Data
    /*
    public void CreatePlayerData(bool isSave = false)
    {
        PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact, affection_interact, affection_interact);
        if (isSave)
        {
            GameManager.Instance.SaveData(data);
        }
    }*/
    #endregion

    public void Affection_ascend()
    {
        gameManager.affection_exp += affection_increase["Twitter"];

        Affection_level_calculate();
    }

    public void Affection_level_calculate()
    {
        if(gameManager.affection_exp >= affection_barrel[gameManager.affection_lv])
        {
            gameManager.affection_lv++;
            gameManager.affection_exp = 0;
        }
    }

    public string Check_Category()
    {
        if(twtData.Count == 0)
        {
            return "Error";
        }

        var data = twtData[_interact_idx];

        if(data.TryGetValue("category", out var cate))
        {
            return cate.ToString();
        }
        else
        {
            return "Error";
        }
    }

    public void Interaction_Path()
    {
        int _restore_rand = gameManager.twt_interact[UnityEngine.Random.Range(0,gameManager.twt_interact.Count)];
        gameManager.twt_interact.Remove(_restore_rand);
        _interact_idx = _restore_rand;
    }

    public void Interact_Init()
    {
        int _cnt = 0;

        while(_cnt < Affection_sheet(2,"Twitter"))
        {
            gameManager.twt_interact.Add(_cnt);
            _cnt++;
        }
    }

    public float Affection_Percentage()
    {
        string _cate_str = Check_Category();
        float aff_percent = 0f;
        if (_cate_str == "Event")
        {
            aff_percent = 1.0f;
        }
        else
        {
            aff_percent = (float)gameManager.affection_exp / (float)affection_barrel[gameManager.affection_lv];
        }

        return aff_percent;
    }

    public int Affection_sheet(int _aff_level, string _category)
    {
        int _aff_sheet = 0;
        List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        if (_category == "Poke" || _category == "Event")
        {
            _data = dialogueData;
        }
        else if(_category == "Twitter")
        {
            _data = twtData;
        }

        var iter = _data.GetEnumerator();
        while (iter.MoveNext())
        {
            var cur = iter.Current;

            if (cur["affection"].Equals(_aff_level.ToString()) && cur["category"].Equals(_category))
            {
                _aff_sheet++;
            }
        }
        return _aff_sheet;
    }
}
