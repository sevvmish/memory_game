using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translations", menuName = "Languages", order = 1)]
public class Translation : ScriptableObject
{
    //HEROES==============================================
    [Header("heroes")]
    public string LeapHeroName;
    


    public Translation() { }
}
