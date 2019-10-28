using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

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
    float springClamp = 250;
    [SerializeField]
    float upSpringForceClamp = 0.5f;
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
    [SerializeField]
    Vector3 CVCSharpRotOffset;
    Vector3 CVCTargetPosition;
    [SerializeField]
    float CVCPositionInterpolation = 1;
    [SerializeField]
    float CVCNoiseLerpMax = 250;
    Vector3 CVCOffset;
    [SerializeField]
    bool usingPS4Controller = false;
    RaceManager raceManager;
    PostProcessVolume PPV;

    [SerializeField]
    float velMag;
    [SerializeField]
    float forwardVelMag;
    [SerializeField]
    float rotDifferences;

    [SerializeField]
    Transform myTransform;
    Transform tran;


    Vector3 lerpPoint;

    Vector3 spawnPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CVCOffset = CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        spawnPos = transform.position;
        PPV = FindObjectOfType<PostProcessVolume>();
        raceManager = FindObjectOfType<RaceManager>();

        tran = myTransform;


        
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
            raceManager.PlayerRespawn(this.gameObject);

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
            CVCTargetPosition = CVCOffset;
            if (hit.distance > hoverHeight) { grounded = false; }
            else {
                grounded = true;
                crntGravity = 0;
            }
            Vector3 targetPos = hit.point + (transform.rotation.normalized * new Vector3(0, hoverHeight, 0));
            hitPoint = hit.point;

            float springCompression = ((Mathf.InverseLerp(0.2f, hoverHeight * 2, Vector3.Distance(transform.position, hit.point)) * 2) - 1) * -1;
            float usedSpringforce = springForce;
            Vector3 springVelocity = transform.up * springCompression * usedSpringforce;
            if (springCompression > 0) {springVelocity = Vector3.ClampMagnitude(springVelocity, upSpringForceClamp); Debug.Log(springVelocity.magnitude); }
            
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
        //Debug Info
        velMag = rb.velocity.magnitude;
        rotDifferences = Quaternion.Angle(transform.rotation, targetRot);
        forwardVelMag = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
        //Camera
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.InverseLerp(0, CVCNoiseLerpMax, rb.velocity.magnitude);
        CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Slerp(CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, CVCTargetPosition, Time.deltaTime * CVCPositionInterpolation * rotDifferences);
        float distortionLerp = Mathf.InverseLerp(maxAcceleration, 1000, rb.velocity.magnitude);
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
