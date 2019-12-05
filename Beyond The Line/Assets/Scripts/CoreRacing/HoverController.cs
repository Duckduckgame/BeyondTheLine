using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using System;

[RequireComponent(typeof(Rigidbody))]
public class HoverController : MonoBehaviour
{

    Rigidbody rb;
    float horiInput;
    float vertInput;
    enum FlightType { Terrain, Track };

    [Header("Speed")]
    public float accellerationIncrease = 10;
    public float maxAcceleration = 300;
    [SerializeField]
    float speedDecrease = 1;
    public float crntAcceleration = 0;
    public float usedAcceleration = 0;
    [SerializeField]
    float velocityInterpolation = 1;
    [SerializeField]
    float jumpForce = 5;
    [SerializeField]
    float flyingSpeedMultiplier = 0.5f;
    [HideInInspector]
    public Vector3 targetVelocity = Vector3.zero;
    [SerializeField]
    float collisionSpeedDecrease;
    [SerializeField]
    float collisionRayLength;

    [Header("Boost")]
    public float boostForce = 20;
    [SerializeField]
    float maxBoost = 200;
    [SerializeField]
    float boostFillLerp;
    [SerializeField]
    float untilFillBoost = 3f;
    float timeSinceBoost = 0;
    public float maxBoostAmount = 5;
    public float crntBoostAmount;
    [SerializeField]
    float crntBoost;
    float crntBoostTime;
    float crntMaxBoostTime;
    float crntBoostPadPower;
    float crntBoostPadTime;
    float boostPadTimer;
    float targetBoostPadPower;


    [Header("Rotations")]
    public float turnSpeed = 2;
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
    float turnMultiplier = 1; 
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
    float damperForce = 15;
    public float timeSinceGroundSensed;
    Vector3 hitPoint;
    float springCompression;
    float oldCompression;

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
    float CVCNoiseMaxAmplitude = 250;
    
    public float minNoiseSpeed;
    public float maxNoiseSpeed;
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
    ParticleSystem speedPS;
    [SerializeField]
    ParticleSystem colPS;
    GameObject colGO;
    int PSEmmissionAmount = 0;
    public bool boostin;
    [SerializeField]
    Material emmissMat;
    float emissStrength;
    float emissLerp;
    public float boostEmissLerp;
    Vector3 lerpPoint;
    public bool IsBoosting;
    Vector3 spawnPos;
    HoverAudioManager hoverAudioManager;

    float noiseBooster = 0;
    #region analytics
    public float strafeTime_anal = 0;
    public float boostTime_anal = 0;
    public float xHeldTime_anal = 0;
    public float maxSpeedTime_anal = 0;
    public float wallBashAmount_anal = 0;
    public float wallBashTime_anal = 0;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CVCOffset = CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        spawnPos = transform.position;
        PPV = FindObjectOfType<PostProcessVolume>();
        raceManager = FindObjectOfType<RaceManager>();
        crntFlightType = FlightType.Terrain;
        crntBoostAmount = maxBoostAmount;
        hoverAudioManager = GetComponent<HoverAudioManager>();
        if (raceManager.crntType == RaceManager.RaceType.Land) noiseBooster = 0.2f;
    }

    void Update()
    {
        horiInput = Input.GetAxis("Horizontal");
        float R2 = Input.GetAxis("PS4_R2");
        float L2 = Input.GetAxis("PS4_L2");
        R2 = (R2 + 1) * 0.5f;
        L2 = ((L2 + 1) * 0.5f) * -1;

        vertInput = Mathf.Clamp(Input.GetAxis("Vertical") + (R2 + L2),-1,1);

        if (Input.GetButtonDown("PS4 Select"))
            raceManager.PlayerRespawn(this.gameObject);

        if (oldFlightType != crntFlightType)
        {
            ChangeFlightType(crntFlightType);
            oldFlightType = crntFlightType;
        }


        timeSinceGroundSensed += Time.deltaTime;
        crntBoostTime += Time.deltaTime;
        boostPadTimer += Time.deltaTime;
        oldFlightType = crntFlightType;
        emmissMat.SetFloat("Vector1_8105018E", emissStrength);
        emmissMat.SetFloat("Vector1_F763066C", boostEmissLerp);

        emissLerp += boostEmissLerp;

        if (Input.GetButton("Jump")) xHeldTime_anal += Time.deltaTime;
        if (crntAcceleration > maxAcceleration - 2) maxSpeedTime_anal += Time.deltaTime;

        timeSinceBoost += Time.deltaTime;
        if (Input.GetButton("Jump") && crntBoostAmount > 0.1f) 
        {
            if(crntBoost < maxBoost)
            crntBoost += boostForce * Time.deltaTime;
            emissLerp += 500f;
            boostEmissLerp = Mathf.Lerp(boostEmissLerp, 1, Time.deltaTime * 20f);
            CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.4f + noiseBooster;
            crntBoostAmount -= Time.deltaTime;
            timeSinceBoost = 0;
            turnMultiplier = 0.5f;
            boostTime_anal += Time.deltaTime;
            IsBoosting = true;
        }
        if (Input.GetButtonUp("Jump") || crntBoostAmount < 0.1f)
        {
            crntBoost = 0;
            emissLerp -= 0.5f;
            boostEmissLerp = 0;
            CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.2f + noiseBooster;
            turnMultiplier = 1;
            IsBoosting = false;
        }
        if (!Input.GetButton("Jump") && crntBoostAmount < maxBoostAmount && untilFillBoost < timeSinceBoost)
        {
            crntBoostAmount += Time.deltaTime;
            boostEmissLerp = 0;
            CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.2f + noiseBooster;
            turnMultiplier = 1;
            IsBoosting = false;
        }
    }

    private void FixedUpdate()
    {
        targetVelocityDirection = Vector3.zero;
        oldCompression = springCompression;
        RaycastHit hit;
        if (Physics.Raycast(positionRay.position, transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null) crntFlightType = FlightType.Terrain;
            if (hit.collider.gameObject.GetComponent<MeshCollider>() != null) crntFlightType = FlightType.Track;
            timeSinceGroundSensed = 0;
            groundSensed = true;
            if (Input.GetAxis("R_Vertical") == 0)
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

        if (!grounded && timeSinceGroundSensed > 0.2f && raceManager != null)
        {
            raceManager.shipGrounded = false;
        }
        else if (raceManager != null) { raceManager.shipGrounded = true; }

        RotationRay();

        Strafe();

        YTurning();

        ForwardSpeed();



        //Final Rot 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * (rotationInterpolation + Quaternion.Angle(transform.rotation, targetRot)));

        if (crntBoostTime < crntMaxBoostTime)
        {
            //crntAcceleration = crntBoost;
            boostin = true;
        }
        else
        {
            boostin = false;
        }
        crntBoostPadPower = Mathf.Lerp(crntBoostPadPower, targetBoostPadPower, Time.deltaTime * 5);
        usedAcceleration = crntAcceleration + crntBoostPadPower;
        usedAcceleration = Mathf.Clamp(usedAcceleration, 0, 600);
        targetVelocity = transform.forward * (vertInput) * usedAcceleration;
        targetVelocity += targetVelocityDirection;

        //Final Velo
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * velocityInterpolation);
        //Debug Info
        velMag = rb.velocity.magnitude;
        rotDifferences = Quaternion.Angle(transform.rotation, targetRot);
        forwardVelMag = usedAcceleration;

        CameraMovements();
        PPVChanges();

        PSEmmissionAmount = Mathf.RoundToInt(Mathf.InverseLerp(minNoiseSpeed, maxNoiseSpeed, usedAcceleration) * 20);
        
        speedPS.Emit(PSEmmissionAmount);
        
    }

    private void PPVChanges()
    {
        float distortionLerp = Mathf.InverseLerp(minNoiseSpeed, maxAcceleration*3, usedAcceleration);
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

        emissLerp = (Mathf.InverseLerp(0, maxAcceleration, crntAcceleration) - 0.5f);
        emissLerp = Mathf.Clamp(emissLerp, 0, 1);
        emissStrength = emissLerp * 15;
    }

    private void CameraMovements()
    {

        float cameraRotLerp = Mathf.InverseLerp(0, 10, rotDifferences);
        if (timeSinceGroundSensed > hangTime && raceManager.crntType == RaceManager.RaceType.Land)
        {
            CVCTargetPosition = CVCFlyingOffset; Debug.Log("is land");
        }
        else {
            CVCTargetPosition = Vector3.Slerp(CVCOffset, CVCSharpRotOffset, cameraRotLerp);
        }


        
        float wiggleMultiplier = Mathf.InverseLerp(minNoiseSpeed, maxNoiseSpeed, usedAcceleration);
        CVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = CVCNoiseMaxAmplitude * wiggleMultiplier;
        CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Slerp(CVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, CVCTargetPosition, Time.deltaTime * CVCPositionInterpolation);
        CVC.m_Lens.FieldOfView = Mathf.Lerp(40, 90, Mathf.InverseLerp(minNoiseSpeed/2, maxNoiseSpeed, usedAcceleration));

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

    }

    private void ForwardSpeed()
    {


        if (vertInput > 0 && crntAcceleration < maxAcceleration)
        {

            float usedIncrease = accellerationIncrease * (Mathf.InverseLerp(0, maxAcceleration, crntAcceleration) * -1 + 1);
            crntAcceleration += usedIncrease * Time.deltaTime;
            if (crntAcceleration < 0) crntAcceleration = 0;
            emissLerp = Mathf.Clamp(emissLerp, 0.3f, 1);
        }else if (vertInput == 0){

            crntAcceleration -= speedDecrease * Time.deltaTime;
            crntAcceleration = Mathf.Clamp(crntAcceleration, 0, maxAcceleration);
            emissLerp = Mathf.Clamp(emissLerp, 0, 0.5f);
        }
        else if(vertInput < 0)
        {
            crntAcceleration -= speedDecrease * Time.deltaTime;
            crntAcceleration = Mathf.Clamp(crntAcceleration, -speedDecrease*3, maxAcceleration);
        }

        if (timeSinceGroundSensed > 0.05f)
            crntAcceleration = Mathf.Lerp(crntAcceleration, crntAcceleration * flyingSpeedMultiplier, Time.deltaTime * gravityInterpolation);

        if(grounded && crntAcceleration < rb.velocity.magnitude && rb.velocity.magnitude < maxAcceleration)
            crntAcceleration = rb.velocity.magnitude;

        if(crntAcceleration > maxAcceleration && crntBoost < 0.1f)
        {
            crntAcceleration -= speedDecrease * Time.deltaTime;
        }

        crntAcceleration += crntBoost * Time.deltaTime;
    }

    private void Strafe()
    {
        if (Input.GetAxis("Strafe") != 0)
        {
            targetVelocityDirection += transform.right * Input.GetAxis("Strafe") * Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
            strafeTime_anal += Time.deltaTime;
        }
    }

    private RaycastHit RotationRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(rotationRayF.position, rotationRayF.transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.GetComponent<trackManager>() != null || raceManager.crntType == RaceManager.RaceType.Land || hit.collider.GetComponent<TerrainCollider>() != null)
            {
                Quaternion tempTargetRot = targetRot;
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
            }
        }
        else if (Physics.Raycast(rotationRayB.position, rotationRayB.transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.GetComponent<trackManager>() != null || raceManager.crntType == RaceManager.RaceType.Land || hit.collider.GetComponent<TerrainCollider>() != null)
            {
                Quaternion tempTargetRot = targetRot;
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
            }
        }
        else if (Physics.Raycast(rotationRayR.position, rotationRayR.transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.GetComponent<trackManager>() != null || raceManager.crntType == RaceManager.RaceType.Land || hit.collider.GetComponent<TerrainCollider>() != null)
            {
                Quaternion tempTargetRot = targetRot;
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
            }
        }
        else if (Physics.Raycast(rotationRayL.position, rotationRayL.transform.up * -1, out hit, maxSenseHeight))
        {
            if (hit.collider.GetComponent<trackManager>() != null || raceManager.crntType == RaceManager.RaceType.Land || hit.collider.GetComponent<TerrainCollider>() != null)
            {
                Quaternion tempTargetRot = targetRot;
                targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                if (Quaternion.Angle(transform.rotation, targetRot) > 45) { targetRot = tempTargetRot; }
            }
        }

        return hit;
    }

    private void YTurning()
    {
        float usedTurnSpeed = turnSpeed * ((Mathf.Clamp(Mathf.InverseLerp(0, maxAcceleration*2, usedAcceleration),0,1) * -1) + 1);
        usedTurnSpeed = Mathf.Clamp(usedTurnSpeed, minTurnSpeed, turnSpeed);

        float targetTurnSpeed = usedTurnSpeed * horiInput * turnMultiplier;
        targetTurnSpeed = Mathf.Lerp(oldTurnSpeed, targetTurnSpeed, Time.deltaTime * turnSpeedInterpolation);
        if (horiInput != 0)
        {
            transform.Rotate(transform.up, targetTurnSpeed, Space.World);
        }
        oldTurnSpeed = targetTurnSpeed;
    }

    public void BoostPad(float speed, float time)
    {
        StartCoroutine(BoostPower(speed, time));
        
    }

    IEnumerator BoostPower(float power, float time)
    {
        targetBoostPadPower += power;
        float minturn = minTurnSpeed;
        minTurnSpeed = turnSpeed;
        Debug.Log("boost power on");
        yield return new WaitForSeconds(time);
        targetBoostPadPower -= power;
        minTurnSpeed = minturn;
        Debug.Log("boost power off");
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
            damperForce = 15;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<TerrainCollider>() == null)
        {
            hoverAudioManager.Bump();

            if (crntAcceleration < Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z))
            {
                crntAcceleration = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z);
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, collisionRayLength))
        {
            hoverAudioManager.Bump();

            wallBashAmount_anal++;
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
        }
        else if (Physics.Raycast(transform.position, transform.right * -1, out hit, collisionRayLength)){
            hoverAudioManager.Bump();

            wallBashAmount_anal++;
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
        }
        else if (Physics.Raycast(transform.position,transform.forward, out hit, collisionRayLength*3))
        {
            hoverAudioManager.Bump();

            wallBashAmount_anal++;
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, collisionRayLength))
        {
            crntAcceleration -= 2;
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
            Debug.Log("bashing" + crntAcceleration);
            wallBashTime_anal += Time.deltaTime;
        }
        else if (Physics.Raycast(transform.position, transform.right * -1, out hit, collisionRayLength))
        {
            crntAcceleration -= 2;
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
            Debug.Log("bashing" + crntAcceleration);
            wallBashTime_anal += Time.deltaTime;
        }
        else if (Physics.Raycast(transform.position, transform.forward, out hit, collisionRayLength * 3))
        {
            hoverAudioManager.Bump();
            StartCoroutine(CollisionParticleSpawn(hit.point, Quaternion.FromToRotation(colPS.gameObject.transform.forward, hit.normal)));
            Debug.Log("bashing" + crntAcceleration);
            wallBashTime_anal += Time.deltaTime;
        }
    }

    IEnumerator CollisionParticleSpawn(Vector3 pos, Quaternion rotation)
    {
        GameObject pGO;
        pGO = Instantiate(colPS.gameObject, pos, rotation);
        yield return new WaitForSeconds(0.5f);
        Destroy(pGO);
        yield return null;
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.right * collisionRayLength);
        Gizmos.DrawRay(transform.position, -transform.right * collisionRayLength);
        Gizmos.DrawRay(transform.position, transform.forward * collisionRayLength*3);
    }

}