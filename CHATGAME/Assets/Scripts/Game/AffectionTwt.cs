using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectionTwt : MonoBehaviour, ICategory
{
    #region Values
    private static AffectionTwt _Instance;

    public static AffectionTwt Instance
    { 
        get
        {
            if (_Instance == null)
            {
                GameObject singletonObject = new GameObject();
                _Instance = singletonObject.AddComponent<AffectionTwt>();
                singletonObject.name = typeof(AffectionTwt).ToString() + " (Singleton)";
                SingletonManager.Instance.RegisterSingleton(_Instance);
            }
            return _Instance;
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
    public List<Dictionary<string, string>> twtData = new List<Dictionary<string, string>>();

    GameManager gameManager;
    DataManager dataManager;
    SheetData affSheet;

    ICategory poke_event_correct;

    public Action SheetLoadAction { get; set; }

    #endregion

    void Awake()
    {
        if(_Instance == null)
        {
            _Instance = this as AffectionTwt;
            SingletonManager.Instance.RegisterSingleton(_Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        SheetLoadAction += SetSheetData;
    }

    void Start()
    {
        StartCoroutine(DataManager.Instance.WaitDataLoading(SheetLoadAction));
        
        Interact_idx = 0;
        gameManager = SingletonManager.Instance.GetSingleton<GameManager>();
        
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

        Interact_Init();
        Barrel_Init();
        Gallery_Index_Init();
    }

    public void Barrel_Init()
    {
        int _cnt = 0;

        while (_cnt < 6)
        {
            affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            _cnt++;
        }

        affection_barrel[10] = 1;
    }

    public List<Dictionary<string , string>> GetDataList(string name)
    {
        return twtData;
    }

    public void Gallery_Index_Init()
    {
        int _cnt = 0;
        if(gameManager.twt_gallery_idx.Count <= 0)
        {
            while (_cnt < twtData.Count)
            {
                gameManager.twt_gallery_idx.Add(0);
                _cnt++;
            }
        }
    }

    #endregion

    public void Affection_ascend()
    {
        if (gameManager.affection_lv >= 10)
        {
            return;
        }
        if (gameManager.affection_lv > 4)
        {
            gameManager.affection_exp += affection_increase["Twitter"];
        }        

        Affection_level_calculate();
    }

    public void Affection_level_calculate()
    {
        poke_event_correct = SingletonManager.Instance.GetSingleton<Waifu>();

        if (gameManager.affection_exp >= affection_barrel[gameManager.affection_lv])
        {
            gameManager.Correction_number += affection_barrel[gameManager.affection_lv];
            gameManager.affection_lv++;
            gameManager.affection_exp = -1;
        }
    }

    public float Affection_Percentage()
    {
        float aff_percent = 0f;

        if(gameManager.affection_lv % 2 == 1)
        {
            aff_percent = 1.0f;
        }
        else
        {
            aff_percent = (float)gameManager.affection_exp < (float)affection_barrel[gameManager.affection_lv] ? (float)gameManager.affection_exp / (float)affection_barrel[gameManager.affection_lv] : 1f;
        }

        return aff_percent;
    }

    public string Affection_compare()
    {
        return "";
    }

    public int Affection_sheet(int _aff_level, string _category)
    {
        int _aff_sheet = 0;
        List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        if (_category == "Poke" || _category == "Event")
        {
            _data = dialogueData;
        }
        else if (_category == "Twt")
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
        if(gameManager.twt_interact.Count <= 0)//두가지 방법을 고려( 1. 기존의 상호작용을 계속해서 랜덤으로 보여준다, 2. 새로운 상호작용 업뎃이 나올때까지 기다려달라고 한다. )
        {
            Interact_Init();
        }
        int _restore_rand = gameManager.twt_interact[UnityEngine.Random.Range(0,gameManager.twt_interact.Count)];
        gameManager.twt_interact.Remove(_restore_rand);
        _interact_idx = _restore_rand;
        gameManager.twt_gallery_idx[_interact_idx] = 1;
    }

    public void Interact_Init()
    {
        if (gameManager.twt_interact.Count > 0)
        {
            return;
        }

        int _cnt = 0;

        while(_cnt < Affection_sheet(2,"Twt"))
        {
            gameManager.twt_interact.Add(_cnt);
            _cnt++;
        }
    }

    public int Interact_img_path()
    {
        return Interact_idx;
    }

    public int Interact_txt_path()
    {
        return Interact_idx;
    }

    public void Sequence_Init()
    {
        return;
    }
}
