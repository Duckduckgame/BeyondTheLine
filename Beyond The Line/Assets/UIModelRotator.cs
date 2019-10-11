using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelRotator : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField]
    float moveMultiplier = 1;
    [SerializeField]
    float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos+ new Vector3(0, Mathf.Sin(Time.time), 0.0f) * moveMultiplier;
        transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, rotSpeed));
    }
}
