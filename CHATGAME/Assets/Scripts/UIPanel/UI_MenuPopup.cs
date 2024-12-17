using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Linq.Expressions;

public class UI_MenuPopup : BasePanel
{
    public Button sndBtn;
    public Button ctrlBtn;
    public Button accBtn;

    public GameObject sndPanel;
    public GameObject ctrlPanel;
    public GameObject accPanel;
    public List<GameObject> panelList; // inspector ���� ����ֱ�

    public Item_MenuPopup1 item_popup1;
    public Item_MenuPopup2 item_popup2;

    private float soundOpt;       // ��ü ����
    private int languageOpt;      // ��� ���� idx

    public static Action backAction;

    void Start()
    {
        backAction += OnClickBackBtn;
    }

    public override void InitChild()
    {
        soundOpt = PlayerPrefs.GetFloat("soundOpt", 0.3f);
        languageOpt = PlayerPrefs.GetInt("languageOpt", 1);

        foreach (var p in panelList)
            p.SetActive(false);

        OnClickSndBtn();
    }

    #region BTN
    public void OnClickSndBtn()
    {
        BtnAction(0);
        SoundBtnAction();
    }

    public void OnClickCtrlBtn()
    {
        BtnAction(1);
        LanBtnAction();
    }

    public void OnClickAccBtn()
    {
        BtnAction(2);
    }

    public void BtnAction(int idx)
    {
        foreach (GameObject panel in panelList)
            panel.SetActive(false);

        panelList[idx].SetActive(true);
    }

    public override void OnClickBackBtn()
    {
        // ���� ���ϰ� ���� ó�� ���Դ� �������� �ٲ�� ����
        GameManager.Instance.soundManager.SoundSetting(item_popup1.tempSoundOpt);
        GameManager.Instance.language = (GameManager.Language)Data.LanguageOpt;
        EndPanel();
    }

    #endregion

    public void SoundBtnAction()
    {
        float[] temp = new float[3] { soundOpt, 0, 0 };
        item_popup1.Init(temp);
    }

    public void LanBtnAction()
    {
        int temp = languageOpt;
        item_popup2.Init(temp);
    }
    
}

// ������ DATA
public static class Data
{
    #region Variable
    private static float soundOpt;
    private static int languageOpt;
    #endregion

    #region get/set
    // ��ü ���� 
    public static float SoundOpt
    {
        get
        {
            return soundOpt;
        }
        set
        {
            soundOpt = value;
            PlayerPrefs.SetFloat(GetMemberName(() => soundOpt), value);
        }
    }

    //��� ���� idx
    public static int LanguageOpt
    {
        get
        {
            return languageOpt;
        }
        set
        {
            languageOpt = value;
            PlayerPrefs.SetInt(GetMemberName(() => languageOpt), value);
        }
    }

    private static string GetMemberName<T>(Expression<Func<T>> memberExpression)    //�������� string���� �������ִ� �Լ�. �������� �״�� key�� ���� ����. 
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    #endregion
}