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
    float crntAcceleration = 0;
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
    float springClamp = 250;
    [SerializeField]
    float upSpringForceClamp = 0.5f;
    [SerializeField]
    float damperForce = 50;
    [SerializeField]
    float timeSinceGroundSensed;
    Vector3 hitPoint;
    float springCompression;
    float oldCompression;

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

    AudioSource audioSource;
    float audioTargetVolume;
    float audioTargetPitch;

    Vector3 lerpPoint;

    Vector3 spawnPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CVCOffset = CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        spawnPos = transform.position;
        PPV = FindObjectOfType<PostProcessVolume>();
        raceManager = FindObjectOfType<RaceManager>();
        audioSource = GetComponent<AudioSource>();
        crntFlightType = FlightType.Track;

        
    }

    void Update()
    {
        horiInput = Input.GetAxis("Horizontal");
        float R2 = Input.GetAxis("PS4_R2");
        float L2 = Input.GetAxis("PS4_L2");
        R2 = (R2 + 1) * 0.5f;
        L2 = ((L2 + 1) * 0.5f) * -1;

        vertInput = Input.GetAxis("Vertical") + (R2 + L2);

        if (Input.GetKeyDown(KeyCode.T))
            raceManager.PlayerRespawn(this.gameObject);

        if(oldFlightType != crntFlightType)
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
            /*if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null) crntFlightType = FlightType.Terrain;
            if (hit.collider.gameObject.GetComponent<MeshCollider>() != null) crntFlightType = FlightType.Track;*/
            timeSinceGroundSensed = 0;
            groundSensed = true;
            CVCTargetPosition = CVCOffset;
            if (hit.distance > hoverHeight) { grounded = false; }
            else {
                grounded = true;
                crntGravity = 0;
            }
            Vector3 targetPos = hit.point + (transform.rotation.normalized * new Vector3(0, hoverHeight, 0));
            hitPoint = hit.point;

            springCompression = ((Mathf.InverseLerp(0.2f, hoverHeight * 2, Vector3.Distance(transform.position, hit.point)) * 2) - 1) * -1;
            float damper = (((springCompression + oldCompression) / Time.deltaTime) * damperForce);
            float usedSpringforce = springForce;
            
            Vector3 springVelocity = transform.up * springCompression * (usedSpringforce + damper);
            
            targetVelocityDirection += Vector3.ClampMagnitude(springVelocity, springClamp);
            
            
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
            if(timeSinceGroundSensed > hangTime)
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
        /*
        if (!grounded && timeSinceGroundSensed > 0.2f)
        {
            raceManager.shipGrounded = false;
            audioTargetVolume -= 0.2f;
            audioTargetPitch += 0.5f;
        }
        else { raceManager.shipGrounded = true; }*/

        RotationRay();

        Strafe();

        YTurning();

        ForwardSpeed();

        AudioLerp();


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
        //Debug Info
        velMag = rb.velocity.magnitude;
        rotDifferences = Quaternion.Angle(transform.rotation, targetRot);
        forwardVelMag = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
        //Camera
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.InverseLerp(0, CVCNoiseLerpMax, rb.velocity.magnitude);
        CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Slerp(CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, CVCTargetPosition, Time.deltaTime * CVCPositionInterpolation * rotDifferences);

        float distortionLerp = Mathf.InverseLerp(maxAcceleration, distortVelocityMax, rb.velocity.magnitude);
        LensDistortion lensDistortion;
        ChromaticAberration chromaticAberration;
        if(PPV.profile.TryGetSettings(out lensDistortion))
        {
            lensDistortion.intensity.value = -100 * distortionLerp;
        }
        if (PPV.profile.TryGetSettings(out chromaticAberration))
        {
            chromaticAberration.intensity.value = 1 * distortionLerp;
        }


    }

    private void AudioLerp()
    {
        audioSource.volume = Mathf.Lerp(audioSource.volume, audioTargetVolume, Time.deltaTime * 1);
        audioSource.pitch = Mathf.Lerp(audioSource.pitch, audioTargetPitch, Time.deltaTime * 1);
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
            audioTargetPitch += 0.2f;
        }
        oldTurnSpeed = targetTurnSpeed;
    }

    public void Boost(float speed, float time) {
        crntBoost = speed * boostMultiplier;
        crntBoostTime = 0f;
        crntMaxBoostTime = time;

        rb.velocity = crntBoost * transform.forward;
    }

    void ChangeFlightType(FlightType newFlightType)
    {
        if(newFlightType == FlightType.Terrain)
        {
            springForce = 10;
            springClamp = 2000;
            upSpringForceClamp = 2000;
            damperForce = 50;
        }
        if(newFlightType == FlightType.Track)
        {
            springForce = 250;
            springClamp = 200;
            upSpringForceClamp = 40;
            damperForce = 0;
        }
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
