using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using System;

public class Item_GalleryScroll : MonoBehaviour, ICell
{
    //UI
    public Text nameLabel;
    public Image mainImg;
    public Image enlargedImgBackground;
    public Image enlargedImgMain;

    //Model
    private Item_Info _itemInfo;
    //private int _itemIdx;
    private string _category;
    private string _imgId;

    //Action
    public GameObject enableBtn;
    public GameObject disableBtn;

    public void ConfigureCell(Item_Info itemInfo, /*int itemIndex,*/ string category, string imgId)//cell ���� ä���
    {
        //_itemIdx = itemIndex;
        _itemInfo = itemInfo;
        _category = category;
        _imgId = imgId;

        Sprite sprite = Resources.Load<Sprite>(_itemInfo.imgPath);
        if (sprite != null)
            mainImg.sprite = sprite;
        else
            Debug.LogWarning("��ο� ���� ����");

        CheckUnlockGallery();
    }

    public void CheckUnlockGallery()//cell Ȱ��ȭ ���� �Ǻ�
    {
        int temp = 0;

        if(_category == "Date")
        {
            if (GameManager.Instance.date_gallery_idx.TryGetValue(_imgId, out temp))
            {
                if (temp == 1)
                {
                    _itemInfo.isunlock = true;
                    EnableBtn();
                }
                else
                {
                    DisableBtn();
                }
            }
        }
        else if((_category == "Poke"))
        {
            int row = 0;
            //int restore = _itemIdx;
            int restore = _itemInfo.cell_idx;
            var poke_event_data = GameManager.Instance.poke_event_gallery_list;
            
            while(row < poke_event_data.Count)
            {
                if(restore - poke_event_data[row].Count < 0)
                {
                    break;
                }
                else
                {
                    restore -= poke_event_data[row].Count;
                    row++;
                }
            }

            if (poke_event_data[row][restore] == 1)
            {
                _itemInfo.isunlock = true;
                EnableBtn();
            }
            else
            {
                DisableBtn();
            }
        }
        else
        {
            if( (_category == "Twitter" && GameManager.Instance.twt_gallery_idx[_itemInfo.cell_idx] == 1) || (_category == "Pat" && GameManager.Instance.pat_gallery_idx[_itemInfo.cell_idx] == 1))
            {
                _itemInfo.isunlock = true;
                EnableBtn();
            }
            else
            {
                DisableBtn();
            }
        }
    }

    public bool GetIsOpen()
    {
        return _itemInfo.isunlock;
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

    public void EnlargeImgSet()//Ȯ��� �̹��� ������Ʈ ���� ���
    {
        GameObject canvasobj = GameObject.Find("Canvas");
        Image[] imglist = canvasobj.GetComponentsInChildren<Image>(true);
        EnlargedImg enlargedImg;

        foreach (var target in imglist) 
        {
            if (target.name == "EnlargeImgBackground")
            {
                enlargedImgBackground = target;
            }
            else if(target.name == "EnlargeImgMain")
            {
                enlargedImgMain = target;
            }
        }
        
        RectTransform rectTransform = enlargedImgBackground.GetComponent<RectTransform>();
        rectTransform.SetAsLastSibling();
        enlargedImg = enlargedImgMain.GetComponentInParent<EnlargedImg>();
        if(enlargedImg != null)
        {
            enlargedImg.Initlarge(_itemInfo.cell_idx);
        }
        else
        {
            Debug.Log("Cannot find enlargedimg class");
        }
    }

    public void EnlargeImg()//������ �̹��� Ȯ���ؼ� ����
    {
        EnlargeImgSet();
        Color mainalpha = enlargedImgMain.color;
        Color bgalpha = enlargedImgBackground.color;
        Sprite sprite = Resources.Load<Sprite>(_itemInfo.imgPath);
        if (sprite != null) 
        {
            mainalpha.a = 1f;
            bgalpha.a = 0.9f;
            enlargedImgMain.color = mainalpha;
            enlargedImgBackground.color = bgalpha;
            enlargedImgMain.sprite = sprite;
        }
    }
}
