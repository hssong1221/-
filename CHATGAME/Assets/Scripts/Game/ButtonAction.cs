using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonAction : MonoBehaviour
{
    #region Values
    public Button button;
    public GameObject enableBtn;
    public GameObject disableBtn;
    public int unlockLevel;/// <summary>
    /// ��ư Ȱ��ȭ ���� ����
    /// </summary>
    public int unlockCount;/// <summary>
    /// ��ư Ȱ��ȭ ���� poke ī��Ʈ Ƚ��
    /// </summary>
    public string cateType;/// <summary>
    /// ��ư ī�װ� Ÿ��
    /// </summary>

    public static Action CheckUnlockAction;
    #endregion

    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        CheckUnlockAction += CheckLockNumber;
    }

    public void CheckLockNumber()
    {
        if (GameManager.Instance.affection_lv >= unlockLevel && GameManager.Instance.unlockBtnCnt[cateType] >= unlockCount && !GameManager.Instance.isDate && GameManager.Instance.affection_lv % 2 == 0)
            EnableBtn();
        else
            DisableBtn();
    }

    public void EnableBtn()
    {
        enableBtn.SetActive(true);
        disableBtn.SetActive(false);
    }

    public void DisableBtn()
    {
        enableBtn.SetActive(false);
        disableBtn.SetActive(true);
    }
}
