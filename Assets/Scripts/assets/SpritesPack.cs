using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesPack : MonoBehaviour
{
    public Sprite[] pack1;

    public Sprite[] pack2;

    public Sprite[] cats;

    public Sprite[] backgrounds;

    public Pack[] packs;

    public Sprite[] GetRandomPack(int Amount, int Diff)
    {
        if (packs.Length == 0)
        {
            return pack2;
        }
        else
        {
            List<Pack> smalls = new List<Pack>();
            List<Pack> bigs = new List<Pack>();

            for (int i = 0; i < packs.Length; i++)
            {
                if (packs[i].sprites.Length <= 10)
                {
                    smalls.Add(packs[i]);
                }
                else
                {
                    bigs.Add(packs[i]);
                }
            }


          


            int value = 0;
            int packID = -1;

            for (int i = 0; i < 100; i++)
            {
                value = UnityEngine.Random.Range(0, packs.Length);
                packID = packs[value].ID;
                if (Globals.PreviousPackIDNumber != packID) break;
            }
            Globals.PreviousPackIDNumber = packID;
            return packs[value].sprites;
        }
    }

    public Sprite GetRandomBackGround()
    {
        return backgrounds[UnityEngine.Random.Range(0, backgrounds.Length)];
    }

}
