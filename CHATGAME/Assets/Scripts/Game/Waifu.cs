using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

/// <summary>
/// ȣ���� �ý��� ������ ���
/// </summary>
public class Waifu : MonoBehaviour
{
    private static Waifu _Instance;

    public static Waifu Instance
    {
        get 
        {
            if( _Instance == null )
            {
                GameObject singletonObject = new GameObject();
                _Instance = singletonObject.AddComponent<Waifu>();
                singletonObject.name = typeof( Waifu ).ToString() + " (Singleton)";
                SingletonManager.Instance.RegisterSingleton(_Instance);
            }
            return _Instance;
        }
    }


    //private int _aff_twt_idx = 0;//Twtdata �� Idx
    //private int _aff_pat_idx = 0;//Patdata �� Idx
    private int _interact_idx = 0;
    public int Interact_idx
    {
        get { return _interact_idx; }
        set { _interact_idx = value; }
    }

    private int _correction_number;//��� ��ġ ����
    public int Correction_number
    {
        get { return _correction_number; }
        set { _correction_number = value; }
    }

    public int affection_exp;//ȣ���� ����ġ
    public int affection_lv;//ȣ���� ����
    public List<int> affection_barrel = new List<int>();//ȣ���� ������ �ʿ� ����ġ
    public string category_restore;//���� ī�װ�
    public List<int> affection_interact = new List<int>();//��ȣ�ۿ� �ε��� ����
    //private List<int> twt_interact = new List<int>();
    //private List<int> pat_interact = new List<int>();
    
    public Dictionary<string, int> affection_increase = new Dictionary<string, int>() { { "Poke", 1 }, { "Event", 1 }, { "Twitter", 2 }, { "Pat", 2 }, { "Date", 2 } };//category ������ ���� ����ġ
    
    public enum Affection_status
    {
        Intruder,
        Suspicious,
        Member,
        Intimate,
        More,
        Boyfriend
    }
    [Header("ȣ���� ����")]
    public Affection_status affection_status;
    
    
    DataManager dataManager;
    SheetData affSheet;

    List<Dictionary<string, string>> dialogueData = new List<Dictionary<string, string>>();
    List<Dictionary<string, string>> patData = new List<Dictionary<string, string>>();
    List<Dictionary<string, string>> twtData = new List<Dictionary<string, string>>();


    public static Action SheetLoadAction;

    void Awake()
    {
        if( _Instance == null )
        {
            _Instance = this as Waifu;
            SingletonManager.Instance.RegisterSingleton(_Instance);
            DontDestroyOnLoad(_Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        SheetLoadAction += SetSheetData;
    }

    void Start()
    {
        Interact_idx = 0;
        _correction_number = 0;
        category_restore = "Poke";
        
        int _cnt = 0;

        while ( _cnt < 6)
        {
            //affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"] + Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            _cnt++;
        }
        
        LoadPlayerData();
        Interact_Init();
        //Twt_Interaction_Init();
        //Pat_Interaction_Init();
    }

    #region EXCEL Data
    public void SetSheetData()
    {
        dataManager = SingletonManager.Instance.GetSingleton<DataManager>();
        affSheet = dataManager.GetSheetData("Dialogue");

        SheetData_Categorize();
    }

    void SheetData_Categorize()
    {
        var iter = affSheet.Data.GetEnumerator();
        while(iter.MoveNext() )
        {
            var cur = iter.Current;
            if (cur["category"].Equals("Poke") || cur["category"].Equals("Event"))
                dialogueData.Add(cur);
            else if (cur["category"].Equals("Pat"))
                patData.Add(cur);
            else if (cur["category"].Equals("Twt"))
                twtData.Add(cur);
        }
    }

    public List<Dictionary<string, string>> GetDataList(string name)
    {
        switch (name)
        {
            case "Poke":
                return dialogueData;
            case "Pat":
                return patData;
            case "Twt":
                return twtData;
            default:
                return dialogueData;
        }
    }

    #endregion

    #region Player Data

    //�÷��̾� ������ �����ϴ� �� - ������ �־�� �ϴ� ��ġ ã�Ƽ� �ֱ�
    public void CreatePlayerData(bool isSave = false)
    {
        //PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact, twt_interact, pat_interact);
        PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact, affection_interact, affection_interact);
        if (isSave)
            GameManager.Instance.SaveData(data);
    }

    // �÷��̾� ������ �޾Ƽ� �ε��ϴ� �� - ������ ���� �Ŀ� �ε������� �Ǵµ� �ʰ� �ʱ� �� ��� �ѹ� �����ִ°� ���µ� ��
    void LoadPlayerData()
    {
        PlayerData data = GameManager.Instance.LoadData();
        affection_exp = data.affection_exp;
        affection_lv = data.affection_lv;
        affection_interact = new List<int>(data.affection_interact);
    }

    #endregion

    public virtual void Affection_ascend()
    {
        
    }


    public virtual void Affection_level_calculate()
    {
        
    }


    public virtual void Interaction_Path()//Poke ��ȣ�ۿ� ��� ��ȣ ã��
    {
        
    }

    public virtual void Interact_Init()
    {
        
    }
    /*
    private void Twt_Interaction_Init()
    {
        int _cnt = 0;

        while(_cnt < Affection_sheet(2,"Twt"))
        {
            twt_interact.Add(_cnt);
            _cnt += 1;
        }
    }
    
    public void Twt_Interaction_Path(string _aff_stat)
    {
        int _restore_rand = twt_interact[UnityEngine.Random.Range(0,twt_interact.Count)];
        twt_interact.Remove(_restore_rand);
        _aff_twt_idx = _restore_rand;
        category_restore=_aff_stat;
        Interact_compare();
    }

    private void Pat_Interaction_Init()
    {
        int _cnt = 0;

        while (_cnt < Affection_sheet(3, "Pat"))
        {
            twt_interact.Add(_cnt);
            _cnt += 1;
        }
    }

    public void Pat_Interaction_Path(string _aff_stat)
    {
        int _restore_rand = pat_interact[UnityEngine.Random.Range(0, pat_interact.Count)];
        pat_interact.Remove(_restore_rand);
        _aff_pat_idx = _restore_rand;
        category_restore = _aff_stat;
        Interact_compare();
    }
    */
    public virtual void Interact_compare()
    {
        
    }

    public virtual string Check_Category()//ī�װ� Ȯ��
    {
        return "";
    }

    public virtual int Affection_sheet(int _aff_level, string _category)//Ư�� ȣ���� �������� Ư�� ��ȣ�ۿ��� ��
    {
        return -1;
    }
}
