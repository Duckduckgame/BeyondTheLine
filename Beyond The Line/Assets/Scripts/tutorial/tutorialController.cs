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
            StartCoroutine(StopCanvas(crntCanvas));
        }
    }

    public void SetCanvas(CanvasGroup canvas)
    {
        crntCanvas = canvas;
        StartCoroutine(StartCanvas(canvas));
        
    }

    IEnumerator StartCanvas( CanvasGroup canvas)
    {
        for (float i = 0; i < 1.1f; i+= 0.3f)
        {
            canvas.alpha = i;
            Time.timeScale = 1 - i;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Time.timeScale = 0;
        canvas.alpha = 1;
        yield return null;
    }

    IEnumerator StopCanvas(CanvasGroup canvas)
    {
        for (float i = 1; i > -0.1; i -= 0.3f)
        {
            canvas.alpha = i;
            Time.timeScale = 1 - i;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Time.timeScale = 1;
        canvas.alpha = 0;
        yield return null;
    }
}
