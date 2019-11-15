using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LandCheckpointHandler : MonoBehaviour
{
    [SerializeField]
    bool checkGround;
    [SerializeField]
    float scale;
    [SerializeField]
    GameObject sphere;
    [SerializeField]
    GameObject overlay;

    public int checkpointNumber = 0;

    public bool firstCheckpoint = false;
    public bool hasPassed;

    public bool isNext;
    public bool isSecond;



    [SerializeField]
    Material nextMat;
    [SerializeField]
    Material secondMat;

    RaceManager raceManager;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMaterial();
    }

    void SetMaterial()
    {
        if (!isNext && !isSecond)
        {
            overlay.GetComponent<Renderer>().enabled = false;
            sphere.GetComponent<Renderer>().enabled = false;
        }
        else if (isNext)
        {
            overlay.GetComponent<Renderer>().enabled = true;
            sphere.GetComponent<Renderer>().enabled = true;
            overlay.GetComponent<Renderer>().material = nextMat;
        }
        else if (isSecond)
        {
            overlay.GetComponent<Renderer>().enabled = true;
            sphere.GetComponent<Renderer>().enabled = true;
            overlay.GetComponent<Renderer>().material = secondMat;
        }
    }

    private void OnValidate()
    {
        if (checkGround == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null)
                {
                    transform.position = hit.point;
                    checkGround = false;
                }
            }
            else if (Physics.Raycast(transform.position, Vector3.up, out hit))
            {
                if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null)
                {
                    transform.position = hit.point;
                    checkGround = false;
                }
            }
        }

        GetComponent<SphereCollider>().radius = scale;
        if(sphere != null)
        sphere.transform.localScale = Vector3.one * scale*2;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<AudioSource>().Play();
        raceManager.CheckpointPassed(checkpointNumber);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }


}
