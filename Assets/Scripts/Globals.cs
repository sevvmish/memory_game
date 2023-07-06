using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static float PanelSimpleRotationSpeed = 0.35f;
    public static float PanelTimeForShowing = 1.1f;

    public static PairGroupTypes CurrentPairGroupType = PairGroupTypes.two;
    public static Vector2 PanelsNumber = new Vector2(8, 5); 

    //max = 10/6
}

public enum PairGroupTypes
{
    two = 2, three = 3, four = 4
}
