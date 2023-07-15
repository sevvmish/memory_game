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

    public PlayerData()
    {
        GT1P = new int[GameDesignManager.MAX_LVL_TYPE_1]; //save of level progress for type 1 of game 2/2
        GT2P = new int[GameDesignManager.MAX_LVL_TYPE_2]; //save of level progress for type 2 of game 3/3
        LGT = 0; //last played game type
        L = ""; //prefered language
        Debug.Log("created PlayerData instance");
    }


}
