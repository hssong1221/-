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

    [Header("�ؽ�Ʈ �ڽ� UI")]
    public Image mainImg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI affectionText;
    public TextMeshProUGUI dialogueText;
    [SerializeField]
    float TextDelayTime;
    Coroutine typingCoroutine;
    string saveLastText;

    [Header("ȣ���� Gauge")]
    public Image guageImg;
    public Image heart1;
    public Image heart2;
    public Image heart3;

    DataManager dataManager;
    //Waifu waifu;
    ICategory waifu;
   

    SheetData diaSheet;
    //SheetData ImgSheet;

    public enum CategoryState
    {
        Poke,
        Event,
        Twitter,
        Pat,
        Date,
        Next,
    }
    [Header("���� ī�װ� ����")]
    public CategoryState categoryState;
    
    public enum TextUIState
    {
        Before,
        Typing,
        End
    }
    [Header("���� Text UI ����")]
    public TextUIState textState;

    private Action SettingAction;

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
        //waifu = SingletonManager.Instance.GetSingleton<Waifu>();
        //waifu = SingletonManager.Instance.GetSingleton<AffectionPokeEvent>();
        waifu = SingletonManager.Instance.GetSingleton<Waifu>();


        diaSheet = dataManager.GetSheetData("Dialogue");
        //ImgSheet = dataManager.GetSheetData("ImgPath");

        nameText.text = "name";
        affectionText.text = "0";
        dialogueText.text = "dialogue";

        TextDelayTime = 0.05f;
        //DataSheetSetting(0, "Poke");

        SettingAction += SetMainImg;
        SettingAction += SetGauge;
        SettingAction += SetText;

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitUntil(() => waifu.GetDataList(CategoryState.Poke.ToString()).Count > 0);

        SettingAction?.Invoke();

        waifu.Affection_ascend();
        waifu.Interaction_Path();

        yield return null;
    }

    /*void Update()
    {

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (isBtn)
                return;

            if (touch.phase == TouchPhase.Began)
                isBtn = true;
            else if (touch.phase == TouchPhase.Ended)
                isBtn = false;

            OnClickPokeBtn();
        }

    }*/

    #region UI data setting

    public void ReLoad()
    {
        SettingAction?.Invoke();
    }

    public void SetMainImg()
    {
        string category = "";   // ��ư ���� + event
        string affState = "";   // ȣ���� ����
        int imgFileName = 0;         // �̹��� ���� �̸�
        
        category = categoryState.ToString();
        affState = waifu.Affection_compare();
        imgFileName = waifu.Interact_img_path();

        string imgPath = "";
        if (category.Equals("Poke") || category.Equals("Event"))
            imgPath = $"image/{category}/{affState}/{imgFileName + 1}";
        else
            imgPath = $"image/{category}/{imgFileName + 1}";
        
        Debug.Log($"���� �̹��� ��� : {imgPath}");

        Sprite sprite = Resources.Load<Sprite>(imgPath);
        if (sprite != null)
            mainImg.sprite = sprite;
        else
            Debug.LogWarning("��ο� ���� ����");
    }

    public void SetGauge()
    {
        float ratio = waifu.Affection_Percentage();
        heart1.gameObject.SetActive(ratio < 0.5);
        heart2.gameObject.SetActive(0.5 <= ratio && ratio < 1);
        heart3.gameObject.SetActive(ratio >= 1);
        guageImg.fillAmount = ratio;
    }

    public void SetText()
    {
        // waifu aff_poke_event_idx �κ��� ���� categorystate ���� �ٸ� idx�� ���� �ٲ�� ��
        var Idx = waifu.Interact_idx;

        var data = waifu.GetDataList(categoryState.ToString())[Idx];
        if (data == null)
            return;

        nameText.text = "����";

        var txt = GameManager.Instance.GetText(data);

        textState = TextUIState.Typing;
        saveLastText = txt;
        typingCoroutine = StartCoroutine(TypingEffect(txt));
    }

    // dialogue text Ÿ���� ȿ��
    IEnumerator TypingEffect(string txt)
    {
        for (int i = 0; i < txt.Length; i++)
        {
            dialogueText.text = txt.Substring(0, i + 1);
            yield return new WaitForSeconds(TextDelayTime);
        }
        textState = TextUIState.End;
    }
    // Ÿ���� ȿ�� ���߰� ��� ��ü ����
    public void StopTypingEffect()
    {
        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        dialogueText.text = saveLastText;
        textState = TextUIState.End;
    }

    

    #endregion

     #region ��ư��

    public void OnClickPokeBtn()
    {
        waifu = SingletonManager.Instance.GetSingleton<Waifu>();

        if (textState == TextUIState.Typing)
            StopTypingEffect();
        else
        {
            string temp = waifu.Check_Category();

            if (temp.Equals("Poke"))
                SetCategoryState(CategoryState.Poke);
            else if (temp.Equals("Event"))
                SetCategoryState(CategoryState.Event);

            SettingAction?.Invoke();

            waifu.Affection_ascend();
            waifu.Interaction_Path();

            ButtonAction.CheckUnlockAction?.Invoke();
        }
    }
    public void OnClickTwtBtn()
    {
        waifu = SingletonManager.Instance.GetSingleton<AffectionTwt>();

        if (textState == TextUIState.Typing)
            StopTypingEffect();
        else
        {
            string temp = waifu.Check_Category();
            if (temp.Equals("Twt"))
                SetCategoryState(CategoryState.Twitter);


            SettingAction?.Invoke();

            waifu.Affection_ascend();
            waifu.Interaction_Path();

            ButtonAction.CheckUnlockAction?.Invoke();
        }
    }
    public void OnClickPatBtn()
    {
        waifu = SingletonManager.Instance.GetSingleton<AffectionPat>();

        if (textState == TextUIState.Typing)
        {
            StopTypingEffect();
        }
        else
        {
            string temp = waifu.Check_Category();
            if (temp.Equals("Pat"))
                SetCategoryState(CategoryState.Pat);

            SettingAction?.Invoke();

            waifu.Affection_ascend();
            waifu.Interaction_Path();

            ButtonAction.CheckUnlockAction?.Invoke();
        }        
    }

    public void OnClickDateBtn()
    {
        waifu = SingletonManager.Instance.GetSingleton<AffectionDate>();

        if (textState == TextUIState.Typing)
        {
            StopTypingEffect();
        }
        else
        {
            string temp = waifu.Check_Category();
            if (temp.Equals("Date"))
                SetCategoryState(CategoryState.Date);

            SettingAction?.Invoke();

            waifu.Affection_ascend();
            waifu.Interaction_Path();

            ButtonAction.CheckUnlockAction?.Invoke();
        }
    }
    public void OnClickNextBtn()
    {
        //categoryState = CategoryState.Next;

        SettingAction?.Invoke();

        waifu.Affection_ascend();
        //waifu.aff_poke_event_idx += 1;
    }


    // temp version
    public void OnclickLanBtn()
    {
        if (GameManager.Instance.language == GameManager.Language.Kor)
            GameManager.Instance.SetLanguage(GameManager.Language.Eng);
        else if (GameManager.Instance.language == GameManager.Language.Eng)
            GameManager.Instance.SetLanguage(GameManager.Language.Kor);

        UI.Instance.ReLoad();
    }

    public void OnclickSaveBtn()
    {
        //Waifu.Instance.CreatePlayerData(true);
    }
    #endregion

    public void SetCategoryState(CategoryState state)
    {
        // ���� ����
        categoryState = state;
        // �÷��̾� ������ ����
        //Waifu.Instance.CreatePlayerData();
    }

    public void SetCategoryState(string state)
    {
        categoryState = (CategoryState)Enum.ToObject(typeof(CategoryState), state);
    }
}
