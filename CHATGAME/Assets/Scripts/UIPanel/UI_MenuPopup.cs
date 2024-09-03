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

    private float soundOpt;       // ��ü ����

    public override void InitChild()
    {
        soundOpt = PlayerPrefs.GetFloat("soundOpt", 0.3f);

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

    #endregion

    public void SoundBtnAction()
    {
        float[] temp = new float[3] { soundOpt, 0, 0 };
        item_popup1.Init(temp);
    }

    
}
public static class Data
{
    #region Variable
    private static float soundOpt;
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

    private static string GetMemberName<T>(Expression<Func<T>> memberExpression)    //�������� string���� �������ִ� �Լ�. �������� �״�� key�� ���� ����. 
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    #endregion
}