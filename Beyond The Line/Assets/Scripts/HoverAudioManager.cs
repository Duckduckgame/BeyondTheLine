using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip hoverSFX;

    float hoverTargetVolume = 1;
    float hoverTargetPitch = 1;

    [SerializeField]
    AudioClip bump;
    [SerializeField]
    AudioClip speedWhoosh;
    [SerializeField]
    AudioClip strafeWhoosh;
    [SerializeField]
    AudioClip boost;

    HoverController hoverController;

    [SerializeField]
    AudioSource[] sources;

    AudioSource hoverSource;
    AudioSource windSource;
    AudioSource strafeSource;
    AudioSource boostSource;
    AudioSource bumpSource;
    AudioSource boostPadSource;

    float boostlerp = 0;

    // Start is called before the first frame update
    void Start()
    {
        hoverController = GetComponent<HoverController>();
        hoverSource = sources[0];
        boostSource = sources[1];
        strafeSource = sources[2];
        strafeSource.clip = strafeWhoosh;
        strafeSource.Play();
        bumpSource = sources[3];
        boostPadSource = sources[4];
    }

    // Update is called once per frame
    void Update()
    {
        float strafeTarget = 0;
        if (Input.GetButton("Strafe"))
            strafeTarget = 1f;

        VolumeCalculations();
        PitchCalculations();
        BoostCalculations();

        //hoverTargetPitch = Mathf.Clamp(hoverTargetPitch, 0, 1.3f);
        hoverSource.volume = Mathf.Lerp(hoverSource.volume, hoverTargetVolume, Time.deltaTime);
        hoverSource.pitch = Mathf.Lerp(hoverSource.pitch, hoverTargetPitch, Time.deltaTime);
        strafeSource.volume = Mathf.Lerp(strafeSource.volume, strafeTarget + Random.Range(-1f * strafeTarget,1f*strafeTarget), Time.deltaTime*2);

    }

    void VolumeCalculations()
    {
        float lerpedVolume = Mathf.InverseLerp(0, hoverController.maxNoiseSpeed, hoverController.crntAcceleration) + 0.3f;

        if (lerpedVolume > 0.8f)
        {
            hoverTargetVolume = lerpedVolume + Random.Range(-0.5f, 0.5f);
        }
        else
        {
            hoverTargetVolume = lerpedVolume;
        }
    }

    void PitchCalculations()
    {
        float lerpedPitch = Mathf.InverseLerp(0, hoverController.maxNoiseSpeed, hoverController.crntAcceleration) + 1f;

        if (lerpedPitch > 0.8f)
        {
            hoverTargetPitch = lerpedPitch + Random.Range(-0.3f, 0.3f) + Mathf.Abs(Input.GetAxis("Horizontal")/2);
        }
        else
        {
            hoverTargetPitch = lerpedPitch + Mathf.Abs(Input.GetAxis("Horizontal"));
        }
    }

    void BoostCalculations()
    {
        if (hoverController.IsBoosting)
        {
            boostlerp = Mathf.Lerp(boostlerp, 1, Time.deltaTime*5);
        }
        else {
            boostlerp = Mathf.Lerp(boostlerp, 0, Time.deltaTime);
        }


        
        boostSource.volume = boostlerp;
        if (hoverController.IsBoosting)
        {
            boostSource.pitch = boostlerp * 6;
        }else boostSource.pitch = 0;
    }

    public void Bump()
    {
        bumpSource.Play();
    }

    public void BoostPadPlay()
    {
        boostPadSource.Play();
    }
}
