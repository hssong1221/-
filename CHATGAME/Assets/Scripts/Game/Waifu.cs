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


    private int _aff_poke_event_idx = 0;//DialogueSheet �� Idx
    private int _aff_twt_idx = 0;//Twtdata �� Idx
    private int _aff_pat_idx = 0;//Patdata �� Idx
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
    [SerializeField]
    private List<int> affection_interact = new List<int>();//��ȣ�ۿ� �ε��� ����
    private List<int> twt_interact = new List<int>();
    [SerializeField]
    private List<int> pat_interact = new List<int>();
    
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
    public Affection_status _affection_status;
    
    
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
        Poke_Event_Interact_Init();
        Twt_Interaction_Init();
        Pat_Interaction_Init();
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
        PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact, twt_interact, pat_interact);
        
        if(isSave)
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

    public void Affection_ascend()
    {/*
        var data = dialogueData[_aff_poke_event_idx];
        
        if (data.TryGetValue("category", out var cate))//- dialogue datasheet Ŭ�������� ȣ���� ��θ� �ҷ��� ����
        {
            category_restore = cate.ToString();
        }*/

        affection_exp += affection_increase[category_restore];
        
        Affection_level_calculate();
    }


    public void Affection_level_calculate()
    {
        int _cnt = 1;

        if(affection_exp >= affection_barrel[affection_lv])
        {
            _correction_number += affection_barrel[affection_lv];
            affection_lv++;
            affection_exp = 0;
            affection_interact.Clear();

            while(_cnt <= affection_barrel[affection_lv])
            {
                affection_interact.Add(_cnt);//������ ��� �ε����� �����ϱ� ���� �۾�
                _cnt++;
            }
        }
    }

    public string Affection_compare()
    {
        _affection_status = (Affection_status)Enum.ToObject(typeof(Affection_status), affection_lv/2);
        return _affection_status.ToString();
    }
    
    public float Affection_Percentage()//ȣ���� UI ����ġ ���� ����
    {
        string _cate_str = Check_Category();
        float aff_percent = 0;
        if(_cate_str == "Event")
        {
            aff_percent = 1.0f;
        }
        else
        {
            aff_percent = (float)affection_exp / (float)affection_barrel[affection_lv];
        }

        return aff_percent;
    }

    public void Affection_Poke_Interaction_Path()//Poke ��ȣ�ۿ� ��� ��ȣ ã��
    {
        int _I_P_N = _correction_number;
        int _restore_rand = 0;
        category_restore = "Poke";

        if (affection_lv < 4)//ȣ���� ���°� Member �̸��� ���
        {
            _I_P_N += affection_exp;
        }
        else if(affection_lv % 2 == 1)//Event �� ���
        {
            _I_P_N += affection_exp;
            category_restore = "Event";
        }
        else if(affection_lv >= 4 && affection_lv%2 == 0)//ȣ���� ���°� Member �̻��� ��� ������ �ߺ����� �ʴ� ��� �ε����� ������
        {
            _restore_rand = affection_interact[UnityEngine.Random.Range(0, affection_interact.Count)];
            affection_interact.Remove(_restore_rand);
            _I_P_N += _restore_rand;
        }

        _aff_poke_event_idx = _I_P_N;
        Interact_compare();
    }

    private void Poke_Event_Interact_Init()
    {
        int _cnt = 0;

        while( _cnt < Affection_sheet(0,"Poke"))
        {
            affection_interact.Add(_cnt);
            _cnt++;
        }
    }

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

    private void Interact_compare()
    {
        if(category_restore == "Poke" || category_restore == "Event")
        {
            _interact_idx = _aff_poke_event_idx;
        }
        else if (category_restore == "Twitter")
        {
            _interact_idx = _aff_twt_idx;
        }
        else if(category_restore == "Pat")
        {
            _interact_idx = _aff_pat_idx;
        }
    }

    public string Check_Category()//ī�װ� Ȯ��
    {
        if (dialogueData.Count == 0)
            return "Error";
        //var data = dialogueData[_aff_poke_event_idx];

        var data = dialogueData[_aff_poke_event_idx];
        
        if (data.TryGetValue("category", out var cate))
        {
            return cate.ToString();
        }
        else
        {
            return "Error";
        }
    }

    public int Affection_sheet(int _aff_level, string _category)//Ư�� ȣ���� �������� Ư�� ��ȣ�ۿ��� ��
    {
        int _aff_sheet = 0;
        List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        if(_category == "Poke" || _category == "Event")
        {
            _data = dialogueData;
        }
        else if(_category == "Twt")
        {
            _data = twtData;
        }
        else if(_category == "Pat")
        {
            _data = patData;
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
