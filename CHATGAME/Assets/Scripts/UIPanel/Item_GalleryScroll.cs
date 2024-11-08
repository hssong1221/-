using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using System;

public class Item_GalleryScroll : MonoBehaviour, ICell
{
    #region Values
    //UI
    public Image mainImg;
    public Image enlargedImgBackground;
    public Image enlargedImgMain;

    //Model
    private Item_Info _itemInfo;//Cell �� ������ �ִ� _contactList �� ����

    //Action
    public GameObject enableBtn;
    public GameObject disableBtn;
    #endregion

    #region Scroll Cell
    public void ConfigureCell(Item_Info itemInfo)//Cell ���� ä���
    {
        _itemInfo = itemInfo;

        Sprite sprite = Resources.Load<Sprite>(_itemInfo.imgPath);
        if (sprite != null)
        {
            mainImg.sprite = sprite;
        }
        else
            Debug.LogWarning("��ο� ���� ����");

        CheckUnlockGallery();
    }

    public void CheckUnlockGallery()//cell Ȱ��ȭ ���� �Ǻ�
    {
        if(_itemInfo.isunlock)
        {
            EnableBtn();
        }
        else
        {
            DisableBtn();
        }
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

    #endregion

    #region Enlarge
    public void OnClickEnlargeImg()//������ �̹��� Ȯ���ؼ� ����
    {
        UICtrl.Instance.ShowPanel("image/UI/UI_EnlargeImgBackground", UI.Instance.gameObject.transform);
        var enlargeImgPanel = UICtrl.Instance.panelInstance["UI_EnlargeImgBackground"];
        var enlargeMain = enlargeImgPanel.GetComponent<EnlargedImg>();
        enlargeMain.InitPath(_itemInfo.imgPath, _itemInfo.cell_idx);
    }
    #endregion
}
