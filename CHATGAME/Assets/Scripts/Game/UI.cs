using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    private static UI _instance;
    public static UI Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<UI>();
                singletonObject.name = typeof(UI).ToString() + " (Singleton)";
                SingletonManager.Instance.RegisterSingleton(_instance);
            }
            return _instance;
        }
    }

    public Image mainImg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI affectionText;
    public TextMeshProUGUI dialogueText;

    DataManager dataManager;
    Waifu waifu;

    SheetData diaSheet;
    //SheetData ImgSheet;

    public enum ButtonState
    {
        Poke,
        Twt,
        Pat,
        Next,
    }
    public ButtonState buttonState;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this as UI;
            SingletonManager.Instance.RegisterSingleton(_instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        dataManager = SingletonManager.Instance.GetSingleton<DataManager>();
        waifu = SingletonManager.Instance.GetSingleton<Waifu>();

        diaSheet = dataManager.GetSheetData("Dialogue");
        //ImgSheet = dataManager.GetSheetData("ImgPath");

        nameText.text = "name";
        affectionText.text = "0";
        dialogueText.text = "dialogue";

        DataSheetSetting(0, "Poke");

        GameManager.CheckProgAction?.Invoke();
        SetMainImg();
        SetText();

        waifu.Affection_ascend();
        waifu.aff_idx += 1;
    }

    public void SetMainImg()
    {
        /* 
         * img sheet�� ������� ��
        var pdata = ImgSheet.GetData(0);
        string imgPath = "";

        if (pdata.TryGetValue("id", out var path))
            imgPath = path;
        */

        string category = "";   // ��ư ���� + event
        string affState = "";   // ȣ���� ����
        int number = 0;         // �ؽ�Ʈ ����

        category = buttonState.ToString();        
        affState = waifu.Affection_compare();
        number = waifu.affection_exp;

        string imgPath = $"image/{category}/{affState}/{number + 1}";
        Debug.Log($"���� �̹��� ��� : {imgPath}");

        Sprite sprite = Resources.Load<Sprite>(imgPath);
        if (sprite != null)
            mainImg.sprite = sprite;
        else
            Debug.LogWarning("��ο� ���� ����");
    }


    public void SetText()
    {
        var data = diaSheet.GetData(waifu.aff_idx);
        if (data == null)
            return;

        if(data.TryGetValue("id", out var id))
            nameText.text = id;

        if (data.TryGetValue("affection", out var aff))
            affectionText.text = aff.ToString();

        if (data.TryGetValue("text", out var txt))
            dialogueText.text = txt;
    }

    public void OnClickPokeBtn()
    {
        GameManager.CheckProgAction?.Invoke();

        buttonState = ButtonState.Poke;

        SetMainImg();
        SetText();

        waifu.Affection_ascend();
        waifu.aff_idx += 1;
    }

    public void OnClickTwtBtn()
    {
        buttonState = ButtonState.Twt;

        SetMainImg();
        SetText();

        waifu.Affection_ascend();
        waifu.aff_idx += 1;
    }
    public void OnClickPatBtn()
    {
        buttonState = ButtonState.Pat;

        SetMainImg();
        SetText();

        waifu.Affection_ascend();
        waifu.aff_idx += 1;
    }
    public void OnClickNextBtn()
    {
        buttonState = ButtonState.Next;

        SetMainImg();
        SetText();

        waifu.Affection_ascend();
        waifu.aff_idx += 1;
    }


    /// <summary>
    /// �Ķ���� �޾Ƽ� �˸°� �����ͽ�Ʈ ����
    /// </summary>
    /// <param name="affection">ȣ���� ����</param>
    /// <param name="category">��ư ����</param>
    /// <returns></returns>
    public List<Dictionary<string , string>> DataSheetSetting(int affection, string category)
    {
        List<Dictionary<string, string>> partialData = new List<Dictionary<string, string>>();

        var data = diaSheet.Data;
        var iter = data.GetEnumerator();
        while(iter.MoveNext())
        {
            var cur = iter.Current;
            /*
            var iter2 = cur.GetEnumerator();
            while(iter2.MoveNext())
            {
                var cur2 = iter2.Current.Value;
                Debug.Log($"{cur2}");
            }
            */
            if (cur["affection"].Equals(affection.ToString()) && cur["category"].Equals(category))
                partialData.Add(cur);
        }

        return partialData;
    }


}
