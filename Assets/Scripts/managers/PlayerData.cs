using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int[] GT1P;
    public int[] GT2P;
    public int LGT;
    public string L;
    public int M;
    public int S;

    public PlayerData()
    {
        GT1P = new int[GameDesignManager.MAX_LVL_TYPE_1]; //save of level progress for type 1 of game 2/2
        GT2P = new int[GameDesignManager.MAX_LVL_TYPE_2]; //save of level progress for type 2 of game 3/3
        LGT = 0; //last played game type
        L = ""; //prefered language
        M = 1; //mobile platform? 1 - true;
        S = 1; // sound on? 1 - true;
        Debug.Log("created PlayerData instance");
    }


}
