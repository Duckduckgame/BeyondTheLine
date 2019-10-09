using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class BoostPadTransfromHandlerEditor : MonoBehaviour
{
    [SerializeField]
    bool checkGround;

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
        if(Physics.Raycast(transform.position, transform.up * -1, out hit, 50f)){
            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<HoverController>() != null)
        {
            other.GetComponent<HoverController>().Boost(1, 5f);
        }
    }
}
