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
    public int aff_poke_event_idx
    {
        get { return _aff_poke_event_idx; }
        set { _aff_poke_event_idx = value; }
    }

    private int _interact_path_number;//��ȣ�ۿ�� ��ȯ�� �ؽ�Ʈ, �̹��� ��ġ ��ȣ
    public int interact_path_number
    {
        get { return _interact_path_number; }
        set { _interact_path_number = value; }
    }

    public int affection_exp;//ȣ���� ����ġ
    public int affection_lv;//ȣ���� ����
    public List<int> affection_barrel = new List<int>();//ȣ���� ������ �ʿ� ����ġ
    private List<int> affection_interact = new List<int>();//��ȣ�ۿ� �ε��� ����
    private List<int> twt_interact = new List<int>();
    private List<int> pat_interact = new List<int>();
    // ------------------------------------------------------------- event �ӽ÷� 1�� �ٲ���� ���� �ٲٱ� -----------------------------------------------
    public Dictionary<string, int> affection_increase = new Dictionary<string, int>() { { "Poke", 1 }, { "Event", 1 }, { "Twt", 2 }, { "Pat", 2 }, { "Date", 2 } };//category ������ ���� ����ġ
    
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
    
    public string category_restore;
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
        aff_poke_event_idx = 0;
        _interact_path_number = 0;
        int _cnt = 0;

        while ( _cnt < 6)
        {
            //affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"] + Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Poke") * affection_increase["Poke"]);
            affection_barrel.Add(Affection_sheet(_cnt, "Event") * affection_increase["Event"]);
            _cnt++;
        }
    }

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

    public void Affection_ascend()
    {
        //var data = dialogueData[_aff_poke_event_idx];
        var data = dialogueData[_aff_poke_event_idx];
        
        if (data.TryGetValue("category", out var cate))//- dialogue datasheet Ŭ�������� ȣ���� ��θ� �ҷ��� ����
        {
            category_restore = cate.ToString();
        }

        affection_exp += affection_increase[category_restore];
        
        Affection_level_calculate();
    }


    public void Affection_level_calculate()
    {
        int _cnt = 1;

        if(affection_exp >= affection_barrel[affection_lv])
        {
            _interact_path_number += affection_barrel[affection_lv];
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
        int _I_P_N = _interact_path_number;
        int _restore_rand = 0;

        if(affection_lv < 4 || affection_lv%2 == 1)//ȣ���� ���°� Member �̸��̰ų� category �� Event �� ���
        {
            _I_P_N += affection_exp;
        }
        else if(affection_lv >= 4 && affection_lv%2 == 0)//ȣ���� ���°� Member �̻��� ��� ������ �ߺ����� �ʴ� ��� �ε����� ������
        {
            _restore_rand = affection_interact[UnityEngine.Random.Range(0, affection_interact.Count)];
            Debug.Log("index? : "+_restore_rand);
            affection_interact.Remove(_restore_rand);
            _I_P_N += _restore_rand;
        }

        _aff_poke_event_idx = _I_P_N;
    }

    public void Affection_Twt_interaction_Path()
    {

    }

    public void Affection_Pat_interaction_Path()
    {

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

        var data = dialogueData;
        var iter = data.GetEnumerator();
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
