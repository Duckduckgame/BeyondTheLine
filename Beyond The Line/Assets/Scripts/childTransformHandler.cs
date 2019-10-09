using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class childTransformHandler : MonoBehaviour
{
    [SerializeField]
    float maxXRotation = 30;
    [SerializeField]
    float ZRotSpeed;
    [SerializeField]
    float velocityRotationInterpolation;
    HoverController tempHover;
    float oldInput = 0;
    float oldVelRotTarget = 0;

    Quaternion targetRotation;
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        tempHover = GetComponentInParent<HoverController>();   
    }

    // Update is called once per frame
    void Update()
    {
        
        float targetInput = Input.GetAxis("Horizontal");
        targetInput = Mathf.Lerp(oldInput, targetInput, Time.deltaTime * ZRotSpeed);

        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 45 * targetInput *-1));

        if(tempHover.groundSensed == false)
        {
            float velRotTarget = tempHover.GetComponent<Rigidbody>().velocity.y;
            velRotTarget = Mathf.Lerp(oldVelRotTarget, velRotTarget, Time.deltaTime * velocityRotationInterpolation);
            velRotTarget = Mathf.Clamp(velRotTarget, -maxXRotation, maxXRotation);
            transform.Rotate(transform.right, Mathf.Abs(velRotTarget), Space.World);

            oldVelRotTarget = velRotTarget;
        }
        if (tempHover.groundSensed)
        {
            oldVelRotTarget = 0;
        }

        oldInput = targetInput;
        
    }
}
