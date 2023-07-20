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
    [SerializeField] private Button resetSave;

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

    [SerializeField] private GameObject cellWithBlock;
    [SerializeField] private GameObject cellWithNumber;
    [SerializeField] private Transform placeForLevelCellsForType1;
    [SerializeField] private Transform placeForLevelCellsForType2;

    [SerializeField] private Button playType1;
    [SerializeField] private Button playType2;

    [SerializeField] private Button Type1_DescriptorBack;
    [SerializeField] private Button Type2_DescriptorBack;

    [Header("panels tranlation")]
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private TextMeshProUGUI loadingSignText;

    [SerializeField] private TextMeshProUGUI type1Name;
    [SerializeField] private TextMeshProUGUI type2Name;

    [SerializeField] private TextMeshProUGUI type1Description;
    [SerializeField] private TextMeshProUGUI type2Description;

    private bool isTryGetData;
    private bool isLocalized;
    private Translation lang;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(fadeScreenOff());

        if (Globals.IsSoundOn)
        {
            _audio.UnMute();
        }
        else
        {
            _audio.Mute();
        }

        mainPlay.gameObject.SetActive(false);
        loading.gameObject.SetActive(true);

        chooseType.SetActive(false);
        Type1_Descriptor.SetActive(false);
        Type2_Descriptor.SetActive(false);

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
        });

        chooseType2.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(false);
            _audio.PlaySound_Click();
            Type1_Descriptor.SetActive(false);
            Type2_Descriptor.SetActive(true);
        });

        chooseType3.onClick.AddListener(() =>
        {
            mainPlay.gameObject.SetActive(false);
            chooseType.SetActive(false);
            _audio.PlaySound_Click();
            Type1_Descriptor.SetActive(false);
            Type2_Descriptor.SetActive(false);
        });

        Type1_DescriptorBack.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            TypeDescriptorsBack();
        });

        Type2_DescriptorBack.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            TypeDescriptorsBack();
        });
                
        playType1.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            Globals.IsRepeteGame = false;
            playType1Game();
        });

        playType2.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            Globals.IsRepeteGame = false;
            playType2Game();
        });

        /*
        resetSave.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            YandexGame.ResetSaveProgress();
            Globals.MainPlayerData = new PlayerData();
            YandexGame.SaveCloud();
        });*/
    }

    private void TypeDescriptorsBack()
    {
        mainPlay.gameObject.SetActive(false);
        chooseType.SetActive(true);
        _audio.PlaySound_Click();
        Type1_Descriptor.SetActive(false);
        Type2_Descriptor.SetActive(false);
    }

    private void InitGame()
    {
        SaveLoadManager.Load();

        print("SDK enabled: " + YandexGame.SDKEnabled);
        Globals.CurrentLanguage = YandexGame.savesData.language;
        print("language set to: " + Globals.CurrentLanguage);

        Globals.IsMobilePlatform = YandexGame.EnvironmentData.isMobile;
        print("platform mobile: " + Globals.IsMobilePlatform);

        if (Globals.MainPlayerData.S == 1)
        {
            Globals.IsSoundOn = true;
            _audio.UnMute();
        }
        else
        {
            Globals.IsSoundOn = false;
            _audio.Mute();
        }

        print("sound is: "+ Globals.IsSoundOn);
        

        if (Globals.TimeWhenStartedPlaying == DateTime.MinValue)
        {
            Globals.TimeWhenStartedPlaying = DateTime.Now;
            Globals.TimeWhenLastInterstitialWas = DateTime.Now;
            Globals.TimeWhenLastRewardedWas = DateTime.Now;
        }

        //Globals.CurrentLanguage = Globals.MainPlayerData.L;
        

        Globals.GameDesignManager = new GameDesignManager();
        //Globals.GameDesignManager.SetLevelData(true);
    }

    private void YGDataReady()
    {        
        if (!Globals.IsInitiated)
        {
            Globals.IsInitiated = true;
            InitGame();
        }    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            YandexGame.ResetSaveProgress();
            Globals.MainPlayerData = new PlayerData();
            SaveLoadManager.Save();
            SaveLoadManager.Load();
        }

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
                panel2_Descriptor();
                mainPlay.gameObject.SetActive(true);
                loading.gameObject.SetActive(false);
            }            
        }
        
    }

    private void Localize()
    {
        lang = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();

        playButtonText.text = lang.playText;
        loadingSignText.text = lang.loadingText;

        type1Name.text = lang.Type1Name;
        type2Name.text = lang.Type2Name;

        type1Description.text = lang.Type1Description;
        type2Description.text = lang.Type2Description;
    }

    private void panel1_Descriptor()
    {
        
        //print("type: " + Globals.GameType + ", level: " + Globals.GameLevel);

        for (int i = 0; i < Globals.MainPlayerData.GT1Pn.Length; i++)
        {
            
            if (Globals.MainPlayerData.GT1Pn[i] == 1 || (i == 0 && Globals.MainPlayerData.GT1Pn[i] == 0))
            {
                GameObject c = Instantiate(cellWithNumber, placeForLevelCellsForType1);
                c.SetActive(true);
                c.gameObject.name = i.ToString();
                c.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                c.GetComponent<ClickOnCurrentLevel>().LevelType = 1;
                c.GetComponent<ClickOnCurrentLevel>().CurrentLevel = i;
            }
            else
            {
                Instantiate(cellWithBlock, placeForLevelCellsForType1);
            }
        }
    }

    private void panel2_Descriptor()
    {
        for (int i = 0; i < Globals.MainPlayerData.GT2Pn.Length; i++)
        {

            if (Globals.MainPlayerData.GT2Pn[i] == 1 || (i == 0 && Globals.MainPlayerData.GT2Pn[i] == 0))
            {
                GameObject c = Instantiate(cellWithNumber, placeForLevelCellsForType2);
                c.SetActive(true);
                c.gameObject.name = i.ToString();
                c.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                c.GetComponent<ClickOnCurrentLevel>().LevelType = 2;
                c.GetComponent<ClickOnCurrentLevel>().CurrentLevel = i;
            }
            else
            {
                Instantiate(cellWithBlock, placeForLevelCellsForType2);
            }
        }
    }

    private void playType1Game()
    {
        Globals.GameType = 1;
        Globals.GameLevel = Globals.MainPlayerData.GT1Pn.Sum();
        Globals.GameDesignManager.SetLevelData(false);
    }

    private void playType2Game()
    {
        Globals.GameType = 2;
        Globals.GameLevel = Globals.MainPlayerData.GT2Pn.Sum();
        Globals.GameDesignManager.SetLevelData(false);
    }

    public static void RepeteGame(int _type, int number)
    {        
        Globals.GameDesignManager.SetLevelData(_type, number);
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
