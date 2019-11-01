using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{

    public void StartRace()
    {
        SceneManager.LoadScene("RaceSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
