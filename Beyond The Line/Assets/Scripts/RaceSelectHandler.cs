using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceSelectHandler : MonoBehaviour
{

    public GameObject[] tracks;

    GameObject selectedTrackOBJ;
    int movementIndex = 0;
    int selectionIndex = 0;
    int nextIndex;
    int lastIndex;
    RaceUIPosHandler raceUIPosHandler;
    Vector3[] positions;
    [SerializeField]
    float UIDelayTime = 0.5f;
    float UIShiftTime = Mathf.Infinity;

    // Start is called before the first frame update
    void Start()
    {
        raceUIPosHandler = GetComponent<RaceUIPosHandler>();
        positions = raceUIPosHandler.positions;
        selectedTrackOBJ = tracks[movementIndex];
        Debug.Log(selectedTrackOBJ.transform.name);
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(selectedTrackOBJ.GetComponent<UITrackObject>().sceneIndexToLoad);
        }
    }

    void ShiftRight()
    {
        movementIndex--;
        if (movementIndex < 0) movementIndex += tracks.Length;
        if (movementIndex == tracks.Length) movementIndex -= tracks.Length;
        selectedTrackOBJ = tracks[movementIndex];

        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].GetComponent<UITrackObject>().targetPos = positions[(i + movementIndex) % tracks.Length];
        }
    }

    void ShiftLeft()
    {
        movementIndex++;
        if (movementIndex < 0) movementIndex += tracks.Length;
        if (movementIndex == tracks.Length) movementIndex -= tracks.Length;
        selectedTrackOBJ = tracks[movementIndex];
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].GetComponent<UITrackObject>().targetPos = positions[(i + movementIndex) % tracks.Length];
            
        }
    }

}
