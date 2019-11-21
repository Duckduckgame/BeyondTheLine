using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    [Header("Gold Medal")]
    public float goldTime;
    public GameObject goldGO;

    [Header("Siver Medal")]
    public float silverTime;
    public GameObject silverGO;

    [Header("Bronze Medal")]
    public float bronzeTime;
    public GameObject bronzeGO;

    public GameObject NAGO;


    public GameObject SetMedalByTime(float time)
    {
        GameObject chosenOBJ = NAGO;

        if (time < bronzeTime) chosenOBJ = bronzeGO;
        if (time < silverTime) chosenOBJ = silverGO;
        if (time < goldTime) chosenOBJ = goldGO;


        return chosenOBJ;

    }

}
