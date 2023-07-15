using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translations", menuName = "Languages", order = 1)]
public class Translation : ScriptableObject
{
    //HEROES==============================================
    public string playText;

    [Header("Types")]
    public string Type1Name;
    public string Type2Name;
    public string Type3Name;

    public string Type1Description;
    public string Type2Description;
    public string Type3Description;


    public Translation() { }
}
