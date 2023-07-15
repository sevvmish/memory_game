using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDesignManager
{
    public const int MAX_LVL_TYPE_1 = 30;
    public const int MAX_LVL_TYPE_2 = 15;

    private Vector2 PanelsNumber;
    private float PanelSimpleRotationSpeed;
    private PairGroupTypes CurrentPairGroupType;
    private float StageDurationInSec;
    private float PanelTimeForShowing;
    private int Difficulty;
    private bool isNextLevelOK;

    public GameDesignManager() {}

    public void SetLevelData(bool isNextLevelOK)
    {
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
        else
        {
            SceneManager.LoadScene("MainMenu");
        }

        CurrentPairGroupType = PairGroupTypes.two;
        PanelSimpleRotationSpeed = 0.35f;
        PanelTimeForShowing = 1.1f;
        Difficulty = 1;
        StageDurationInSec = 240;

        switch (Globals.GameLevel)
        {
            case 0:
                PanelsNumber = new Vector2(3,2);
                break;

            case 1:
                PanelsNumber = new Vector2(4, 2);
                break;

            case 2:
                PanelsNumber = new Vector2(4, 3);
                break;

            case 3:
                PanelsNumber = new Vector2(4, 4);
                break;

            case 4:
                PanelsNumber = new Vector2(5, 4);
                break;

            case 5:
                PanelsNumber = new Vector2(6, 4);
                break;

            case 6:
                PanelsNumber = new Vector2(6, 5);
                break;

            case 7:
                PanelsNumber = new Vector2(8, 5);
                break;

            case 8:
                PanelsNumber = new Vector2(8, 6);
                break;
        }

        update();
    }

    private void GameType_2_Logic()
    {

    }

    private void update()
    {
        Globals.PanelsNumber = PanelsNumber;
        Globals.PanelSimpleRotationSpeed = PanelSimpleRotationSpeed;
        Globals.CurrentPairGroupType = CurrentPairGroupType;
        Globals.StageDurationInSec = StageDurationInSec;
        Globals.PanelTimeForShowing = PanelTimeForShowing;
        Globals.Difficulty = Difficulty;

        Debug.Log("type: " + Globals.GameType + ", level: " + Globals.GameLevel);
        SceneManager.LoadScene("Gameplay");
    }
}

public enum PairGroupTypes
{
    two = 2, three = 3, four = 4
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
