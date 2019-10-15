using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public enum UIMode {Race, EndRace, StartRace }
    public UIMode crntMode = UIMode.Race;
    UIMode oldMode;
    Dictionary<UIMode, CanvasGroup> modeToCanvas;
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
        modeToCanvas.Add(UIMode.Race, raceCanvas);
        modeToCanvas.Add(UIMode.EndRace, endRaceCanvas);
        oldMode = crntMode;
    }


    // Update is called once per frame
    void Update()
    {
        CheckModeChange();
        if (crntMode == UIMode.EndRace) { UpdateEndRaceUI(); }
        float crntLapTimeNum = raceManager.crntLapTime;
        crntLapTime.text = Mathf.Floor(crntLapTimeNum / 60).ToString("00") + ":" + (crntLapTimeNum % 60).ToString("00") + ":" + ((crntLapTimeNum*100) % 100).ToString("##");
        lapCount.text = raceManager.crntLap.ToString() + "/" + raceManager.numberOfLaps.ToString();
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
    
}
