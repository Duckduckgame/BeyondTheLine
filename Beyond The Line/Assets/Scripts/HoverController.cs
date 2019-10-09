﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class HoverController : MonoBehaviour
{

    Rigidbody rb;
    float horiInput;
    float vertInput;

    [Header("Speed")]
    [SerializeField]
    float accellerationIncrease = 10;
    [SerializeField]
    float maxAcceleration = 300;
    [SerializeField]
    float speedDecrease = 1;
    float crntAcceleration = 0;
    [SerializeField]
    float velocityInterpolation = 1;
    [SerializeField]
    float jumpForce = 5;
    [SerializeField]
    float flyingSpeedMultiplier = 0.5f;
    Vector3 targetVelocity = Vector3.zero;

    [Header("Boost")]
    [SerializeField]
    float boostMultiplier;
    float crntBoost;
    float crntBoostTime;
    float crntMaxBoostTime;


    [Header("Rotations")]
    [SerializeField]
    float turnSpeed = 2;
    [SerializeField]
    float turnSpeedInterpolation = 4;
    Quaternion targetRot;
    [SerializeField]
    float rotationInterpolation = 1;
    [SerializeField]
    float predictRotationLength = 10;
    bool surfacePredicted;
    Vector3 targetVelocityDirection;
    float oldTurnSpeed = 0;
    [SerializeField]
    float rotSenseHeight = 5;

    [Header("Hover")]
    [SerializeField]
    float hoverHeight = 4;
    [SerializeField]
    float PositionToGroundInterpolation = 3;
    [SerializeField]
    float maxSenseHeight = 10;
    public bool grounded = false;
    public bool groundSensed = false;
    [SerializeField]
    float gravityStrength = 140;
    float crntGravity = 0;
    [SerializeField]
    float hangTime = 0.2f;
    [SerializeField]
    float gravityInterpolation;
    [SerializeField]
    float springForce = 150;
    [SerializeField]
    float timeSinceGroundSensed;
    Vector3 hitPoint;

    [Header("Strafing")]
    [SerializeField]
    float strafeSpeed = 250;

    [Header("children")]
    [SerializeField]
    Transform rotationRay;
    [SerializeField]
    Transform positionRay;

    [Header("Misc")]
    [SerializeField]
    CinemachineVirtualCamera CVC;
    [SerializeField]
    Vector3 CVCFlyingOffset;
    Vector3 CVCTargetPosition;
    [SerializeField]
    float CVCPositionInterpolation = 1;
    [SerializeField]
    float CVCNoiseLerpMax = 250;
    Vector3 CVCOffset;
    [SerializeField]
    bool usingPS4Controller = false;

    [SerializeField]
    float velMag;
    [SerializeField]
    float forwardVelMag;
    [SerializeField]
    float rotDifferences;

    Vector3 lerpPoint;

    Vector3 spawnPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CVCOffset = CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        spawnPos = transform.position;
    }

    void Update()
    {
        horiInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        float R2 = Input.GetAxis("PS4_R2");
        float L2 = Input.GetAxis("PS4_L2");
        R2 = (R2 + 1) * 0.5f;
        L2 = ((L2 + 1) * 0.5f) * -1;
        if (usingPS4Controller)
        {
            vertInput = R2 + L2;
        }
        if (Input.GetKeyDown(KeyCode.T))
            transform.position = spawnPos;

        timeSinceGroundSensed += Time.deltaTime;
        crntBoostTime += Time.deltaTime;
        
            
       
    }

    private void FixedUpdate()
    {
        targetVelocityDirection = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(positionRay.position, transform.up * -1, out hit, maxSenseHeight))
        {
            timeSinceGroundSensed = 0;
            groundSensed = true;
            if (hit.distance > hoverHeight) { grounded = false; }
            else {
                grounded = true;
                crntGravity = 0;
                CVCTargetPosition = CVCOffset;
            }
            Vector3 targetPos = hit.point + (transform.rotation.normalized * new Vector3(0, hoverHeight, 0));
            hitPoint = hit.point;

            float springCompression = ((Mathf.InverseLerp(0.2f, hoverHeight * 2, Vector3.Distance(transform.position, hit.point)) * 2) - 1) * -1;
            targetVelocityDirection += (transform.up * springCompression * springForce);
            
            
        }
        else
        {
        groundSensed = false;
            if (timeSinceGroundSensed > hangTime)
            {
                crntGravity = Mathf.Lerp(crntGravity, gravityStrength, Time.deltaTime * gravityInterpolation);
                targetVelocityDirection = Vector3.down * crntGravity;
            }
        if(!surfacePredicted)
        targetRot = Quaternion.Lerp(targetRot, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, Time.deltaTime * rotationInterpolation);
        if(timeSinceGroundSensed > hangTime)
        targetRot = Quaternion.Euler(targetRot.eulerAngles.x, transform.rotation.eulerAngles.y, targetRot.eulerAngles.z);
            if(timeSinceGroundSensed > 2f)
        CVCTargetPosition = CVCFlyingOffset;
        }

        if (grounded == false)
        {
            RaycastHit predictHit;
            if (Physics.Raycast(rotationRay.position, transform.forward, out predictHit, predictRotationLength))
            {
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                surfacePredicted = true;
            }
            else { surfacePredicted = false; }
        }
        else { surfacePredicted = false; }

        if(grounded == true && Input.GetButton("Jump"))
        {
            Debug.Log("Press");
            targetVelocityDirection += transform.up * jumpForce;
        }
        
        RotationRay();

        Strafe();

        YTurning();

        ForwardSpeed();


        //Final Rot 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * (rotationInterpolation + Quaternion.Angle(transform.rotation, targetRot) / 5));

        if(crntBoostTime < crntMaxBoostTime)
        {
            crntAcceleration = crntBoost;
        }
        else
        {
            crntAcceleration = maxAcceleration;
        }
        
        targetVelocity = transform.forward * (vertInput) * crntAcceleration;
        targetVelocity += targetVelocityDirection;
       
        //Final Velo
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * velocityInterpolation);
        //Camera
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.InverseLerp(0, CVCNoiseLerpMax, rb.velocity.magnitude);
        CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, CVCTargetPosition, Time.deltaTime * CVCPositionInterpolation);
        //Debug Info
        velMag = rb.velocity.magnitude;
        rotDifferences = Quaternion.Angle(transform.rotation, targetRot);
        forwardVelMag = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);

    }

    private void ForwardSpeed()
    {
        if(vertInput != 0 && crntAcceleration < maxAcceleration)
            crntAcceleration = Mathf.Lerp(crntAcceleration, maxAcceleration, Time.deltaTime * accellerationIncrease);
        if(vertInput == 0)
            crntAcceleration = Mathf.Lerp(crntAcceleration, 0, Time.deltaTime * speedDecrease);
    }

    private void Strafe()
    {
        if (Input.GetAxis("Strafe") != 0)
        {
            targetVelocityDirection += transform.right * Input.GetAxis("Strafe") * Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
        }
    }

    private RaycastHit RotationRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(rotationRay.position, transform.up * -1, out hit, maxSenseHeight))
        {

           Quaternion tempTargetRot = targetRot;
           targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
           if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
            
        }

        return hit;
    }

    private void YTurning()
    {
        float targetTurnSpeed = turnSpeed * horiInput;
        targetTurnSpeed = Mathf.Lerp(oldTurnSpeed, targetTurnSpeed, Time.deltaTime * turnSpeedInterpolation);
        if (horiInput != 0)
        {
            transform.Rotate(transform.up, targetTurnSpeed, Space.World);
        }
        oldTurnSpeed = targetTurnSpeed;
    }

    public void Boost(float speed, float time) {
        crntBoost = speed * boostMultiplier;
        crntBoostTime = 0f;
        crntMaxBoostTime = time;

        rb.velocity = crntBoost * transform.forward;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(positionRay.position, (transform.up * -1) * maxSenseHeight);
        Gizmos.DrawRay(rotationRay.position, (transform.up * -1) * rotSenseHeight);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionRay.position + (transform.up * -1) * hoverHeight, 0.2f);
        Gizmos.color = Color.green;
        if(rb != null)
        Gizmos.DrawRay(transform.position, rb.velocity);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hitPoint, 0.2f);
    }
}
