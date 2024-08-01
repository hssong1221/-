using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


    private int _aff_idx = 0;//DialogueSheet �� Idx
    public int aff_idx
    {
        get { return _aff_idx; }
        set { _aff_idx = value; }
    } 

    public int affection_exp;//ȣ���� ����ġ
    public int affection_lv;//ȣ���� ����
    public int affection_barrel;//ȣ���� ������ �ʿ� ����ġ
    public string affection_status;//ȣ���� ����( intruder, suspicious, member, intimate, more, boyfriend )
    public string affection_restore;//�������� �޾ƿ� ȣ������ ����
    DataManager dataManager;
    SheetData affSheet;

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
    }

    void Start()
    {
        dataManager = SingletonManager.Instance.GetSingleton<DataManager>();
        //affSheet = dataManager.GetSheetData("Dialogue");

        aff_idx = 0;

        affection_barrel = 5;

        SheetLoadAction += SetSheetData;

        Affection_compare();
    }

    public void SetSheetData()
    {
        affSheet = dataManager.GetSheetData("Dialogue");
        Affection_compare();
    }

    public void Affection_ascend()
    {
        affection_exp++;
        Affection_level_calculate();
        Affection_compare();
    }

    public void Affection_descend()
    {
        if( affection_exp > 0 )
        {
            affection_exp--; 
        }
        Affection_level_calculate();
        Affection_compare();
    }

    public void Affection_level_calculate()
    {
        if(affection_exp >= affection_barrel)
        {
            affection_lv++;
            affection_barrel = affection_barrel * 2 - (affection_lv % 5);//�ӽ�
            //affection_exp = 0;
        }
    }

    public string Affection_transport()//UI�� ȣ���� ��ġ�� ������
    {
        return affection_lv.ToString();
    }

    public string Affection_compare()
    {
        if (affSheet == null)
            return "ERROR";

        var data = affSheet.GetData(_aff_idx);

        if(data == null )
        {
            return "empty";//�ӽ�
        }

        if(data.TryGetValue("affection",out var aff))
        {
            affection_restore = aff.ToString();
        }

        if (affection_lv < int.Parse(affection_restore) )//excel ���Ͽ��� ȣ���� ��θ� �ҷ��� ����
        {
            //intruder
            affection_status = "Intruder";
        }
        else if (affection_lv == int.Parse(affection_restore))
        {
            //member
            affection_status = "Suspicious";
        }
        else if (affection_lv > int.Parse(affection_restore))
        {
            //suspicious
            affection_status = "Member";
        }

        return affection_status;
    }
}
