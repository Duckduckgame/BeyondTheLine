using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{

    AudioSource audioSource;


    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            StartRace();
        }
    }

    public void StartRace()
    {
        StartCoroutine(StartGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator StartGame()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        SceneManager.LoadScene("RaceSelect");
        yield return null;
    }
}
