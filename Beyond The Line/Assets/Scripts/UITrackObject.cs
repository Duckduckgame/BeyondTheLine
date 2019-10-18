using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrackObject : MonoBehaviour
{
    Button button;
    GameObject trackPreview;


    public Transform targetPos;

    [SerializeField]
    float slerpSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, targetPos.position, Time.deltaTime * slerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPos.rotation, Time.deltaTime * slerpSpeed);
    }
}
