using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static float PanelSimpleRotationSpeed = 0.35f;
    public static float PanelTimeForShowing
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
        //= 1.1f;

    public static PairGroupTypes CurrentPairGroupType = PairGroupTypes.two;
    public static Vector2 PanelsNumber = new Vector2(8, 6);

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
    // 10/6 - 30 uniques
    //max = 10/6

    //THREE
    // 3/3 - 3 uniques
    // 4/3 - 4 uniques
    // 5/3 - 5 uniques
    // 6/3 - 6 uniques
    // 6/5 - 10 uniques
    // 6/6 - 12 uniques
    // 8/6 - 16 uniques

}

public enum PairGroupTypes
{
    two = 2, three = 3, four = 4
}
