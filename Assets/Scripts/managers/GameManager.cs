using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject back;
    [SerializeField] private Transform PanelsLocation;
    [SerializeField] private SpritesPack spritesPack;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject basicPanel;

    [SerializeField] private AudioManager _audio;
    public AudioManager GetAudio { get => _audio; }

    [SerializeField] private Button re;
    private bool isRestaring;
    private bool isTouchActive;

    private int pairAmount;
    private int overallPanels
    {
        get
        {
            return (int)(Globals.PanelsNumber.x * Globals.PanelsNumber.y);
        }
    }

    private int collectedPanels;

    private Ray ray;
    private RaycastHit hit;

    
    private List<panel> panels = new List<panel>();
    private List<panel> groupsToCompare = new List<panel>();

    private void Awake()
    {
        //Screen.SetResolution(1200, 600, true);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        AudioListener.volume = 0.7f;

        pairAmount = (int)Globals.CurrentPairGroupType;
                
        int count = CreatePanels((int)Globals.PanelsNumber.x, (int)Globals.PanelsNumber.y);
        ArrangePanels(spritesPack.GetRandomPack(), count, pairAmount);

        StartCoroutine(playShowPanels());

        re.onClick.AddListener(() => 
        {
            SceneManager.LoadScene("Gameplay");
        });
    }

    private int CreatePanels(int horizontaly, int vertically)
    {
        int panelsAmount = 0;

        float xAxis = 0;
        float yAxis = 0;
        float zAxis = 0;

        if (horizontaly <= 4)
        {
            zAxis = -4.5f;
        }
        else if (horizontaly <= 5)
        {
            zAxis = -4.7f;
            xAxis = 0.5f;
        }
        else if (horizontaly <= 6 && vertically <= 4)
        {
            zAxis = -4.4f;
            xAxis = 0.5f;
        }
        else if (horizontaly <= 6 && vertically <= 5)
        {
            zAxis = -3.9f;
            xAxis = 0.5f;
        }
        else if (horizontaly <= 8 && vertically <= 5)
        {
            zAxis = -3.6f;
            xAxis = 0.5f;
        }
        else if (horizontaly <= 8 && vertically <= 6)
        {
            zAxis = -3.2f;
            xAxis = 0.6f;
        }
        else if (horizontaly <= 10 && vertically <= 6)
        {
            zAxis = -3.2f;
            xAxis = 0.7f;
        }

        float startX = (float)horizontaly / 2 - 0.5f;
        float startY = (float)vertically / 2 - 0.5f;

        for (int x = 0; x < horizontaly; x++)
        {
            for (int y = 0; y < vertically; y++)
            {
                GameObject g = Instantiate(basicPanel, new Vector3(x - startX + xAxis, y - startY + yAxis, zAxis), Quaternion.identity, PanelsLocation);
                panel p = g.GetComponent<panel>();
                panels.Add(p);
                panelsAmount++;                
            }            
        }

        return panelsAmount;
    }

    private void ArrangePanels(Sprite[] source, int panelsAmount, int similarPanelsAmount)
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

    private void Update()
    {
        if (groupsToCompare.Count == pairAmount)
        {
            
            bool isOK = true;
            int ID = -1;
            for (int i = 0; i < groupsToCompare.Count; i++)
            {                
                if (ID >= 0 && groupsToCompare[i].ID != ID)
                {
                    
                    isOK = false;
                    break;
                }
                else if (ID < 0)
                {
                    ID = groupsToCompare[i].ID;
                }
            }

            if (isOK)
            {
                for (int i = 0; i < groupsToCompare.Count; i++)
                {
                    groupsToCompare[i].MakeCompleted();
                    groupsToCompare[i].panelTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    collectedPanels++;
                    print("panel compleed");
                }

                groupsToCompare.Clear();
            }     
            else
            {
                //groupsToCompare.Clear();
                for (int i = 0; i < groupsToCompare.Count; i++)
                {
                    if ((groupsToCompare[i].IsFaceOn && groupsToCompare[i].IsClosing) || !groupsToCompare[i].IsFaceOn)
                    {
                        groupsToCompare.Remove(groupsToCompare[i]);
                    }
                }
            }
        } 
        else if (groupsToCompare.Count > 0)
        {
            for (int i = 0; i < groupsToCompare.Count; i++)
            {
                if ((groupsToCompare[i].IsFaceOn && groupsToCompare[i].IsClosing) || !groupsToCompare[i].IsFaceOn)
                {
                    groupsToCompare.Remove(groupsToCompare[i]);
                }
            }
        }
     

        if (overallPanels == collectedPanels)
        {
            Debug.LogError("GAME WIN");
            if (!isRestaring) StartCoroutine(playRestart());
        }


        if (Input.GetMouseButton(0) && isTouchActive)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 20))
            {                
                if (hit.collider.TryGetComponent(out panel takenPanel))
                {
                    if (groupsToCompare.Count < pairAmount && !takenPanel.IsFaceOn &&  !groupsToCompare.Contains(takenPanel))
                    {
                        if (takenPanel.TryShowFace())
                        {
                            groupsToCompare.Add(takenPanel);
                            print("added to comparers");
                        }
                    }
                    
                }                
            }
        }
    }

    private IEnumerator playRestart()
    {
        isRestaring = true;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Gameplay");
    }

    private IEnumerator playShowPanels()
    {
        float xmin = 1000;
        float xmax = -1000;
        float ymin = 1000;
        float ymax = -1000;

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetVisibility(false);
            panels[i].transform.DOScale(Vector3.zero, 0);
            if (panels[i].transform.position.x < xmin)
            {
                xmin = panels[i].transform.position.x;
            }
            if (panels[i].transform.position.x > xmax)
            {
                xmax = panels[i].transform.position.x;
            }
            if (panels[i].transform.position.y < ymin)
            {
                ymin = panels[i].transform.position.y;
            }
            if (panels[i].transform.position.y > ymax)
            {
                ymax = panels[i].transform.position.y;
            }
        }

        float x = (xmin + xmax) / 2f;
        float y = (ymin + ymax) / 2f;

        for (float j = 0; j < 5; j+=0.15f)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (
                    panels[i].transform.position.x < (x + j) 
                    && panels[i].transform.position.x > (x - j)
                    && panels[i].transform.position.y < (y + j)
                    && panels[i].transform.position.y > (y - j)
                    )
                {
                    if (!panels[i].GetVisibility())
                    {                        
                        panels[i].SetVisibility(true);                        
                        panels[i].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutSine);
                    }
                    
                }
            }

            yield return new WaitForSeconds(0.02f);
        }

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetVisibility(true);
        }

        isTouchActive = true;      
    }

}
