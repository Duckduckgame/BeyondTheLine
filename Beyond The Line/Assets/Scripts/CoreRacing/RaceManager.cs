using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class RaceManager : MonoBehaviour
{
    public enum RaceType {Track, Land };
    public RaceType crntType = RaceType.Land;
    [SerializeField]
    public GameObject[] checkpoints;
    CheckpointHandler[] checkpointHandlers;
    LandCheckpointHandler[] landCheckpointHandlers;
    public int crntCheckpoint;
    
    public float numberOfLaps;
    public float crntLap = 1;
    public float crntLapTime = 0;
    float bestLap = 0;
    float totalLapTimes = 0;

    float deathCount = 0;
    UIManager uIManager;

    Vector3 startPos;
    public int nextCheckpointIndex;

    public bool raceOver = false;
    float timeSinceRaceOver;
    [SerializeField]
    float waitUntilLoad;
    [SerializeField]
    GameObject fogSphere;
    [SerializeField]
    bool manuallyFinishRace = false;
    public GameObject player;
    AudioSource audioSource;
    [SerializeField]
    float audioVolumeLerp;
    [SerializeField]
    float customVolume;
    public bool shipGrounded;
    Quaternion startRot;
    LandUIManager landUIManager;
    bool useUI = true;
    bool onFinalCheckpoint;
    Medal medal;
    GameObject medalGO;
    [SerializeField]
    Transform medalSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        medal = GetComponent<Medal>();
        if (crntType == RaceType.Track)
        {
            uIManager = FindObjectOfType<UIManager>();
            GetTrackCheckpoints();
        }
        if (crntType == RaceType.Land)
        {
            landUIManager = FindObjectOfType<LandUIManager>();
            GetLandCheckpoints();
        }
        if (!useUI) uIManager.enabled = false;


        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            player = Instantiate(player, startPos, startRot);
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        audioSource.volume = customVolume;
    }

    void GetTrackCheckpoints() {
        checkpointHandlers = new CheckpointHandler[checkpoints.Length];

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].GetComponent<CheckpointHandler>().checkpointNumber = i;
            checkpointHandlers[i] = checkpoints[i].GetComponent<CheckpointHandler>();

        }

        checkpoints[0].GetComponent<CheckpointHandler>().firstCheckpoint = true;
        startPos = checkpoints[0].transform.position + new Vector3(0, 0, 10);
        startRot = checkpoints[0].transform.rotation;
    }

    void GetLandCheckpoints()
    {
        landCheckpointHandlers = new LandCheckpointHandler[checkpoints.Length];

        for (int i = 0; i < checkpoints.Length; i++)
        {
            landCheckpointHandlers[i] = checkpoints[i].GetComponent<LandCheckpointHandler>();
            landCheckpointHandlers[i].GetComponent<LandCheckpointHandler>().checkpointNumber = i;

        }

        landCheckpointHandlers[0].GetComponent<LandCheckpointHandler>().firstCheckpoint = true;
        landCheckpointHandlers[0].GetComponent<LandCheckpointHandler>().isNext = true;
        landCheckpointHandlers[1].GetComponent<LandCheckpointHandler>().isSecond = true;
        startPos = checkpoints[0].transform.position + new Vector3(0, 0, 10);
        startRot = checkpoints[0].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        crntLapTime += Time.deltaTime;


        if (raceOver)
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync("RaceSelect");
        }

        ShipAudio();
    }


    public void CheckpointPassed(int checkPointPassed)
    {
        if (onFinalCheckpoint)
        {
            EndRace();
        }
        if (crntType == RaceType.Land && checkPointPassed == crntCheckpoint && !onFinalCheckpoint) {
            landCheckpointHandlers[crntCheckpoint].isNext = false;
            landCheckpointHandlers[crntCheckpoint].isSecond = false;
            crntCheckpoint++;
            landCheckpointHandlers[crntCheckpoint].isNext = true;
            if(crntCheckpoint+1 < landCheckpointHandlers.Length)
            {
                landCheckpointHandlers[crntCheckpoint+1].isSecond = true;
            }
            else
            {
                onFinalCheckpoint = true;
            }
        }

    }

    private void ShipAudio()
    {
        if (player.GetComponent<HoverController>().timeSinceGroundSensed > 2)
        {
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.3f * customVolume, Time.deltaTime * audioVolumeLerp);
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, 0.8f, Time.deltaTime * audioVolumeLerp);
            }
        }
        else if (shipGrounded)
        {
            if (audioSource != null)
            {
                audioSource.volume = customVolume;
                audioSource.pitch = 1;
            }
        }
    }

    public void PlayerRespawn(GameObject player)
    {
        deathCount++;
        if(crntType == RaceType.Track)
        checkpointHandlers[crntCheckpoint].deathsHere++;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        HoverController hoverController = player.GetComponent <HoverController>();
        player.transform.position = checkpoints[crntCheckpoint].transform.position;
        player.transform.rotation = checkpoints[crntCheckpoint].transform.rotation;
        hoverController.targetRot = checkpoints[crntCheckpoint].transform.rotation;
        hoverController.targetVelocity = Vector3.zero;
    }

    public void FinishedLap()
    {
        if (checkpointHandlers[checkpointHandlers.Length - 2].hasPassed == true)
        {
            if (uIManager)
            {
                if (uIManager.bestLapSet == false || uIManager.bestLapTimeNum > crntLapTime)
                {
                    uIManager.bestLapTimeNum = crntLapTime;
                    uIManager.bestLapSet = true;
                    uIManager.SetBestLapTime(crntLapTime);
                    bestLap = crntLapTime;
                }
            }
            totalLapTimes += crntLapTime;

            crntLapTime = 0;
            crntLap++;
            if (crntLap > numberOfLaps)
            {
                EndRace();
            }


            ResetCheckpoints();
        }

    }

    void ResetCheckpoints()
    {

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].GetComponent<CheckpointHandler>().hasPassed = false;
            
        }
    }

    public void EndRace()
    {
        if(medal != null)
        SetMedal();

        Time.timeScale = 0.25f;
        Debug.Log("Race Over");
        HoverController hoverController = player.GetComponent<HoverController>();
        if (crntType == RaceType.Track)
        {
            string scene = SceneManager.GetActiveScene().name.ToString();
            Analytics.CustomEvent("finished: " + scene, new Dictionary<string, object> { { "Vehicle", player.transform.name }, { "TotalTime", totalLapTimes }, { "BestLapTime", bestLap }, { "NumberOfLaps", numberOfLaps }, { "DeathCount", deathCount } });
            Analytics.CustomEvent(player.transform.name + "Info " + scene, new Dictionary<string, object> { { "Vehicle", player.transform.name }, { "TotalTime", totalLapTimes }, { "strafeTime", hoverController.strafeTime_anal }, { "boostTime", hoverController.boostTime_anal }, { "xHeldTime", hoverController.xHeldTime_anal - hoverController.boostTime_anal }, { "maxSpeedTime", hoverController.maxSpeedTime_anal }, { "wallBashAmount", hoverController.wallBashAmount_anal }, { "wallBashTime", hoverController.wallBashTime_anal } });
            SendDeaths();
        }
        uIManager.enabled = true;
        uIManager.totalLapTimes = totalLapTimes;
        uIManager.crntMode = UIManager.UIMode.EndRace;

    }
        void SendDeaths()
        {
            bool necessary = false;
            foreach (CheckpointHandler CH in checkpointHandlers)
                {
                    if(CH.deathsHere > 0)
                    {
                        necessary = true;
                        break;
                    }
                }

            if (necessary)
            {
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    Analytics.CustomEvent("Death Locations in Glenn", new Dictionary<string, object> { { "CH1 count", checkpointHandlers[0].deathsHere }, { "CH2 count", checkpointHandlers[1].deathsHere }, { "CH3 count", checkpointHandlers[2].deathsHere }, { "CH4 count", checkpointHandlers[3].deathsHere }, { "CH5 count", checkpointHandlers[4].deathsHere }, { "CH6 count", checkpointHandlers[5].deathsHere } });
                }
                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    Analytics.CustomEvent("Death Locations in Canyon", new Dictionary<string, object> { { "CH1 count", checkpointHandlers[0].deathsHere }, { "CH2 count", checkpointHandlers[1].deathsHere }, { "CH3 count", checkpointHandlers[2].deathsHere }, { "CH4 count", checkpointHandlers[3].deathsHere }, { "CH5 count", checkpointHandlers[4].deathsHere }, { "CH6 count", checkpointHandlers[5].deathsHere }, { "CH7 count", checkpointHandlers[6].deathsHere }, { "CH8 count", checkpointHandlers[7].deathsHere }, { "CH9 count", checkpointHandlers[8].deathsHere } });
                }
            }
        }

    void SetMedal()
    {
        medalGO = medal.SetMedalByTime(totalLapTimes);
        medalGO.layer = 5;
        medalGO = Instantiate(medalGO, medalSpawnPoint.position, Quaternion.Euler(Vector3.zero), medalSpawnPoint);
        medalGO.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void QuitRace(bool toMenu)
    {
        if (FindObjectOfType<MasterSelectionHandler>() != null) {
            string scene = SceneManager.GetActiveScene().name.ToString();
            Analytics.CustomEvent("quit: " + scene, new Dictionary<string, object> { { "Track", scene }, { "Vehicle", player.transform.name }, { "TotalTime", totalLapTimes }, { "BestLapTime", bestLap }, { "NumberOfLaps", numberOfLaps }, { "DeathCount", deathCount } });
            SendDeaths();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Transform[] children = GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (i != children.Length-1)
                Gizmos.DrawLine(children[i].transform.position + new Vector3(0,500,0), children[i + 1].transform.position + new Vector3(0, 500, 0));

            }
        
    }
}
