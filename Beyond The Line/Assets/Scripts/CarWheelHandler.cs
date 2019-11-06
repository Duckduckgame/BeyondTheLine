using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelHandler : MonoBehaviour
{
    CarController carController;
    GameObject car;
    Vector3 vehicleToWheelPos;

    Vector3 offset;

    Vector3 springRestingPosition;
    Vector3 springCompressedPosition;
    Vector3 springStretchedPosition;
    float springRestingLength;
    float springMaxMin;

    float springTension;


    Vector3 targetPos;

    Rigidbody carRB;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponentInParent<CarController>();
        offset = transform.localPosition;
        car = carController.gameObject;
        carRB = car.GetComponent<Rigidbody>();
        targetPos = transform.position;
        vehicleToWheelPos = car.transform.position + new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        springRestingLength = Vector3.Distance(transform.position, vehicleToWheelPos);

    }

    // Update is called once per frame
    void Update()
    {
        vehicleToWheelPos = car.transform.position + new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

        springMaxMin = carController.springMaxMin;
        springRestingPosition = car.transform.position + (car.transform.rotation * offset);
        springCompressedPosition = springRestingPosition + (car.transform.rotation * new Vector3(0, springMaxMin, 0));
        springStretchedPosition = springRestingPosition + (car.transform.rotation * new Vector3(0, -springMaxMin, 0));



        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime );
        transform.rotation = car.transform.rotation;
    }

    private void FixedUpdate()
    {

        targetPos = springRestingPosition;
        float distanceToStretched = Vector3.Distance(vehicleToWheelPos, springStretchedPosition);
        RaycastHit hit;
        if (Physics.Raycast(vehicleToWheelPos, -transform.up, out hit, distanceToStretched))
        {
            Debug.DrawLine(transform.position, hit.point);
            targetPos = hit.point;
            springTension = Mathf.InverseLerp(Vector3.Distance(vehicleToWheelPos, springStretchedPosition), Vector3.Distance(vehicleToWheelPos, springCompressedPosition), Vector3.Distance(transform.position, vehicleToWheelPos));

            //carRB.AddForceAtPosition((transform.up * 10) * springTension, vehicleToWheelPos);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(springCompressedPosition, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(springStretchedPosition, 0.1f);


    }
}
