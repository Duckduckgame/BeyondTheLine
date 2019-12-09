using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{

    AudioSource audioSource;
    [SerializeField]
    bool isLandMenu;

    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            StartRace();
        }
    }

    public void StartRace()
    {
        if (isLandMenu) { FindObjectOfType<MasterSelectionHandler>().loadAsLand = true; StartCoroutine(StartGame("Land01")); return; }
        
        if(!isLandMenu) FindObjectOfType<MasterSelectionHandler>().loadAsLand = false; StartCoroutine(StartGame("RaceSelect"));

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadTutorial()
    {
        FindObjectOfType<MasterSelectionHandler>().loadAsLand = false;
        FindObjectOfType<MasterSelectionHandler>().loadLandTut = true;
        StartCoroutine(StartGame("Track00"));
    }

    IEnumerator StartGame(string sceneName)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        SceneManager.LoadScene(sceneName);
        yield return null;
    }
}
