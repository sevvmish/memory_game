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
        ArrangePanels(spritesPack.pack1, count, pairAmount);

        re.onClick.AddListener(() => 
        {
            SceneManager.LoadScene("SampleScene");
        });
    }

    private int CreatePanels(int horizontaly, int vertically)
    {
        int panelsAmount = 0;

        int max = horizontaly > vertically ? horizontaly : vertically;
        float zAxis = 0;

        if (max <= 4)
        {
            zAxis = -3.5f;
        }
        else if (max <= 5)
        {
            zAxis = -3.5f;
        }
        else if (max <= 6)
        {
            zAxis = -3.5f;
        }

        float startX = (float)horizontaly / 2 - 0.5f;
        float startY = (float)vertically / 2 - 0.5f;

        for (int x = 0; x < horizontaly; x++)
        {
            for (int y = 0; y < vertically; y++)
            {
                GameObject g = Instantiate(basicPanel, new Vector3(x - startX, y - startY, zAxis), Quaternion.identity, PanelsLocation);
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
            Debug.LogError("���������� �������� �� ����� ���������� ����������");
        }

        if (panelsAmount < (similarPanelsAmount * 2))
        {
            Debug.LogError("���������� ������� � ��� ��� ��� �� ���������");
        }

        if (uniques > source.Length)
        {
            Debug.LogError("�� ������� ���������� �������");
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


        if (Input.GetMouseButton(0))
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
        SceneManager.LoadScene("SampleScene");
    }

    

}
