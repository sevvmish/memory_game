using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static GameDesignManager GameDesignManager;
    public static PlayerData MainPlayerData;
    public static int GameType = 1;// = 1;
    public static int GameLevel = 3;// = 0;
    public static int Difficulty;// = 0;
    public static int RewardedSeconds = 20;// = 10;

    //GAME PRESETS
    public const float BASE_VOLUME = 0.8f;

    public static bool IsInitiated;
    public static DateTime TimeWhenStartedPlaying;

    public const float INTERVAL_FOR_INTERSTITITAL = 120;
    public const float INTERVAL_FOR_REWARDED = 120;
    public static DateTime TimeWhenLastInterstitialWas;
    public static int InterstitialsAmount;
    public static DateTime TimeWhenLastRewardedWas;
    public static int RewardedAmount;

    public static string CurrentLanguage;

    public static float PanelSimpleRotationSpeed = 0.35f;// = 0.35f;
    public static PairGroupTypes CurrentPairGroupType = PairGroupTypes.two;


    //level customization============================
    public static float StageDurationInSec = 240;
    public static Vector2 PanelsNumber = new Vector2(4, 2);

    //===============================================

    public static float PanelTimeForShowing = 1.1f;
    
    /*
    {
        get
        {
            switch(CurrentPairGroupType)
            {
                case PairGroupTypes.two:
                    return 1.1f;
                case PairGroupTypes.three:
                    return 1.3f;
            }

            return 1.1f;
        }
    }
    */
}


