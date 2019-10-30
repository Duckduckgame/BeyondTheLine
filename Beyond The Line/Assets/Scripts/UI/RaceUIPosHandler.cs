using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class RaceUIPosHandler : MonoBehaviour
{
    [SerializeField]
    float circleRadius = 1f;

    RaceSelectHandler raceSelectHandler;

    GameObject[] trackOBJs;

    [HideInInspector]
    public Vector3[] positions;

    private void OnEnable()
    {
        SetTrackOBJs();
        PlaceTracks();
    }

    private void OnValidate()
    {
        SetTrackOBJs();
        PlaceTracks();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            SetTrackOBJs();
            PlaceTracks();
        }
    }

    void SetTrackOBJs()
    { 
        raceSelectHandler = GetComponent<RaceSelectHandler>();
        trackOBJs = raceSelectHandler.tracks;
    }

    void PlaceTracks()
    {
        CreateCirclePositions();
        for (int i = 0; i < trackOBJs.Length; i++)
        {
            trackOBJs[i].transform.position = positions[i];
        }
    }

    void CreateCirclePositions()
    {
        positions = new Vector3[trackOBJs.Length];
        for (int i = 0; i < trackOBJs.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / trackOBJs.Length;
            Vector3 newPos = transform.rotation * new Vector3(Mathf.Cos(angle) * circleRadius, transform.position.y, Mathf.Sin(angle) * circleRadius);

            newPos += transform.position;
            positions[i] = newPos;
        }
    }
}
