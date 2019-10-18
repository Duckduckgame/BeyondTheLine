using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceSelectHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] tracks;

    [SerializeField]
    Transform leftPos;
    [SerializeField]
    Transform rightPos;
    [SerializeField]
    Transform centrePos;

    GameObject selected;

    // Start is called before the first frame update
    void Start()
    {
        tracks[0].GetComponent<UITrackObject>().targetPos = leftPos;
        tracks[1].GetComponent<UITrackObject>().targetPos = centrePos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
