using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CarController : MonoBehaviour
{
    [SerializeField]
    Transform[] wheelPositions;

    [SerializeField]
    float wheelRadius;
    [SerializeField]
    public float springMaxMin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity += transform.forward * Input.GetAxis("Horizontal") * 100;
    }

    private void OnDrawGizmos()
    {
        foreach(Transform pos in wheelPositions)
        {
            Gizmos.DrawSphere(pos.position, wheelRadius);
            
        }
    }
}
