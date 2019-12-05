using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;


public class MasterSelectionHandler : MonoBehaviour
{
    public GameObject selectedVehicle;
    public string selectedScene = null;

    RaceManager crntRaceManager;
    [SerializeField]
    AudioSource MusicSource;
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
        if (GameObject.FindGameObjectWithTag("Rock") != null)
        {
            StartCoroutine(transitionBar(GameObject.FindGameObjectWithTag("Rock").GetComponent<Image>()));
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
            if(MusicSource.isPlaying) MusicSource.Stop();

        }
        else
        {
            if (MusicSource != null)
            {
                if (!MusicSource.isPlaying) MusicSource.Play();
            }
        }
    }

    IEnumerator transitionBar(Image image)
    {
        for (float i = 0; i < 1.1; i+= 0.1f)
        {
            image.fillAmount = i;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        SceneManager.LoadScene(selectedScene);
        yield return null;
    }
}
