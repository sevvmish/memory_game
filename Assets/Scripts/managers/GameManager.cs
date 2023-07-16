using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using YG;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{    
    public static GameManager Instance { get; private set; }

    [Header("win or lose panels")]
    [SerializeField] private GameObject winLosePanel;
    [SerializeField] private GameObject winPartPanel; 
    [SerializeField] private GameObject losePartPanelNoReward;
    [SerializeField] private GameObject losePartPanelReward;
    [Header("win or lose buttons")]
    [SerializeField] private Button nextGameWinCondition;
    [SerializeField] private Button restartLoseConditionNoreward;
    [SerializeField] private Button restartLoseConditionReward;
    [SerializeField] private Button rewardForMoreSeconds;
    [Header("win or lose texts")]
    [SerializeField] private TextMeshProUGUI addSecondsInfoText;
    [SerializeField] private TextMeshProUGUI secondsAmountText;
    [SerializeField] private TextMeshProUGUI forRewardText;

    [SerializeField] private GameObject timerPanel;
    [SerializeField] private Button menu;
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

    //to del
    [SerializeField] private Button re;
    [SerializeField] private Button win;
    [SerializeField] private Button lose;

    //translatings and texts
    [Header("panels translation")]
    [SerializeField] private TextMeshProUGUI winLoseMessages;

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
        
    private List<Panel> panels = new List<Panel>();
    private List<Panel> groupsToCompare = new List<Panel>();

    private Action nextLevelAction;
    private Translation lang;

    private void Awake()
    {
        //Screen.SetResolution(1200, 600, true);
        //SaveLoadManager.Load();
        _audio.UnMute();
        lang = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();
        winLosePanel.SetActive(false);
        winPartPanel.SetActive(false);
        losePartPanelNoReward.SetActive(false);
        losePartPanelReward.SetActive(false);

        addSecondsInfoText.text = lang.getMoreSecondsInfo;
        secondsAmountText.text = Globals.RewardedSeconds.ToString() + " " + lang.secondsAmountPart;
        forRewardText.text = lang.forRewarded;

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
                        
        pairAmount = (int)Globals.CurrentPairGroupType;
                
        int count = Panel.CreatePanels((int)Globals.PanelsNumber.x, (int)Globals.PanelsNumber.y, 
            basicPanel, PanelsLocation, ref panels);        
        Panel.ArrangePanels(spritesPack.GetRandomPack(), count, pairAmount, ref panels);
        backGround.sprite = spritesPack.GetRandomBackGround();

        timerSliderImage.fillAmount = 1f;
        timerText.text = "";
        currentTimer = Globals.StageDurationInSec;
        timerPanel.SetActive(true);

        StartCoroutine(playShowPanels());
        StartCoroutine(fadeScreenOff());

        re.onClick.AddListener(() => 
        {
            SceneManager.LoadScene("Gameplay");
        });

        win.onClick.AddListener(() =>
        {
            gameWin();
        });

        lose.onClick.AddListener(() =>
        {
            currentTimer = 0;
        });

        menu.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            SceneManager.LoadScene("MainMenu");
        });

        Resources.UnloadUnusedAssets();

        nextGameWinCondition.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            ShowInterstitial();
        });

        restartLoseConditionNoreward.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            restartCurrentGame();
        });

        restartLoseConditionReward.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            restartCurrentGame();
        });

        rewardForMoreSeconds.onClick.AddListener(() =>
        {
            _audio.PlaySound_Click();
            ShowRewarded();
        });
    }

    

    
    private void Update()
    {
        if (!isGameStarted) return;

        if (currentTimer <= 0)
        {
            gameLose();
        }

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
     
        //WIN CONDITION
        if (overallPanels == collectedPanels && !isRestaring)
        {
            gameWin();
        }


        if (Input.GetMouseButton(0) && isTouchActive)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 20))
            {                
                if (hit.collider.TryGetComponent(out Panel takenPanel))
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

    private void gameWin()
    {
        timerPanel.SetActive(false);
        PanelsLocation.gameObject.SetActive(false);
        winLosePanel.SetActive(true);
        winPartPanel.SetActive(true);
        winLoseMessages.text = lang.winText;

        isGameStarted = false;
        nextLevelAction = toNextLevel;
        SaveLoadManager.Save();
        isRestaring = true;

        Debug.LogError("GAME WIN");
        
    }

    private void gameLose()
    {
        PanelsLocation.gameObject.SetActive(false);
        winLoseMessages.text = lang.loseText;
        winLosePanel.SetActive(true);

        isGameStarted = false;
        
        isRestaring = true;
        Debug.LogError("GAME LOST");

        print(DateTime.Now.Subtract(Globals.TimeWhenLastRewardedWas).TotalSeconds + " till starting rewarded");

        if (DateTime.Now.Subtract(Globals.TimeWhenLastRewardedWas).TotalSeconds > Globals.INTERVAL_FOR_REWARDED)
        {            
            losePartPanelReward.SetActive(true);
            losePartPanelNoReward.SetActive(false);
        }
        else
        {
            timerPanel.SetActive(false);
            losePartPanelReward.SetActive(false);
            losePartPanelNoReward.SetActive(true);
            nextLevelAction = restartCurrentGame;
        }
    }


    private void ShowRewarded()
    {
        StartCoroutine(playRewarded());
    }
    private IEnumerator playRewarded()
    {
        print("how much time till start rew: " + DateTime.Now.Subtract(Globals.TimeWhenLastRewardedWas).TotalSeconds);

        if (DateTime.Now.Subtract(Globals.TimeWhenLastRewardedWas).TotalSeconds > Globals.INTERVAL_FOR_REWARDED)
        {
            yield return new WaitForSeconds(0.5f);

            _audio.Mute();
            print("starting rewarded");
                       
            Globals.TimeWhenLastRewardedWas = DateTime.Now;
            Globals.TimeWhenLastInterstitialWas = DateTime.Now;
            Globals.RewardedAmount++;
            YandexGame.OpenVideoEvent = advStarted;
            YandexGame.CloseVideoEvent = advRewardedClosed;//nextLevelAction;
            YandexGame.ErrorVideoEvent = advRewardedError;//nextLevelAction;
            YandexGame.RewVideoShow(0);
        }
        else
        {
            print("not a time for interstitital");
            nextLevelAction?.Invoke();
        }
    }



    private void ShowInterstitial()
    {
        StartCoroutine(playInterstitial());
    }
    private IEnumerator playInterstitial()
    {
        print("how much time till start: " + DateTime.Now.Subtract(Globals.TimeWhenLastInterstitialWas).TotalSeconds);

        if (DateTime.Now.Subtract(Globals.TimeWhenLastInterstitialWas).TotalSeconds > Globals.INTERVAL_FOR_INTERSTITITAL)
        {
            yield return new WaitForSeconds(0.5f);

            _audio.Mute();
            print("starting interstitital");

            GameObject TransitionScreen = Instantiate(Resources.Load<GameObject>("TransitionCanvas"));
            TransitionScreen.gameObject.name = "TransitionScreen";
            TransitionScreen.transform.GetChild(0).GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 1);

            Globals.TimeWhenLastInterstitialWas = DateTime.Now;
            Globals.InterstitialsAmount++;
            YandexGame.OpenFullAdEvent = advStarted;
            YandexGame.CloseFullAdEvent = advClosed;//nextLevelAction;
            YandexGame.ErrorFullAdEvent = advError;//nextLevelAction;
            YandexGame.FullscreenShow();
        }
        else
        {
            print("not a time for interstitital");
            nextLevelAction?.Invoke();
        }
    }

    private void advStarted()
    {
        print("adv was OK");        
    }

    private void advClosed()
    {
        print("adv was closed");
        nextLevelAction?.Invoke();
    }

    private void advRewardedClosed()
    {
        print("rewarded was OK-closed");
        //
        PanelsLocation.gameObject.SetActive(true);
        currentTimer += Globals.RewardedSeconds;
        isGameStarted = true;
        isRestaring = false;
        winLosePanel.SetActive(false);
        losePartPanelReward.SetActive(false);
        losePartPanelNoReward.SetActive(false);
        timerPanel.SetActive(true);
    }

    private void advError()
    {
        print("adv was ERROR");
        nextLevelAction?.Invoke();        
    }

    private void advRewardedError()
    {
        print("adv was ERROR");
        losePartPanelReward.SetActive(false);
        losePartPanelNoReward.SetActive(true);
    }

    

    private void restartCurrentGame()
    {
        StartCoroutine(playrestartCurrentGame());
    }
    private IEnumerator playrestartCurrentGame()
    {
        //isRestaring = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(fadeScreenOn());
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Gameplay");
    }

    private void toNextLevel()
    {        
        StartCoroutine(playNextLevel());
    }
    private IEnumerator playNextLevel()
    {
        //isRestaring = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(fadeScreenOn());
        yield return new WaitForSeconds(1);
        Globals.GameDesignManager.SetLevelData(true);
    }

    private void updateTimer()
    {
        timerSliderImage.fillAmount = currentTimer / Globals.StageDurationInSec;
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
