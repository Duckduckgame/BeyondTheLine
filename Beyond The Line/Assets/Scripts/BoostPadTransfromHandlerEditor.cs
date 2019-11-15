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
    {/*
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up * -1, out hit, 500f)){
            if (hit.collider.gameObject.GetComponent<trackManager>() != null)
            {
                transform.position = hit.point;
            }
        }
        if (Physics.Raycast(transform.position, transform.up, out hit, 80f))
        {
            if (hit.collider.gameObject.GetComponent<trackManager>() != null)
            {
                transform.position = hit.point;
            }
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<HoverController>() != null)
        {
            other.GetComponent<HoverController>().Boost(boostStrength, boostTime);
        }
    }
}
