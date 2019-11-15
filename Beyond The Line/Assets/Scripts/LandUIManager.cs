using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandUIManager : MonoBehaviour
{
    [SerializeField]
    Button but;
    Button firstBut;
    Button secondBut;
    Canvas mainCanvas;
    RaceManager raceManager;
    float panelHeight;
    float panelWidth;
    
    public GameObject firstCheckpoint;
    public GameObject secondCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
        mainCanvas = GetComponent<Canvas>();
        firstCheckpoint = raceManager.checkpoints[0];
        secondCheckpoint = raceManager.checkpoints[1];
        firstBut = Instantiate(but, mainCanvas.transform);
        secondBut = Instantiate(but, mainCanvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        SetCheckpoint(mainCanvas.GetComponentInChildren<Button>(), raceManager.checkpoints[0].transform.position);
        //SetCheckpoint(secondBut, secondCheckpoint.transform.position, true);
    }

    void SetCheckpoint(Button checkpointBut, Vector3 checkpointPosition, bool fadeCheckpoint = false)
    {
        Vector3 ViewportPoint = Camera.main.WorldToViewportPoint(checkpointPosition);
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(checkpointPosition);
        bool onScreen = ViewportPoint.z > 0 && ViewportPoint.x > 0 && ViewportPoint.x < 1 && ViewportPoint.y > 0 && ViewportPoint.y < 1;
        if (onScreen)
        {
            checkpointBut.GetComponent<RectTransform>().anchoredPosition3D = screenPoint;
            Debug.Log("on screen");
        }
        //checkpointBut.transform.localPosition = new Vector3(but.transform.localPosition.x, but.transform.localPosition.y, 10);

        if (fadeCheckpoint)
        {
            checkpointBut.GetComponent<Image>().color = Color.blue;

        }

    }
}
