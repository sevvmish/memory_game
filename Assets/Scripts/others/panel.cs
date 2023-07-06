using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class panel : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
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
    }

    public bool MakeCompleted()
    {
        if (IsCompleted) { return false; }
        IsCompleted = true;
        _audioPack.PlaySound_Success();
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
}
