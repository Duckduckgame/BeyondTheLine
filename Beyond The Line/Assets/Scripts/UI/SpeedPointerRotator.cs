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
        Vector3 usedEndRot = endRot;
        if (hoverController == null) hoverController = FindObjectOfType<HoverController>();
        float lerpAmount = Mathf.InverseLerp(0, hoverController.maxAcceleration, hoverController.crntAcceleration);
        if(hoverController.maxAcceleration - hoverController.usedAcceleration < 5 || hoverController.usedAcceleration > hoverController.maxAcceleration)
        {
            usedEndRot -= new Vector3(0,0, Random.Range(0, 15));
        }
        transform.eulerAngles = Vector3.Lerp(startRot, usedEndRot, lerpAmount);
    }
}
