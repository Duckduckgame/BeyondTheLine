using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;


public class MasterSelectionHandler : MonoBehaviour
{
    public GameObject selectedVehicle;
    public string selectedScene = null;

    RaceManager crntRaceManager;
    [SerializeField]
    AudioSource audioSource;
    // Start is called before the first frame update

    private static MasterSelectionHandler _instance;

    public static MasterSelectionHandler Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSelections()
    {
        if (FindObjectOfType<transitionPlane>())
        {
            StartCoroutine(FindObjectOfType<transitionPlane>().TransitionOut(selectedScene));
        }
        else
        {
            SceneManager.LoadScene(selectedScene);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (FindObjectOfType<transitionPlane>())
        {
            StartCoroutine(FindObjectOfType<transitionPlane>().TransitionIn());
        }
        if(FindObjectOfType<RaceManager>() != null && selectedVehicle != null)
        {
            Analytics.CustomEvent("Race Started", new Dictionary<string, object> { { "Track", SceneManager.GetActiveScene().name.ToString() }, {"Vehicle ", selectedVehicle.transform.name } });
            crntRaceManager = FindObjectOfType<RaceManager>();
            crntRaceManager.crntType = RaceManager.RaceType.Track;
            crntRaceManager.player = selectedVehicle;
            if(audioSource.isPlaying) audioSource.Stop();

        }
        else
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }
}
