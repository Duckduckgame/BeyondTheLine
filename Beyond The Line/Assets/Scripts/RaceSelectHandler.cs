using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceSelectHandler : MonoBehaviour
{

    public GameObject[] tracks;

    GameObject selectedTrackOBJ;
    int movementIndex = 0;
    [SerializeField]
    int selectionIndex = 0;
    int nextIndex;
    int lastIndex;
    RaceUIPosHandler raceUIPosHandler;
    Vector3[] positions;
    [SerializeField]
    float UIDelayTime = 0.5f;
    float UIShiftTime = Mathf.Infinity;

    MasterSelectionHandler masterSelectionHandler;
    

    // Start is called before the first frame update
    void Start()
    {
        raceUIPosHandler = GetComponent<RaceUIPosHandler>();
        positions = raceUIPosHandler.positions;
        selectedTrackOBJ = tracks[movementIndex];
        Debug.Log(selectedTrackOBJ.transform.name);

        masterSelectionHandler = FindObjectOfType<MasterSelectionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        UIShiftTime += Time.deltaTime;

        if (Input.GetAxisRaw("Horizontal") > 0 && UIShiftTime > UIDelayTime) //shift right
        {
            UIShiftTime = 0;
            ShiftRight();
        }
        if (Input.GetAxisRaw("Horizontal") < 0 && UIShiftTime > UIDelayTime) //shift right
        {
            UIShiftTime = 0;
            ShiftLeft();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Jump"))
        {
            if (selectedTrackOBJ.GetComponent<UISelectionObject>().selectionType == UISelectionObject.SelectionType.Track)
            {
                masterSelectionHandler.selectedScene = selectedTrackOBJ.GetComponent<UISelectionObject>().stringToLoad;
                SceneManager.LoadScene("Car Select");
            }
            if (selectedTrackOBJ.GetComponent<UISelectionObject>().selectionType == UISelectionObject.SelectionType.Vehicle)
            {
                masterSelectionHandler.selectedVehicle = selectedTrackOBJ.GetComponent<UISelectionObject>().objectToLoad;
                masterSelectionHandler.LoadSelections();
            }
        }
    }

    void ShiftRight()
    {
        movementIndex--;
        if (movementIndex < 0) movementIndex += tracks.Length;
        if (movementIndex == tracks.Length) movementIndex -= tracks.Length;
        selectionIndex++;
        if (selectionIndex > tracks.Length -1) selectionIndex = 0;
        selectedTrackOBJ = tracks[selectionIndex];

        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].GetComponent<UISelectionObject>().targetPos = positions[(i + movementIndex) % tracks.Length];
        }
    }

    void ShiftLeft()
    {
        movementIndex++;
        if (movementIndex < 0) movementIndex += tracks.Length;
        if (movementIndex == tracks.Length) movementIndex -= tracks.Length;
        selectionIndex--;
        if (selectionIndex < 0) selectionIndex = tracks.Length-1;
        selectedTrackOBJ = tracks[selectionIndex];
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].GetComponent<UISelectionObject>().targetPos = positions[(i + movementIndex) % tracks.Length];
            
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
