﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] checkpoints;
    CheckpointHandler[] checkpointHandlers;
    public int crntCheckpoint;
    
    public float numberOfLaps;
    public float crntLap = 1;
    public float crntLapTime = 0;
    float bestLap = 0;
    float totalLapTimes = 0;

    float deathCount = 0;
    UIManager uIManager;

    Vector3 startPos;

    [SerializeField]
    bool raceOver = false;
    float timeSinceRaceOver;
    [SerializeField]
    float waitUntilLoad;


    // Start is called before the first frame update
    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        checkpointHandlers = new CheckpointHandler[checkpoints.Length];

        for(int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].GetComponent<CheckpointHandler>().checkpointNumber = i;
            checkpointHandlers[i] = checkpoints[i].GetComponent<CheckpointHandler>();
            
        }

        checkpoints[0].GetComponent<CheckpointHandler>().firstCheckpoint = true;
        startPos = checkpoints[0].transform.position + new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        crntLapTime += Time.deltaTime;

        if (raceOver)
        {
            timeSinceRaceOver += Time.deltaTime;
        }

        if(timeSinceRaceOver > waitUntilLoad)
        {
            SceneManager.LoadSceneAsync(0);
        }
    }

    public void PlayerRespawn(GameObject player)
    {
        deathCount++;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        HoverController hoverController = player.GetComponent <HoverController>();
        player.transform.position = checkpoints[crntCheckpoint].transform.position;
        player.transform.rotation = checkpoints[crntCheckpoint].transform.rotation;
        hoverController.targetRot = checkpoints[crntCheckpoint].transform.rotation;
        hoverController.targetVelocity = Vector3.zero;
    }

    public void FinishedLapUI()
    {
        if (checkpointHandlers[checkpointHandlers.Length - 2].hasPassed == true)
        {
            Debug.Log("lap ended at " + uIManager.crntLapTime.text);
            if (uIManager.bestLapSet == false || uIManager.bestLapTimeNum > crntLapTime)
            {
                uIManager.bestLapTimeNum = crntLapTime;
                uIManager.bestLapSet = true;
                uIManager.SetBestLapTime(crntLapTime);
                bestLap = crntLapTime;
            }
            totalLapTimes += crntLapTime;

            crntLapTime = 0;
            crntLap++;
            if(crntLap > numberOfLaps)
            {
                EndRace();
            }
            ResetCheckpoints();



        }
        else
        {
            Debug.Log("Skipped Checkpoint");
        }
    }

    void ResetCheckpoints()
    {

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].GetComponent<CheckpointHandler>().hasPassed = false;
            
        }
    }

    void EndRace() {
        Debug.Log("Race Over");
        string scene = SceneManager.GetActiveScene().name.ToString();
        Analytics.CustomEvent("raceEnd", new Dictionary<string, object> { { "Track", scene }, {"TotalTime", totalLapTimes }, {"BestLapTime", bestLap}, {"NumberOfLaps", numberOfLaps }, {"DeathCount", deathCount } });
        uIManager.totalLapTimes = totalLapTimes;
        uIManager.crntMode = UIManager.UIMode.EndRace;
        raceOver = true;
    }
}
