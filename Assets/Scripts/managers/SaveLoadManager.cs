using UnityEngine;
using System.Linq;
using YG;

public class SaveLoadManager
{
    //[DllImport("__Internal")]
    //private static extern void SaveExtern(string data);

    //[DllImport("__Internal")]
    //private static extern void LoadExtern();

    private const string ID = "Playerdata07";

    public static void Save()
    {
        switch(Globals.GameType)
        {
            case 1:
                Globals.MainPlayerData.GT1P[Globals.GameLevel] = 1;
                Globals.MainPlayerData.LGT = 1;
                break;

            case 2:
                Globals.MainPlayerData.GT2P[Globals.GameLevel] = 1;
                Globals.MainPlayerData.LGT = 2;
                break;
        }

        Globals.MainPlayerData.L = Globals.CurrentLanguage;

        string data = JsonUtility.ToJson(Globals.MainPlayerData);
        Debug.Log("saved: " + data);
        PlayerPrefs.SetString(ID, data);

        try
        {
            YandexGame.savesData.PlayerMainData = data;
            YandexGame.SaveCloud();
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

        try
        {
            YandexGame.LoadCloud();
            fromSave = YandexGame.savesData.PlayerMainData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError("error loading data, defaults loaded");
            fromSave = PlayerPrefs.GetString(ID);
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
                Globals.MainPlayerData.LGT = 1;
                Globals.MainPlayerData.GT1P[0] = 1;
                Globals.MainPlayerData.GT1P[1] = 1;
                Globals.MainPlayerData.GT1P[2] = 1;
                Globals.MainPlayerData.GT1P[3] = 1;
            }
                        
        }
        else
        {
            Globals.MainPlayerData = new PlayerData();
        }       
    }

}
