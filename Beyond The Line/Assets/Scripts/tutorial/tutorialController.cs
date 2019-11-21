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
    [HideInInspector]
    public bool showBoost;

    [SerializeField]
    CanvasGroup R2Canvas;
    [SerializeField]
    CanvasGroup L3Canvas;
    [SerializeField]
    CanvasGroup stickCanvas;
    [SerializeField]
    CanvasGroup strafeCanvas;
    [SerializeField]
    CanvasGroup boostCanvas;

    CanvasGroup crntCanvas;

    private void Start()
    {
        Time.timeScale = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        if(crntCanvas != null && Input.GetButtonDown("Jump"))
        {
            crntCanvas.alpha = 0;
            Time.timeScale = 1;
        }
    }

    public void SetCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        crntCanvas = canvas;
        Time.timeScale = 0;
    }
}
