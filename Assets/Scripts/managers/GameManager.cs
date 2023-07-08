using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Image backGround;
    [SerializeField] private Transform PanelsLocation;
    [SerializeField] private SpritesPack spritesPack;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject basicPanel;
    [SerializeField] private AudioManager _audio;

    [Header("Timer")]
    [SerializeField] private Image timerSliderImage;
    [SerializeField] private TextMeshProUGUI timerText;

    public AudioManager GetAudio { get => _audio; }

    [SerializeField] private Button re;
    private bool isRestaring;
    private bool isTouchActive;
    private bool isGameStarted;

    private int pairAmount;
    private int overallPanels
    {
        get
        {
            return (int)(Globals.PanelsNumber.x * Globals.PanelsNumber.y);
        }
    }

    private int collectedPanels;
        
    private float currentTimer;
    private float _timer;
    private readonly float timerUpdateCooldown = 1f;


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

        AudioListener.volume = 0.8f;

        pairAmount = (int)Globals.CurrentPairGroupType;
                
        int count = CreatePanels((int)Globals.PanelsNumber.x, (int)Globals.PanelsNumber.y);
        //ArrangePanels(spritesPack.GetRandomPack(), count, pairAmount);
        panel.ArrangePanels(spritesPack.GetRandomPack(), count, pairAmount, ref panels);
        backGround.sprite = spritesPack.GetRandomBackGround();

        timerSliderImage.fillAmount = 1f;
        timerText.text = "";
        currentTimer = Globals.TimeForLevelInSec;

        StartCoroutine(playShowPanels());
        StartCoroutine(fadeScreenOff());

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
        }
        else if (horizontaly <= 6 && vertically <= 4)
        {
            zAxis = -4.4f;
            //xAxis = 0.2f;
        }
        else if (horizontaly <= 6 && vertically <= 5)
        {
            zAxis = -4f;
            //xAxis = 0.2f;
        }
        else if (horizontaly <= 8 && vertically <= 5)
        {
            zAxis = -3.1f;
            xAxis = 0.1f;
        }
        else if (horizontaly <= 8 && vertically <= 6)
        {
            zAxis = -3.2f;
            xAxis = 0.1f;
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
                g.transform.localEulerAngles = new Vector3(0,0,UnityEngine.Random.Range(-1.5f, 1.5f));
                panel p = g.GetComponent<panel>();
                panels.Add(p);
                panelsAmount++;                
            }            
        }

        return panelsAmount;
    }

    
    private void Update()
    {
        if (isGameStarted)
        {
            currentTimer -= Time.deltaTime;
            //timer update every 1 sec
            if (_timer > timerUpdateCooldown)
            {
                _timer = 0;
                if (currentTimer >= 0) updateTimer();
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
        

        //check for finding
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
                        }
                    }
                    
                }                
            }
        }
    }

    private void updateTimer()
    {
        timerSliderImage.fillAmount = currentTimer / Globals.TimeForLevelInSec;
        int minutes = (int)(currentTimer / 60f);
        int seconds = (int)(currentTimer - (minutes * 60));

        string minutesToShow = minutes.ToString();
        string secondsToShow = seconds.ToString();

        if (minutes <=0)
        {
            minutesToShow = "00";
        }
        else if (minutes < 10)
        {
            minutesToShow = "0" + minutes.ToString();
        }

        if (seconds < 10)
        {
            secondsToShow = "0" + seconds.ToString();
        }

        timerText.text = minutesToShow + ":" + secondsToShow;
    }

    private IEnumerator playRestart()
    {
        isRestaring = true;
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(fadeScreenOn());
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

        yield return new WaitForSeconds(0.5f);

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
        isGameStarted = true;
    }

    private IEnumerator fadeScreenOff()
    {
        GameObject TransitionScreen = Instantiate(Resources.Load<GameObject>("TransitionCanvas"));
        TransitionScreen.gameObject.name = "TransitionScreen";
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 0);
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 1);
        yield return new WaitForSeconds(1);

        TransitionScreen.SetActive(false);
    }

    private IEnumerator fadeScreenOn()
    {
        GameObject TransitionScreen = Instantiate(Resources.Load<GameObject>("TransitionCanvas"));
        TransitionScreen.gameObject.name = "TransitionScreen";
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0);
        TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1);
        yield return new WaitForSeconds(1);                
    }
}
