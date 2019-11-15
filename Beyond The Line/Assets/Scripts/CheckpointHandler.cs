using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteAlways]
public class CheckpointHandler : MonoBehaviour
{
    RaceManager raceManager;
    RaceManager.RaceType crntType;
    public int checkpointNumber;

    public bool firstCheckpoint = false;
    public bool hasPassed;

    [SerializeField]
    bool checkGround;

    public int deathsHere = 0;
    private void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
        crntType = RaceManager.RaceType.Land;//raceManager.crntType;
    }

    private void OnTriggerEnter(Collider other)
    {
        
            raceManager.crntCheckpoint = checkpointNumber;
            //hasPassed = true;
            raceManager.CheckpointPassed(checkpointNumber);
            if (firstCheckpoint)
            {
                raceManager.FinishedLap();
                Debug.Log("Lap done");
            }
                
        
           
    }
    private void OnTriggerExit(Collider other)
    {
        hasPassed = true;
    }

    private void OnDrawGizmos()
    {
        if(crntType == RaceManager.RaceType.Land)
        {
            Gizmos.DrawWireSphere(transform.position, 20f);

        }
    }

    private void OnValidate()
    {
        if (checkGround == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null)
                {
                    transform.position = hit.point;
                    checkGround = false;
                }
            }
            else if (Physics.Raycast(transform.position, Vector3.up, out hit))
            {
                if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null)
                {
                    transform.position = hit.point;
                    checkGround = false;
                }
            }
        }
    }
}
