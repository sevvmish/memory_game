using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static float PanelSimpleRotationSpeed = 0.5f;
    public static float PanelTimeForShowing = 1.2f;

    public static PairGroupTypes CurrentPairGroupType = PairGroupTypes.two;
    public static Vector2 PanelsNumber = new Vector2(4, 4);
}

public enum PairGroupTypes
{
    two = 2, three = 3, four = 4
}
