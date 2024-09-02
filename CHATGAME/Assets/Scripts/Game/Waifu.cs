using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

/// <summary>
/// ȣ���� �ý��� ������ ���
/// </summary>
public class Waifu : MonoBehaviour, ICategory
{
    #region Values
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

    private int _interact_idx = 0;
    public int Interact_idx
    {
        get { return _interact_idx; }
        set { _interact_idx = value; }
    }

    private int _correction_number;
    public int Correction_number
    {
        get { return _correction_number; }
        set { _correction_number = value; }
    }

    public List<int> affection_barrel = new List<int>();/// <summary>
    /// ȣ���� ������ �ʿ� ����ġ
    /// </summary>
    public string category_restore;/// <summary>
    /// ���� ī�װ�
    /// </summary>
    public Dictionary<string, int> affection_increase = new Dictionary<string, int>() { { "Poke", 1 }, { "Event", 1 }, { "Twitter", 2 }, { "Pat", 2 }, { "Date", 2 } };/// <summary>
                                                                                                                                                                       /// category ������ ���� ����ġ
                                                                                                                                                                       /// </summary>
    private int _aff_poke_event_idx = 0;/// <summary>
    /// ȣ���� ����ġ + ���� ��ġ(Correction_number) = Interact_idx ��� ������ ���̴� ����
    /// </summary>
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

    GameManager gameManager;
    DataManager dataManager;
    SheetData affSheet;

    public List<Dictionary<string, string>> dialogueData = new List<Dictionary<string, string>>();/// <summary>
    /// poke, event ��ȣ�ۿ� ���� ����
    /// </summary>

    public Action SheetLoadAction { get; set; }
    #endregion

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
        gameManager = SingletonManager.Instance.GetSingleton<GameManager>();
        
        Interact_Init();
    }

    

    #region EXCEL Data
    public void SetSheetData()
    {
        dataManager = SingletonManager.Instance.GetSingleton<DataManager>();
        affSheet = dataManager.GetSheetData("Dialogue");

        SheetData_Categorize();
        Barrel_Init();
    }

    public void SheetData_Categorize()
    {
        var iter = affSheet.Data.GetEnumerator();
        while(iter.MoveNext() )
        {
            var cur = iter.Current;
            if (cur["category"].Equals("Poke") || cur["category"].Equals("Event"))
                dialogueData.Add(cur);
        }
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
    }

    public List<Dictionary<string, string>> GetDataList(string name)
    {
        switch (name)
        {
            case "Poke":
                return dialogueData;
            default:
                return dialogueData;
        }
    }

    #endregion

    #region Player Data

    //�÷��̾� ������ �����ϴ� �� - ������ �־�� �ϴ� ��ġ ã�Ƽ� �ֱ�
    /*
    public void CreatePlayerData(bool isSave = false)
    {
        //PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact, twt_interact, pat_interact);
        PlayerData data = new PlayerData(affection_exp, affection_lv, affection_interact);
        if (isSave)
            GameManager.Instance.SaveData(data);
    }
    */
    // �÷��̾� ������ �޾Ƽ� �ε��ϴ� �� - ������ ���� �Ŀ� �ε������� �Ǵµ� �ʰ� �ʱ� �� ��� �ѹ� �����ִ°� ���µ� ��
    /*
    void LoadPlayerData()
    {
        PlayerData data = GameManager.Instance.LoadData();
        GameManager.affection_exp = data.affection_exp;
        affection_lv = data.affection_lv;
        affection_interact = new List<int>(data.affection_interact);
    }*/

    #endregion
    public void Affection_ascend()
    {
        gameManager.affection_exp += affection_increase[category_restore];

        Affection_level_calculate();
    }

    public void Affection_level_calculate()
    {
        int _cnt = 0;

        if (gameManager.affection_exp >= affection_barrel[gameManager.affection_lv])
        {
            gameManager.Correction_number += affection_barrel[gameManager.affection_lv];
            gameManager.affection_lv++;
            gameManager.affection_exp = 0;
            gameManager.affection_interact.Clear();

            while (_cnt < affection_barrel[gameManager.affection_lv])//ȣ���� ���� ��¸��� ��ȣ�ۿ� �ε����� ������
            {
                gameManager.affection_interact.Add(_cnt);
                _cnt++;
            }
        }
    }

    public float Affection_Percentage()//ȣ���� UI ����ġ ���� ����
    {
        float aff_percent = 0;

        if (gameManager.affection_lv % 2 == 1)
        {
            aff_percent = 1.0f;
        }
        else
        {
            aff_percent = (float)gameManager.affection_exp < (float)affection_barrel[gameManager.affection_lv] ? (float)gameManager.affection_exp / (float)affection_barrel[gameManager.affection_lv] : 1f;
        }

        return aff_percent;
    }

    public string Affection_compare()//ȣ���� ���� ����
    {
        affection_status = (Affection_status)Enum.ToObject(typeof(Affection_status), gameManager.affection_lv / 2);
        return affection_status.ToString();
    }

    public int Affection_sheet(int _aff_level, string _category)//Ư�� ȣ���� �������� Ư�� ��ȣ�ۿ��� ��
    {
        int _aff_sheet = 0;
        List<Dictionary<string, string>> _data = new List<Dictionary<string, string>>();

        if (_category == "Poke" || _category == "Event")
        {
            _data = dialogueData;
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

    public string Check_Category()//ī�װ� Ȯ��
    {
        if (dialogueData.Count == 0)
            return "Error";

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

    public void Interaction_Path()//Poke ��ȣ�ۿ� ��� ��ȣ ã��
    {
        if(gameManager.affection_interact.Count <= 0)
        {
            Interact_Init();
        }

        int _I_P_N = gameManager.Correction_number;
        int _restore_rand = 0;
        category_restore = "Poke";

        if (gameManager.affection_lv < 4)//ȣ���� ���°� Member �̸��� ���
        {
            _I_P_N += gameManager.affection_exp;
        }
        else if (gameManager.affection_lv % 2 == 1)//Event �� ���
        {
            _I_P_N += gameManager.affection_exp;
            category_restore = "Event";
        }
        else if (gameManager.affection_lv >= 4 && gameManager.affection_lv % 2 == 0)//ȣ���� ���°� Member �̻��� ��� ������ �ߺ����� �ʴ� ��� �ε����� ������
        {
            _restore_rand = gameManager.affection_interact[UnityEngine.Random.Range(0, gameManager.affection_interact.Count)];
            gameManager.affection_interact.Remove(_restore_rand);
            _I_P_N += _restore_rand;
        }

        _aff_poke_event_idx = _I_P_N;
        Interact_compare();
    }

    public void Interact_Init()
    {
        if(gameManager.affection_interact.Count > 0)
        {
            return;
        }

        int _cnt = 0;

        while (_cnt < Affection_sheet(gameManager.affection_lv, "Poke"))
        {
            gameManager.affection_interact.Add(_cnt);
            _cnt++;
        }
    }

    public void Interact_compare()
    {
        if (category_restore == "Poke" || category_restore == "Event")
        {
            gameManager.Interact_idx = _aff_poke_event_idx;
        }
    }

    public int Interact_img_path()
    {
        return gameManager.Interact_idx - gameManager.Correction_number;
    }

    public int Interact_txt_path()
    {
        return gameManager.Interact_idx;
    }

    public void Sequence_Init()
    {
        return;
    }
}
