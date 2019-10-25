using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialController : MonoBehaviour
{
    [HideInInspector]
    public bool showR2;
    [HideInInspector]
    public bool showL3;
    [HideInInspector]
    public bool showSticking;
    [HideInInspector]
    public bool showStrafe;

    [SerializeField]
    CanvasGroup R2Canvas;
    [SerializeField]
    CanvasGroup L3Canvas;
    [SerializeField]
    CanvasGroup stickCanvas;
    [SerializeField]
    CanvasGroup strafeCanvas;

    CanvasGroup crntCanvas;

    // Update is called once per frame
    void Update()
    {
        if(crntCanvas != null && Input.GetButtonDown("Jump"))
        {
            crntCanvas.alpha = 0;
            Time.timeScale = 1;
        }
    }

    public void setR2Canvas()
    {
        R2Canvas.alpha = 1;
        crntCanvas = R2Canvas;
        Time.timeScale = 0.0001f;
    }
    public void setL3Canvas()
    {
        L3Canvas.alpha = 1;
        crntCanvas = L3Canvas;
        Time.timeScale = 0.0001f;
    }
    public void setStickCanvas()
    {
        stickCanvas.alpha = 1;
        crntCanvas = stickCanvas;
        Time.timeScale = 0.0001f;
    }
    public void setStrafeCanvas()
    {
        strafeCanvas.alpha = 1;
        crntCanvas = strafeCanvas;
        Time.timeScale = 0.0001f;
    }
}
