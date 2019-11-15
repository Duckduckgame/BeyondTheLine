using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;
using System;

[RequireComponent(typeof(Rigidbody))]
public class HoverController : MonoBehaviour
{

    Rigidbody rb;
    float horiInput;
    float vertInput;
    enum FlightType { Terrain, Track };

    [Header("Speed")]
    [SerializeField]
    float accellerationIncrease = 10;
    [SerializeField]
    float maxAcceleration = 300;
    [SerializeField]
    float speedDecrease = 1;
    public float crntAcceleration = 0;
    [SerializeField]
    float velocityInterpolation = 1;
    [SerializeField]
    float jumpForce = 5;
    [SerializeField]
    float flyingSpeedMultiplier = 0.5f;
    [HideInInspector]
    public Vector3 targetVelocity = Vector3.zero;

    [Header("Boost")]
    [SerializeField]
    float boostMultiplier = 1;
    float crntBoost;
    float crntBoostTime;
    float crntMaxBoostTime;


    [Header("Rotations")]
    [SerializeField]
    float turnSpeed = 2;
    [SerializeField]
    float minTurnSpeed;
    [SerializeField]
    float turnSpeedInterpolation = 4;
    [HideInInspector]
    public Quaternion targetRot;
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
    FlightType crntFlightType;
    FlightType oldFlightType;
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
    float springForce = 10;
    [SerializeField]
    float springClamp = 100;
    [SerializeField]
    float upSpringForceClamp = 0.5f;
    [SerializeField]
    float damperForce = 15;
    public float timeSinceGroundSensed;
    Vector3 hitPoint;
    float springCompression;
    float oldCompression;

    [Header("Strafing")]
    [SerializeField]
    float strafeSpeed = 250;

    [Header("children")]
    [SerializeField]
    Transform rotationRayF;
    [SerializeField]
    Transform rotationRayB;
    [SerializeField]
    Transform rotationRayR;
    [SerializeField]
    Transform rotationRayL;
    [SerializeField]
    Transform positionRay;

    [Header("Misc")]
    [SerializeField]
    CinemachineVirtualCamera CVC;
    [SerializeField]
    Vector3 CVCFlyingOffset;
    [SerializeField]
    Vector3 CVCSharpRotOffset;
    Vector3 CVCTargetPosition;
    [SerializeField]
    float CVCPositionInterpolation = 1;
    [SerializeField]
    float CVCNoiseLerpMax = 250;
    Vector3 CVCOffset;
    RaceManager raceManager;
    PostProcessVolume PPV;
    [SerializeField]
    float distortVelocityMax = 1000;

    [SerializeField]
    float velMag;
    [SerializeField]
    float forwardVelMag;
    [SerializeField]
    float rotDifferences;

    [SerializeField]
    AudioSource hoverAudioSource;
    [SerializeField]
    AudioSource crashAudioSource;
    float audioTargetVolume;
    float audioTargetPitch;

    public bool boostin;

    Vector3 lerpPoint;

    Vector3 spawnPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CVCOffset = CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        spawnPos = transform.position;
        PPV = FindObjectOfType<PostProcessVolume>();
        raceManager = FindObjectOfType<RaceManager>();
        hoverAudioSource = GetComponent<AudioSource>();
        crntFlightType = FlightType.Terrain;


    }

    void Update()
    {
        horiInput = Input.GetAxis("Horizontal");
        float R2 = Input.GetAxis("PS4_R2");
        float L2 = Input.GetAxis("PS4_L2");
        R2 = (R2 + 1) * 0.5f;
        L2 = ((L2 + 1) * 0.5f) * -1;

        vertInput = Mathf.Clamp(Input.GetAxis("Vertical") + (R2 + L2),-1,1);

        if (Input.GetKeyDown(KeyCode.T))
            raceManager.PlayerRespawn(this.gameObject);

        if (oldFlightType != crntFlightType)
        {
            ChangeFlightType(crntFlightType);
            oldFlightType = crntFlightType;
        }


        timeSinceGroundSensed += Time.deltaTime;
        crntBoostTime += Time.deltaTime;

        oldFlightType = crntFlightType;
    }

    private void FixedUpdate()
    {
        targetVelocityDirection = Vector3.zero;
        audioTargetPitch = 1;
        audioTargetVolume = 1;
        oldCompression = springCompression;
        RaycastHit hit;
        if (Physics.Raycast(positionRay.position, transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null) crntFlightType = FlightType.Terrain;
            if (hit.collider.gameObject.GetComponent<MeshCollider>() != null) crntFlightType = FlightType.Track;
            timeSinceGroundSensed = 0;
            groundSensed = true;
            CVCTargetPosition = CVCOffset;
            if (hit.distance > hoverHeight) { grounded = false; }
            else
            {
                grounded = true;
                crntGravity = 0;
            }
            Vector3 targetPos = hit.point + (transform.rotation.normalized * new Vector3(0, hoverHeight, 0));
            hitPoint = hit.point;

            SpringCalculations();


        }
        else
        {
            groundSensed = false;
            if (timeSinceGroundSensed > hangTime)
            {
                crntGravity = Mathf.Lerp(crntGravity, gravityStrength, Time.deltaTime * gravityInterpolation);
                Vector3 gravityVector = Vector3.down * crntGravity;
                gravityVector = Vector3.ClampMagnitude(gravityVector, 400);
                targetVelocityDirection = gravityVector;
            }
            if (!surfacePredicted)
                targetRot = Quaternion.Lerp(targetRot, Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation, Time.deltaTime * rotationInterpolation);
            if (timeSinceGroundSensed > hangTime)
                targetRot = Quaternion.Euler(targetRot.eulerAngles.x, transform.rotation.eulerAngles.y, targetRot.eulerAngles.z);
            if (timeSinceGroundSensed > hangTime)
                CVCTargetPosition = CVCFlyingOffset;
        }

        if (grounded == false)
        {
            RaycastHit predictHit;
            if (Physics.Raycast(rotationRayF.position, transform.forward, out predictHit, predictRotationLength))
            {
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                surfacePredicted = true;
            }
            else { surfacePredicted = false; }
        }
        else { surfacePredicted = false; }

        if (grounded == true && Input.GetButton("Jump"))
        {
            Debug.Log("Press");
            targetVelocityDirection += transform.up * jumpForce;
        }

        if (!grounded && timeSinceGroundSensed > 0.2f && raceManager != null)
        {
            raceManager.shipGrounded = false;
            audioTargetVolume -= 0.2f;
            audioTargetPitch += 0.5f;
        }
        else if (raceManager != null) { raceManager.shipGrounded = true; }

        RotationRay();

        Strafe();

        YTurning();

        ForwardSpeed();

        AudioLerp();

        XBoost();


        //Final Rot 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * (rotationInterpolation + Quaternion.Angle(transform.rotation, targetRot) / 5));
        
        if (crntBoostTime < crntMaxBoostTime)
        {
            //crntAcceleration = crntBoost;
            boostin = true;
        }else
        {
            boostin = false;
        }


        targetVelocity = transform.forward * (vertInput) * crntAcceleration;
        targetVelocity += targetVelocityDirection;

        //Final Velo
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * velocityInterpolation);
        //Debug Info
        velMag = rb.velocity.magnitude;
        rotDifferences = Quaternion.Angle(transform.rotation, targetRot);
        forwardVelMag = crntAcceleration;
        //Camera
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.InverseLerp(0, CVCNoiseLerpMax, rb.velocity.magnitude);
        CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Slerp(CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, CVCTargetPosition, Time.deltaTime * CVCPositionInterpolation * rotDifferences);

        float distortionLerp = Mathf.InverseLerp(maxAcceleration, distortVelocityMax, rb.velocity.magnitude);
        LensDistortion lensDistortion;
        ChromaticAberration chromaticAberration;
        if (PPV.profile.TryGetSettings(out lensDistortion))
        {
            lensDistortion.intensity.value = -100 * distortionLerp;
        }
        if (PPV.profile.TryGetSettings(out chromaticAberration))
        {
            chromaticAberration.intensity.value = 1 * distortionLerp;
        }


    }

    void XBoost()
    {
        if (Input.GetButton("Jump"))
        {
            crntAcceleration = maxAcceleration;
        }
    }

    void SpringCalculations()
    {
        springCompression = ((Mathf.InverseLerp(0.2f, hoverHeight * 2, Vector3.Distance(transform.position, hitPoint)) * 2) - 1) * -1;

        Vector3 upVelocity = rb.velocity;
        Vector3 n1 = upVelocity * (damperForce * damperForce) * Time.deltaTime;
        float n2 = 1 + damperForce * Time.deltaTime;
        Vector3 force = n1 / (n2 * n2);


        Vector3 springVelocity = transform.up * force.magnitude * springCompression;
        targetVelocityDirection += Vector3.ClampMagnitude(springVelocity, springClamp);
    }
    private void AudioLerp()
    {
        hoverAudioSource.volume = Mathf.Lerp(hoverAudioSource.volume, audioTargetVolume, Time.deltaTime * 1);
        hoverAudioSource.pitch = Mathf.Lerp(hoverAudioSource.pitch, audioTargetPitch, Time.deltaTime * 1);
    }

    private void ForwardSpeed()
    { 

    
        if (vertInput > 0 && crntAcceleration < maxAcceleration)
        {
            if (crntAcceleration < 100)
                crntAcceleration = 100;

            crntAcceleration += accellerationIncrease * Time.deltaTime;
            
        }else if (vertInput == 0){
            crntAcceleration = Mathf.Lerp(crntAcceleration, 0, Time.deltaTime * speedDecrease);
        }else if(vertInput < 0)
        {
            crntAcceleration = Mathf.Clamp(crntAcceleration,0, maxAcceleration / 5);
        }

        if (timeSinceGroundSensed > 0.05f)
            crntAcceleration = Mathf.Lerp(crntAcceleration, crntAcceleration * flyingSpeedMultiplier, Time.deltaTime * gravityInterpolation);

        if(grounded && crntAcceleration < rb.velocity.magnitude)
            crntAcceleration = rb.velocity.magnitude;

        if(boostin)
            crntAcceleration = crntBoost + maxAcceleration;
        if (!boostin)
            crntAcceleration = Mathf.Clamp(crntAcceleration, -maxAcceleration, maxAcceleration);
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
        if (Physics.Raycast(rotationRayF.position, rotationRayF.transform.up * -1, out hit, maxSenseHeight))
        {

            Quaternion tempTargetRot = targetRot;
            targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }

        }
        else if (Physics.Raycast(rotationRayB.position, rotationRayB.transform.up * -1, out hit, maxSenseHeight))
        {

            Quaternion tempTargetRot = targetRot;
            targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
        }
        else if (Physics.Raycast(rotationRayR.position, rotationRayR.transform.up * -1, out hit, maxSenseHeight))
        {

            Quaternion tempTargetRot = targetRot;
            targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
        }
        else if (Physics.Raycast(rotationRayL.position, rotationRayL.transform.up * -1, out hit, maxSenseHeight))
        {

            Quaternion tempTargetRot = targetRot;
            targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
        }

        return hit;
    }

    private void YTurning()
    {
        float usedTurnSpeed = turnSpeed * ((Mathf.Clamp(Mathf.InverseLerp(maxAcceleration/4, maxAcceleration, crntAcceleration),0,1) * -1) + 1);
        usedTurnSpeed = Mathf.Clamp(usedTurnSpeed, minTurnSpeed, turnSpeed);

        float targetTurnSpeed = usedTurnSpeed * horiInput;
        targetTurnSpeed = Mathf.Lerp(oldTurnSpeed, targetTurnSpeed, Time.deltaTime * turnSpeedInterpolation);
        if (horiInput != 0)
        {
            transform.Rotate(transform.up, targetTurnSpeed, Space.World);
            audioTargetPitch += 0.2f;
        }
        oldTurnSpeed = targetTurnSpeed;
    }

    public void Boost(float speed, float time)
    {
        crntBoost = speed * boostMultiplier;
        crntBoostTime = 0f;
        crntMaxBoostTime = time;

        crntAcceleration = crntBoost+ maxAcceleration; 
    }

    void ChangeFlightType(FlightType newFlightType)
    {
        if (newFlightType == FlightType.Terrain)
        {
            springForce = 10;
            springClamp = 100;
            damperForce = 15;
        }
        if (newFlightType == FlightType.Track)
        {
            springForce = 10;
            springClamp = 100;
            upSpringForceClamp = 40;
            damperForce = 15;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<TerrainCollider>() == null)
        {   
            if(crashAudioSource)
            crashAudioSource.Play();

            if (crntAcceleration > Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z))
            {
                crntAcceleration = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(positionRay.position, (transform.up * -1) * maxSenseHeight);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(rotationRayF.position, (rotationRayF.transform.up * -1) * rotSenseHeight);
        Gizmos.DrawRay(rotationRayB.position, (rotationRayB.transform.up * -1) * rotSenseHeight);
        Gizmos.DrawRay(rotationRayR.position, (rotationRayR.transform.up * -1) * rotSenseHeight);
        Gizmos.DrawRay(rotationRayL.position, (rotationRayL.transform.up * -1) * rotSenseHeight);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionRay.position + (transform.up * -1) * hoverHeight, 0.2f);
        Gizmos.color = Color.green;
        if (rb != null)
            Gizmos.DrawRay(transform.position, rb.velocity);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hitPoint, 0.2f);
    }

}