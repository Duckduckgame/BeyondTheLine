using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpeedPointerRotator : MonoBehaviour
{

    Vector3 startRot;
    [SerializeField]
    Vector3 endRot = new Vector3(0,0, -93.50401f);
    HoverController hoverController;


    // Start is called before the first frame update
    void Start()
    {
        startRot = transform.eulerAngles;
        hoverController = FindObjectOfType<HoverController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hoverController == null) hoverController = FindObjectOfType<HoverController>();
        float lerpAmount = Mathf.InverseLerp(0, hoverController.maxAcceleration, hoverController.crntAcceleration);
        transform.eulerAngles = Vector3.Lerp(startRot, endRot, lerpAmount);
    }
}
