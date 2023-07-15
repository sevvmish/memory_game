using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image backGround;
    [SerializeField] private AudioManager _audio;
    [SerializeField] private SpritesPack spritesPack;

    private bool isTryGetData;

    // Start is called before the first frame update
    void Awake()
    {
        backGround.sprite = spritesPack.GetRandomBackGround();              
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
        Globals.GameDesignManager.SetLevelData(true);
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
        else
        {
            Globals.GameDesignManager.SetLevelData(true);
        }
        
    }

    private void Update()
    {
        if (!Globals.IsInitiated)
        {
            if (Globals.IsInitiated && !isTryGetData)
            {
                if (YandexGame.SDKEnabled)
                {
                    isTryGetData = true;
                    YGDataReady();
                }
            }
        }
        
    }
}
