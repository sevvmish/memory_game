using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDesignManager
{
    public const int MAX_LVL_TYPE_1 = 50;
    public const int MAX_LVL_TYPE_2 = 15;
    /*
    private Vector2 PanelsNumber;
    private float PanelSimpleRotationSpeed;
    private PairGroupTypes CurrentPairGroupType;
    private float StageDurationInSec;
    private float PanelTimeForShowing;
    private int Difficulty;    
    private int RewardedSeconds;
    */
    private bool isNextLevelOK;

    private List<Conditions> conditionsForTypeOne = new List<Conditions>();

    public GameDesignManager() 
    {
        InitForType1();

    }

    private void InitForType1()
    {
        int levelDuration = 0;
        int rewardsSecs = 5;
        float panelDuration = 1f;
        int additionalPanels = 0;

        for (int p = 0; p < 9; p++)
        {
            Vector2 cellsSpread = GetCellsSpreadByType((TwoPairCellsList)p);

            for (int difficulty = 1; difficulty <= 5; difficulty++)
            {
                //additional panels==========
                if (p == 0 && difficulty >= 4)
                {
                    additionalPanels = 2;
                }
                else if (p == 1 && (difficulty == 3 || difficulty == 4))
                {
                    additionalPanels = 2;
                }
                else if (p == 1 && difficulty == 5)
                {
                    additionalPanels = 4;
                }
                else if (p == 2 && (difficulty == 3 || difficulty == 4))
                {
                    additionalPanels = 2;
                }
                else if (p == 2 && difficulty == 5)
                {
                    additionalPanels = 4;
                }
                else if (p == 3 && (difficulty == 4 || difficulty == 5))
                {
                    additionalPanels = 4;
                }
                else if (p == 4 && (difficulty == 4 || difficulty == 5))
                {
                    additionalPanels = 4;
                }
                else if (p == 5 && (difficulty == 4 || difficulty == 5))
                {
                    additionalPanels = 4;
                }
                else if (p == 6 && (difficulty == 4 || difficulty == 5))
                {
                    additionalPanels = 6;
                }
                else
                {
                    additionalPanels = 0;
                }
                //===========================

                //level duration and rewards=============
                if (p == 0 && difficulty == 1)
                {
                    levelDuration = 30;
                }
                else
                {
                    switch ((TwoPairCellsList)p)
                    {
                        case TwoPairCellsList._3_2:
                            levelDuration = 12 + difficulty + additionalPanels;
                            rewardsSecs = 5;
                            break;

                        case TwoPairCellsList._4_2:
                            levelDuration = 20 + difficulty + additionalPanels;
                            rewardsSecs = 10;
                            break;

                        case TwoPairCellsList._4_3:
                            levelDuration = 30 + difficulty + additionalPanels * 2;
                            rewardsSecs = 15;
                            break;

                        case TwoPairCellsList._4_4:
                            levelDuration = 35 + (int)(difficulty * 1.5f) + additionalPanels * 2;
                            rewardsSecs = 20;
                            break;

                        case TwoPairCellsList._5_4:
                            levelDuration = 45 + difficulty * 2 + additionalPanels * 2;
                            rewardsSecs = 25;
                            break;

                        case TwoPairCellsList._6_4:
                            levelDuration = 70 + difficulty * 2 + additionalPanels * 3;
                            rewardsSecs = 30 + difficulty * 2;
                            break;

                        case TwoPairCellsList._6_5:
                            levelDuration = 80 + difficulty * 5 + additionalPanels * 3;
                            rewardsSecs = 40 + additionalPanels;
                            break;

                        case TwoPairCellsList._8_5:
                            levelDuration = 130 + difficulty * 10;
                            rewardsSecs = 45;
                            break;

                        case TwoPairCellsList._8_6:
                            levelDuration = 180 + difficulty * 10;
                            rewardsSecs = 60;
                            break;
                    }
                }
                //===========================

                //panel duration=============
                if (p == 4 || p == 5 || p == 6)
                {
                    panelDuration = 1.2f;
                }
                else if (p == 7 || p == 8)
                {
                    panelDuration = 1.3f;
                }
                else
                {
                    panelDuration = 1.1f;
                }
                //===========================


                conditionsForTypeOne.Add(new Conditions(cellsSpread, levelDuration, difficulty, PairGroupTypes.two, panelDuration, 0.3f, rewardsSecs, additionalPanels));

                if (
                    (p == 1 && difficulty == 5)
                    || (p == 2 && difficulty == 5)
                    )
                {
                    conditionsForTypeOne.Add(new Conditions(cellsSpread, levelDuration, difficulty, PairGroupTypes.two, panelDuration, 0.3f, rewardsSecs, additionalPanels));
                }
            }
        }

        for (int i = 0; i < 7; i++)
        {
            conditionsForTypeOne.Add(new Conditions(new Vector2(8, 6), 240, 5, PairGroupTypes.two, 1.3f, 0.3f, 60, 0));
        }

        Debug.Log(conditionsForTypeOne.Count + " !!!!!!!!!!!!!!!!!!!!!!");

        /*
        for (int i = 0; i < conditionsForTypeOne.Count; i++)
        {
            Debug.Log(conditionsForTypeOne[i].PanelsNumber + ": " + conditionsForTypeOne[i].StageDurationInSec + "sec, diff: " + conditionsForTypeOne[i].Difficulty + ", rew: " + conditionsForTypeOne[i].RewardedSeconds);
        }*/
    }

    public void SetLevelData(int _type, int _level)
    {
        Globals.IsRepeteGame = true;
        Globals.GameLevelRepete = _level;
        Globals.GameTypeRepete = _type;

        switch (Globals.GameType)
        {
            case 1:
                Conditions c = conditionsForTypeOne[_level];                
                update(c);
                break;

            case 2:
                GameType_2_Logic();
                break;
        }
    }

    public void SetLevelData( bool isNextLevelOK)
    {
        Globals.IsRepeteGame = false;
        this.isNextLevelOK = isNextLevelOK;

        switch(Globals.GameType)
        {
            case 1:
                GameType_1_Logic();
                break;

            case 2:
                GameType_2_Logic();
                break;
        }
    }

    private void GameType_1_Logic()
    {
        if (Globals.GameLevel < MAX_LVL_TYPE_1)
        {            
            if (isNextLevelOK) Globals.GameLevel++;
        }        
        

        Conditions c = new Conditions();
        if (Globals.GameLevel < conditionsForTypeOne.Count)
        {
            c = conditionsForTypeOne[Globals.GameLevel];
        }
        else
        {
            c = conditionsForTypeOne[conditionsForTypeOne.Count-1];
        }
        
        update(c);
    }

    private void GameType_2_Logic()
    {

    }

    private void update(Conditions c)
    {
        Globals.PanelsNumber = c.PanelsNumber;
        Globals.PanelSimpleRotationSpeed = c.PanelSimpleRotationSpeed;
        Globals.CurrentPairGroupType = c.CurrentPairGroupType;
        Globals.StageDurationInSec = c.StageDurationInSec;
        Globals.PanelTimeForShowing = c.PanelTimeForShowing;
        Globals.Difficulty = c.Difficulty;
        Globals.RewardedSeconds = c.RewardedSeconds;
        Globals.AdditionalPanelsAmount = c.AdditionalPanelsAmount;

        Debug.Log("type: " + Globals.GameType + ", level: " + Globals.GameLevel);
        SceneManager.LoadScene("Gameplay");
    }

    public static Vector2 GetCellsSpreadByType(TwoPairCellsList p)
    {
        switch (p)
        {
            case TwoPairCellsList._3_2:
                return new Vector2(3, 2);

            case TwoPairCellsList._4_2:
                return new Vector2(4, 2);

            case TwoPairCellsList._4_3:
                return new Vector2(4, 3);

            case TwoPairCellsList._4_4:
                return new Vector2(4, 4);

            case TwoPairCellsList._5_4:
                return new Vector2(5, 4);

            case TwoPairCellsList._6_4:
                return new Vector2(6, 4);

            case TwoPairCellsList._6_5:
                return new Vector2(6, 5);

            case TwoPairCellsList._8_5:
                return new Vector2(8, 5);

            case TwoPairCellsList._8_6:
                return new Vector2(8, 6);
        }

        return Vector2.zero;
    }
}

public enum PairGroupTypes
{
    two = 2, three = 3, four = 4
}

public enum TwoPairCellsList
{
    _3_2,
    _4_2,
    _4_3,
    _4_4,
    _5_4,
    _6_4,
    _6_5,
    _8_5,
    _8_6
}

public struct Conditions
{
    public Vector2 PanelsNumber;
    public float StageDurationInSec;
    public int Difficulty;
    public PairGroupTypes CurrentPairGroupType;
    public float PanelTimeForShowing;
    public float PanelSimpleRotationSpeed;
    public int RewardedSeconds;
    public int AdditionalPanelsAmount;

    public Conditions(Vector2 panelsNumber, float stageDurationInSec, int difficulty, PairGroupTypes currentPairGroupType, float panelTimeForShowing, float panelSimpleRotationSpeed, int rewardedSeconds, int additionalPanelsAmount)
    {
        PanelsNumber = panelsNumber;
        StageDurationInSec = stageDurationInSec;
        Difficulty = difficulty;
        CurrentPairGroupType = currentPairGroupType;
        PanelTimeForShowing = panelTimeForShowing;
        PanelSimpleRotationSpeed = panelSimpleRotationSpeed;
        RewardedSeconds = rewardedSeconds;
        AdditionalPanelsAmount = additionalPanelsAmount;
    }
}

//TWO
// 3/2 - 3 uniques
// 4/2 - 4 uniques
// 4/3 - 6 uniques
// 4/4 - 8 uniques
// 5/4 - 10 uniques
// 6/4 - 12 uniques
// 6/5 - 15 uniques
// 8/5 - 20 uniques
// 8/6 - 24 uniques    
//max = 8/6

//THREE
// 3/3 - 3 uniques
// 4/3 - 4 uniques
// 5/3 - 5 uniques
// 6/3 - 6 uniques
// 6/5 - 10 uniques
// 6/6 - 12 uniques
// 8/6 - 16 uniques
