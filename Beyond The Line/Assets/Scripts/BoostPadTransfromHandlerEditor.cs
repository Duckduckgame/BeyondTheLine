using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[SelectionBase]
[ExecuteAlways]
public class BoostPadTransfromHandlerEditor : MonoBehaviour
{

    [SerializeField]
    bool checkGround;
    [SerializeField]
    float boostStrength = 300;
    [SerializeField]
    float boostTime = 2;

    private void Update()
    {
        if (checkGround)
        {
            PositionToGround();
            checkGround = false;
        }
    }

    void PositionToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up * -1, out hit, 500f)){
            transform.position = hit.point;
            Quaternion targetRot;
            targetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            targetRot = Quaternion.Euler(targetRot.eulerAngles.x, transform.rotation.eulerAngles.y, targetRot.eulerAngles.z);
            //transform.rotation = targetRot;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<HoverController>() != null)
        {
            other.GetComponent<HoverController>().Boost(boostStrength, boostTime);
        }
    }
}
