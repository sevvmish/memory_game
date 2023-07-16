using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPlay;
    [SerializeField] private GameObject chooseType;

    [SerializeField] private Image backGround;
    [SerializeField] private AudioManager _audio;
    [SerializeField] private SpritesPack spritesPack;

    [SerializeField] private Button loading;
    [SerializeField] private Button mainPlayButton;
    [SerializeField] private Button chooseType1;
    [SerializeField] private Button chooseType2;
    [SerializeField] private Button chooseType3;

    [SerializeField] private GameObject Type1_Descriptor;
    [SerializeField] private GameObject Type2_Descriptor;
    [SerializeField] private GameObject Type3_Descriptor;

    [SerializeField] private GameObject cellWithBlock;
    [SerializeField] private GameObject cellWithNumber;
    [SerializeField] private Transform placeForLevelCellsForType1;
    [SerializeField] private Transform placeForLevelCellsForType2;
    [SerializeField] private Transform placeForLevelCellsForType3;

    [SerializeField] private Button playType1;
    [SerializeField] private Button playType2;
    [SerializeField] private Button playType3;

    [SerializeField] private Button Type1_DescriptorBack;
    [SerializeField] private Button Type2_DescriptorBack;
    [SerializeField] private Button Type3_DescriptorBack;

    [Header("panels tranlation")]
    [SerializeField] private TextMeshProUGUI playButtonText;

    [SerializeField] private TextMeshProUGUI type1Name;
    [SerializeField] private TextMeshProUGUI type2Name;
    [SerializeField] private TextMeshProUGUI type3Name;

    [SerializeField] private TextMeshProUGUI type1Description;
    [SerializeField] private TextMeshProUGUI type2Description;
    [SerializeField] private TextMeshProUGUI type3Description;

    private bool isTryGetData;
    private bool isLocalized;
    private Translation lang;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(fadeScreenOff());

        mainPlay.gameObject.SetActive(false);
        loading.gameObject.SetActive(true);

        chooseType.SetActive(false);
        Type1_Descriptor.SetActive(false);
        Type2_Descriptor.SetActive(false);
        Type3_Descriptor.SetActive(false);

        backGround.sprite = spritesPack.GetRandomBackGround();        
        //chooseType.SetActive(Globals.IsInitiated);

        mainPlayButton.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(true);
            _audio.PlaySound_Success();
        });

        chooseType1.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(false);
            _audio.PlaySound_Click();
            Type1_Descriptor.SetActive(true);
            Type2_Descriptor.SetActive(false);
            Type3_Descriptor.SetActive(false);
        });

        chooseType2.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(false);
            _audio.PlaySound_Click();
            Type1_Descriptor.SetActive(false);
            Type2_Descriptor.SetActive(true);
            Type3_Descriptor.SetActive(false);
        });

        chooseType3.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(false);
            _audio.PlaySound_Click();
            Type1_Descriptor.SetActive(false);
            Type2_Descriptor.SetActive(false);
            Type3_Descriptor.SetActive(true);
        });

        Type1_DescriptorBack.onClick.AddListener(() =>
        {
            TypeDescriptorsBack();
        });

        Type2_DescriptorBack.onClick.AddListener(() =>
        {
            TypeDescriptorsBack();
        });

        Type3_DescriptorBack.onClick.AddListener(() =>
        {
            TypeDescriptorsBack();
        });

        playType1.onClick.AddListener(() =>
        {
            playType1Game();
        });
    }

    private void TypeDescriptorsBack()
    {
        mainPlay.gameObject.SetActive(false);
        chooseType.SetActive(true);
        _audio.PlaySound_Click();
        Type1_Descriptor.SetActive(false);
        Type2_Descriptor.SetActive(false);
        Type3_Descriptor.SetActive(false);
    }

    private void InitGame()
    {
        SaveLoadManager.Load();

        if (Globals.TimeWhenStartedPlaying == DateTime.MinValue)
        {
            Globals.TimeWhenStartedPlaying = DateTime.Now;
            Globals.TimeWhenLastInterstitialWas = DateTime.Now;
            Globals.TimeWhenLastRewardedWas = DateTime.Now;
        }

        Globals.CurrentLanguage = Globals.MainPlayerData.L;
        Globals.GameType = 1;
        Globals.GameLevel = Globals.MainPlayerData.GT1P.Sum();
        print("type: " + Globals.GameType + ", level: " + Globals.GameLevel);

        Globals.GameDesignManager = new GameDesignManager();
        //Globals.GameDesignManager.SetLevelData(true);
    }

    private void YGDataReady()
    {
        print("SDK enabled: " + YandexGame.SDKEnabled);
        Globals.CurrentLanguage = YandexGame.savesData.language;
        print("language set to: " + Globals.CurrentLanguage);

        if (!Globals.IsInitiated)
        {
            Globals.IsInitiated = true;
            InitGame();
        }    
    }

    private void Update()
    {
        if (!Globals.IsInitiated)
        {
            if (!isTryGetData)
            {
                if (YandexGame.SDKEnabled)
                {
                    isTryGetData = true;
                    YGDataReady();
                    mainPlay.gameObject.SetActive(true);
                    loading.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (!isLocalized)
            {
                isLocalized = true;
                Localize();
                panel1_Descriptor();
                mainPlay.gameObject.SetActive(true);
                loading.gameObject.SetActive(false);
            }            
        }
        
    }

    private void Localize()
    {
        lang = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();

        playButtonText.text = lang.playText;

        type1Name.text = lang.Type1Name;
        type2Name.text = lang.Type2Name;
        type3Name.text = lang.Type3Name;

        type1Description.text = lang.Type1Description;
        type2Description.text = lang.Type2Description;
        type3Description.text = lang.Type3Description;
    }

    private void panel1_Descriptor()
    {
        for (int i = 0; i < Globals.MainPlayerData.GT1P.Length; i++)
        {
            
            if (Globals.MainPlayerData.GT1P[i] == 1 || (i == 0 && Globals.MainPlayerData.GT1P[i] == 0))
            {
                GameObject c = Instantiate(cellWithNumber, placeForLevelCellsForType1);
                c.SetActive(true);
                c.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            }
            else
            {
                Instantiate(cellWithBlock, placeForLevelCellsForType1);
            }
        }
    }

    private void playType1Game()
    {
        Globals.GameDesignManager.SetLevelData(false);
    }

    private IEnumerator fadeScreenOff()
    {
        GameObject TransitionScreen = Instantiate(Resources.Load<GameObject>("TransitionCanvas"));
        TransitionScreen.gameObject.name = "TransitionScreen";
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 0);
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 1);
        yield return new WaitForSeconds(1);

        TransitionScreen.SetActive(false);
    }

    private IEnumerator fadeScreenOn()
    {
        GameObject TransitionScreen = Instantiate(Resources.Load<GameObject>("TransitionCanvas"));
        TransitionScreen.gameObject.name = "TransitionScreen";
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0);
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1);
        yield return new WaitForSeconds(1);
    }
}
