using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrackObject : MonoBehaviour
{
    Button button;
    GameObject trackPreview;

    
    public int sceneIndexToLoad;

    public Vector3 targetPos;

    float slerpSpeed = 4;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * slerpSpeed);
    }

}
