using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panel : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private GameObject blinkEffect;
    [SerializeField] private GameObject matchEffect;

    public int ID;
    public Transform panelTransform;

    public bool IsFaceOn { get 
        {
            return (panelTransform.localEulerAngles.y != 0);
        } }

    public bool IsCompleted { get; private set; }
    public bool IsOpening { get; private set; }
    public bool IsClosing { get; private set; }

    private AudioManager _audioPack;

    public void SetPanelData(int _id, Sprite sprite)
    {
        _audioPack = GameManager.Instance.GetAudio;
        panelTransform = transform;
        Material newMaterial = Instantiate(_renderer.material);
        newMaterial.SetTexture("_MainTex", sprite.texture);
        ID = _id;
        _renderer.material = newMaterial;
        IsCompleted = false;
        blinkEffect.SetActive(false);
        matchEffect.SetActive(false);
    }

    public bool MakeCompleted()
    {
        if (IsCompleted) { return false; }
        IsCompleted = true;
        _audioPack.PlaySound_Success();
        matchEffect.SetActive(true);
        //panelTransform.localEulerAngles = new Vector3(0, 180, 0);
        StartCoroutine(playShowFace());
        return true;
    }

    public bool TryShowFace()
    {
        if (IsFaceOn || IsCompleted) { return false; }
        StartCoroutine(playShowFace());
        return true;
    }

    private IEnumerator playShowFace()
    {
        _audioPack.PlaySound_Click();
        blinkEffect.SetActive(true);

        IsOpening = true;
        IsClosing = false;

        panelTransform.DORotate(new Vector3(0, 180, 0), Globals.PanelSimpleRotationSpeed).SetEase(Ease.Linear);        
        yield return new WaitForSeconds(Globals.PanelTimeForShowing);
        blinkEffect.SetActive(false);

        IsOpening = false;

        if (!IsCompleted)
        {
            StartCoroutine(playHideFace());
        }
    }
    private IEnumerator playHideFace()
    {
        _audioPack.PlaySound_BackRotate();

        IsOpening = false;
        IsClosing = true;
        
        panelTransform.DORotate(Vector3.zero, Globals.PanelSimpleRotationSpeed).SetEase(Ease.Linear);
        yield return new WaitForSeconds(Globals.PanelTimeForShowing);

        IsClosing = false;
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public bool GetVisibility() => gameObject.activeSelf;

    public static void ArrangePanels(Sprite[] source, int panelsAmount, int similarPanelsAmount, ref List<panel> panels)
    {
        int uniques = panelsAmount / similarPanelsAmount;

        if (panelsAmount % similarPanelsAmount != 0)
        {
            Debug.LogError("количество карточек не четно количеству одинаковых");
        }

        if (panelsAmount < (similarPanelsAmount * 2))
        {
            Debug.LogError("количество панелей и пар для них не сщвпадают");
        }

        if (uniques > source.Length)
        {
            Debug.LogError("не хватает уникальных текстур");
        }

        List<int> panelsNumberToMark = new List<int>();
        for (int i = 0; i < panelsAmount; i++)
        {
            panelsNumberToMark.Add(i);
        }

        List<int> spritesNumberToSet = new List<int>();
        for (int i = 0; i < source.Length; i++)
        {
            spritesNumberToSet.Add(i);
        }

        for (int i = 0; i < uniques; i++)
        {
            int uniqueID = UnityEngine.Random.Range(0, 1000000);

            int spriteRND = UnityEngine.Random.Range(0, spritesNumberToSet.Count);
            Sprite sprite = source[spritesNumberToSet[spriteRND]];
            spritesNumberToSet.Remove(spritesNumberToSet[spriteRND]);

            for (int j = 0; j < similarPanelsAmount; j++)
            {
                int panelNumber = -1;
                if (panelsNumberToMark.Count > 1)
                {
                    int rnd = UnityEngine.Random.Range(0, panelsNumberToMark.Count);
                    panelNumber = panelsNumberToMark[rnd];
                    panelsNumberToMark.Remove(panelsNumberToMark[rnd]);
                }
                else
                {
                    panelNumber = panelsNumberToMark[0];
                    panelsNumberToMark.Remove(panelsNumberToMark[0]);
                }

                panels[panelNumber].SetPanelData(uniqueID, sprite);
            }
        }

    }

}
