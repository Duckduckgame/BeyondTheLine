using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandCourseLoader : MonoBehaviour
{

    [SerializeField]
    GameObject mountainCourse;

    [SerializeField]
    GameObject forestCourse;

    ArrowHandler arrowHandler;

    private void Start()
    {
        arrowHandler = FindObjectOfType<ArrowHandler>();
        arrowHandler.gameObject.SetActive(false);
    }

    public void LoadMountain()
    {
        mountainCourse.SetActive(true);
        arrowHandler.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void LoadForest()
    {
        forestCourse.SetActive(true);
        arrowHandler.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void QuitToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu Land");
    }
}
