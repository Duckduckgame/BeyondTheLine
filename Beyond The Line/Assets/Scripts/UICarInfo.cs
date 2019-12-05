using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICarInfo : MonoBehaviour
{
    RaceSelectHandler raceSelectHandler;

    HoverController hoverInfo;
    [SerializeField]
    HoverController[] ships;
    [SerializeField]
    float shiftSpeed = 1;

    [SerializeField]
    Image accelImage;
    [SerializeField]
    Image speedImage;
    [SerializeField]
    Image handlingImage;
    [SerializeField]
    Image boostImage;

    float accellSize;
    float speedSize;
    float handlingSize;
    float boostSize;

    float maxAccellSize = 0;
    float maxSpeedSize = 0;
    float maxHandlingSize = 0;
    float maxBoostSize = 0;
    // Start is called before the first frame update
    void Start()
    {
        raceSelectHandler = FindObjectOfType<RaceSelectHandler>();
        findMaximums();
    }

    private void Update()
    {
        UpdateCarInfo();
        LerpLengths();
    }

    void LerpLengths()
    {
        accelImage.rectTransform.offsetMax = new Vector2(Mathf.Lerp(accelImage.rectTransform.offsetMax.x, accellSize * 100, Time.deltaTime * shiftSpeed), accelImage.rectTransform.offsetMax.y);
        speedImage.rectTransform.offsetMax = new Vector2(Mathf.Lerp(speedImage.rectTransform.offsetMax.x, speedSize * 100, Time.deltaTime * shiftSpeed), speedImage.rectTransform.offsetMax.y);
        handlingImage.rectTransform.offsetMax = new Vector2(Mathf.Lerp(handlingImage.rectTransform.offsetMax.x, handlingSize * 100, Time.deltaTime * shiftSpeed), handlingImage.rectTransform.offsetMax.y);
        boostImage.rectTransform.offsetMax = new Vector2(Mathf.Lerp(boostImage.rectTransform.offsetMax.x, boostSize * 100, Time.deltaTime * shiftSpeed), boostImage.rectTransform.offsetMax.y);
    }

    public void UpdateCarInfo()
    {
            hoverInfo = raceSelectHandler.selectedTrackOBJ.GetComponent<UISelectionObject>().objectToLoad.GetComponent<HoverController>();
            accellSize = Mathf.InverseLerp(0, maxAccellSize, hoverInfo.accellerationIncrease);
            speedSize = Mathf.InverseLerp(0, maxSpeedSize, hoverInfo.maxAcceleration);
            handlingSize = Mathf.InverseLerp(0, maxHandlingSize, hoverInfo.turnSpeed);
            boostSize = Mathf.InverseLerp(0, maxBoostSize, hoverInfo.boostForce * hoverInfo.maxBoostAmount);
        
    }

    void findMaximums()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            if (ships[i].accellerationIncrease > maxAccellSize) maxAccellSize = ships[i].accellerationIncrease;
            if (ships[i].maxAcceleration > maxSpeedSize) maxSpeedSize = ships[i].maxAcceleration;
            if (ships[i].turnSpeed > maxHandlingSize) maxHandlingSize = ships[i].turnSpeed;
            if (ships[i].maxBoostAmount * ships[i].boostForce > maxBoostSize) maxBoostSize = ships[i].maxBoostAmount * ships[i].boostForce;
        }
        Debug.Log(maxAccellSize);
        Debug.Log(maxSpeedSize);
        Debug.Log(maxHandlingSize);
        Debug.Log(maxBoostSize);
    }
}
