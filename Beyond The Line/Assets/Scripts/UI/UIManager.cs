using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public enum UIMode {Race, Pause,  EndRace, StartRace }
    public UIMode crntMode = UIMode.Race;
    UIMode oldMode;
    Dictionary<UIMode, CanvasGroup> modeToCanvas;
    #region starRaceUI
    [SerializeField]
    CanvasGroup startCanvas;
    [SerializeField]
    TextMeshProUGUI countDownText;
    #endregion
    #region RaceUI
    public CanvasGroup raceCanvas;
    [SerializeField]
    TextMeshProUGUI lapCount;
    public TextMeshProUGUI crntLapTime;
    float crntLapMinutes = 0;
    float crntLapSeconds = 0;
    public TextMeshProUGUI bestLapTime;
    public float bestLapTimeNum = 0;
    public bool bestLapSet = false;
    public bool bestLapChange = false;
    #endregion
    [SerializeField]
    CanvasGroup pauseCanvas;
    #region EndRaceUI
    [SerializeField]
    CanvasGroup endRaceCanvas;
    public TextMeshProUGUI endRaceTotalTime;
    public TextMeshProUGUI endRaceBestTime;
    public float totalLapTimes;
    #endregion
    public RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        modeToCanvas = new Dictionary<UIMode, CanvasGroup>();
        modeToCanvas.Add(UIMode.StartRace, startCanvas);
        modeToCanvas.Add(UIMode.Race, raceCanvas);
        modeToCanvas.Add(UIMode.EndRace, endRaceCanvas);
        modeToCanvas.Add(UIMode.Pause, pauseCanvas);
        oldMode = crntMode;

        if (crntMode == UIMode.StartRace) RaceCountdown();
    }


    // Update is called once per frame
    void Update()
    {

        raceManager = FindObjectOfType<RaceManager>();
        if (crntMode == UIMode.EndRace) { UpdateEndRaceUI(); }
        float crntLapTimeNum = raceManager.crntLapTime;
        crntLapTime.text = Mathf.Floor(crntLapTimeNum / 60).ToString("00") + ":" + (crntLapTimeNum % 60).ToString("00") + ":" + ((crntLapTimeNum*100) % 100).ToString("##");
        lapCount.text = raceManager.crntLap.ToString() + "/" + raceManager.numberOfLaps.ToString();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("PS4 Start")) PauseUnpause();

        CheckModeChange();
        oldMode = crntMode;
    }

    void CheckModeChange()
    {
        if(oldMode != crntMode)
        {
            Debug.Log(oldMode);
            Debug.Log(crntMode);
            modeToCanvas[oldMode].alpha = 0;
            modeToCanvas[crntMode].alpha = 1;
            modeToCanvas[oldMode].interactable = false;
            modeToCanvas[crntMode].interactable = true;
            modeToCanvas[oldMode].blocksRaycasts = false;
            modeToCanvas[crntMode].blocksRaycasts = true;
            oldMode = crntMode;
        }
    }

    public void SetBestLapTime(float time)
    {
        bestLapTime.text = Mathf.Floor(time / 60).ToString("00") + ":" + (time % 60).ToString("00") + ":" + ((time * 1000) % 1000).ToString("00");
    }

    public void UpdateEndRaceUI()
    {
        endRaceTotalTime.text = Mathf.Floor(totalLapTimes / 60).ToString("00") + ":" + (totalLapTimes % 60).ToString("00") + ":" + ((totalLapTimes * 1000) % 1000).ToString("00");
        endRaceBestTime.text = bestLapTime.text;
    }

    public void RaceCountdown()
    {
        
        StartCoroutine(CountDown(3));
    }

    IEnumerator CountDown(int seconds)
    {
        int count = seconds;
        while (count > 0)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(1);
            count--;

            countDownText.text = count.ToString("0");
        }

        // count down is finished...
        Time.timeScale = 1f;
        crntMode = UIMode.Race;
    }


    public void PauseUnpause()
    {


        if (crntMode == UIMode.Race)
        {
            Time.timeScale = 0;
            crntMode = UIMode.Pause;
            return;
        }

        if (crntMode == UIMode.Pause)
        {
            Time.timeScale = 1;
            crntMode = UIMode.Race;
            return;
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        raceManager.QuitRace(true);
        SceneManager.LoadScene(0);
    }

    public void QuitToDesktop()
    {
        raceManager.QuitRace(false);
        Application.Quit();
    }

    public void EndRace()
    {
        raceManager.raceOver = true;
    }
    
}
