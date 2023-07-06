using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesPack : MonoBehaviour
{
    public Sprite[] pack1;

    public Sprite[] pack2;

    public Pack[] packs;

    public Sprite[] GetRandomPack()
    {
        if (packs.Length == 0)
        {
            return pack2;
        }
        else
        {
            return packs[UnityEngine.Random.Range(0, packs.Length - 1)].sprites;
        }
    }

}
