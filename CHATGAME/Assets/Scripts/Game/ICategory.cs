using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public interface ICategory
{
    public int Interact_idx { get; set; }
    public int Correction_number { get; set; }

    //public Affection_status affection_status { get; set; }

    public Action SheetLoadAction { get; set; }

    //public void CreatePlayerData(bool isSave);

    #region EXCEL Data
    public void SetSheetData();
    public void SheetData_Categorize();
    public List<Dictionary<string, string>> GetDataList(string name);
    #endregion

    public void Affection_ascend();

    public float Affection_Percentage();

    public string Affection_compare();

    public string Check_Category();
        
    public void Interaction_Path();

    public int Interact_img_path();

    public int Interact_txt_path();

    public void Sequence_Init();
}
