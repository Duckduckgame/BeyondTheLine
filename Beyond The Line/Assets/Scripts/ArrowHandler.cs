using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{

    RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(raceManager.checkpoints[raceManager.crntCheckpoint].transform, Vector3.up);
    }
}
