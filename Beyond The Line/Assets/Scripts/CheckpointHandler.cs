using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointHandler : MonoBehaviour
{
    RaceManager raceManager;

    public int checkpointNumber;

    public bool firstCheckpoint = false;
    public bool hasPassed;

    private void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        raceManager.crntCheckpoint = checkpointNumber;
        hasPassed = true;

        if (firstCheckpoint)
            raceManager.FinishedLapUI();
           
    }
}
