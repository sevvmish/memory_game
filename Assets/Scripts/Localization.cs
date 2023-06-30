using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization
{
    private Translation translation;
    private Localization() 
    {
        translation = Resources.Load<Translation>("languages/russian");
    }

    private static Localization instance;
    public static Localization GetInstanse()
    {
        if (instance == null)
        {
            instance = new Localization();
        }

        return instance;
    }

    public Translation GetCurrentTranslation() => translation;

}
