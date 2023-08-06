using UnityEngine;
using System.Linq;
using YG;

public class SaveLoadManager
{
    //[DllImport("__Internal")]
    //private static extern void SaveExtern(string data);

    //[DllImport("__Internal")]
    //private static extern void LoadExtern();

    private const string ID = "Playerdata28";

    public static void Save()
    {
        switch(Globals.GameType)
        {
            case 1:
                Globals.MainPlayerData.GT1Pn3[Globals.GameLevel] = 1;
                Globals.MainPlayerData.LGT1 = 1;
                break;

            case 2:
                Globals.MainPlayerData.GT2Pn3[Globals.GameLevel] = 1;
                Globals.MainPlayerData.LGT1 = 2;
                break;
        }

        Globals.MainPlayerData.L = Globals.CurrentLanguage;
        Globals.MainPlayerData.M = Globals.IsMobilePlatform ? 1 : 0;
        Globals.MainPlayerData.S = Globals.IsSoundOn ? 1 : 0;

        string data = JsonUtility.ToJson(Globals.MainPlayerData);
        //Debug.Log("saved: " + data);
        PlayerPrefs.SetString(ID, data);

        YandexGame.savesData.PlayerMainData3 = data;

        try
        {
            //YandexGame.SaveCloud();
            YandexGame.SaveProgress();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError("error saving data, defaults loaded");            
        }        
    }

    public static void Load()
    {
        string fromSave = "";
        YandexGame.LoadProgress();

        try
        {
            //YandexGame.LoadCloud();
            //YandexGame.LoadProgress();
            fromSave = YandexGame.savesData.PlayerMainData3;
            //Debug.Log("что получено из сейва облака: " + fromSave);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError("error loading data, defaults loaded");
            
        }
            
            
        if (!string.IsNullOrEmpty(fromSave))
        {
            

            Debug.Log("loaded: " + fromSave);
            try
            {
                Globals.MainPlayerData = JsonUtility.FromJson<PlayerData>(fromSave);
            }
            catch (System.Exception)
            {
                Globals.MainPlayerData = new PlayerData();
            }
                        
        }
        else
        {
            fromSave = PlayerPrefs.GetString(ID);

            if (string.IsNullOrEmpty(fromSave))
            {
                Globals.MainPlayerData = new PlayerData();
            }
            else
            {
                Globals.MainPlayerData = JsonUtility.FromJson<PlayerData>(fromSave);
            }                
        }       
    }

}
