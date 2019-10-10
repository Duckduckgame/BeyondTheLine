using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public enum UIMode {Race, EndRace }
    public UIMode crntMode = UIMode.Race;
    UIMode oldMode;
    Dictionary<UIMode, CanvasGroup> modeToCanvas;
    #region RaceUI
    [SerializeField]
    CanvasGroup RaceCanvas;
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
    #endregion

    RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        modeToCanvas.Add(UIMode.Race, RaceCanvas);
        modeToCanvas.Add(UIMode.EndRace, endRaceCanvas);
        raceManager = FindObjectOfType<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckModeChange();
        float crntLapTimeNum = raceManager.crntLapTime;
        crntLapTime.text = Mathf.Floor(crntLapTimeNum / 60).ToString("00") + ":" + (crntLapTimeNum % 60).ToString("00") + ":" + ((crntLapTimeNum*1000) % 1000).ToString("00");
        lapCount.text = raceManager.crntLap.ToString() + "/" + raceManager.numberOfLaps.ToString();
        oldMode = crntMode;
    }

    void CheckModeChange()
    {
        if(oldMode != crntMode)
        {
            modeToCanvas[oldMode].alpha = 0;
            modeToCanvas[crntMode].alpha = 1;
            oldMode = crntMode;
        }
    }

    public void SetBestLapTime(float time)
    {
        bestLapTime.text = Mathf.Floor(time / 60).ToString("00") + ":" + (time % 60).ToString("00") + ":" + ((time * 1000) % 1000).ToString("00");
    }

    
}
