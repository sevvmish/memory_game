using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panel : MonoBehaviour
{
    public MeshRenderer renderer;
    public int ID;
    public Transform panelTransform;
    public bool IsFaceOn { get 
        {
            return (panelTransform.localEulerAngles.y != 0);
        } }

    public bool IsCompleted;
    public bool IsOpening { get; private set; }
    public bool IsClosing { get; private set; }

    public void SetPanelData(int _id, Sprite sprite)
    {
        panelTransform = transform;
        Material newMaterial = Instantiate(renderer.material);
        newMaterial.SetTexture("_MainTex", sprite.texture);
        ID = _id;
        renderer.material = newMaterial;
        IsCompleted = false;
    }

    public bool TryShowFace()
    {
        if (IsFaceOn || IsCompleted) { return false; }
        StartCoroutine(playShowFace());
        return true;
    }

    private bool TryHideFace()
    {
        if (!IsFaceOn || IsCompleted) { return false; }
        StartCoroutine(playHideFace());
        return true;
    }

    private IEnumerator playShowFace()
    {
        IsOpening = true;
        IsClosing = false;
        panelTransform.DORotate(new Vector3(0, 180, 0), Globals.PanelSimpleRotationSpeed).SetEase(Ease.Linear);
        yield return new WaitForSeconds(Globals.PanelTimeForShowing);

        IsOpening = false;

        if (!IsCompleted)
        {
            StartCoroutine(playHideFace());
        }
    }
    private IEnumerator playHideFace()
    {
        IsOpening = false;
        IsClosing = true;
        
        panelTransform.DORotate(Vector3.zero, Globals.PanelSimpleRotationSpeed).SetEase(Ease.Linear);
        yield return new WaitForSeconds(Globals.PanelTimeForShowing);

        IsClosing = false;
    }
}
