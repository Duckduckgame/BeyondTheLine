using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class transitionPlane : MonoBehaviour
{
    [SerializeField]
    float timeToTransition = 2f;

    Material myMat;
    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<RawImage>().material;
        myMat.SetFloat("Vector1_8CA26D50", -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TransitionOut(string levelName)
    {
        for (float i = 0; i < timeToTransition; i+= 0.01f)
        {
            float lerp = Mathf.InverseLerp(0, timeToTransition, i);
            myMat.SetFloat("Vector1_8CA26D50", lerp);
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadSceneAsync(levelName);

        yield return null;
    }

    public IEnumerator TransitionOut(int levelIndex)
    {
        for (float i = 0; i < timeToTransition; i += 0.01f)
        {
            float lerp = Mathf.InverseLerp(0, timeToTransition, i);
            myMat.SetFloat("Vector1_8CA26D50", lerp);
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadSceneAsync(levelIndex);

        yield return null;
    }

    public IEnumerator TransitionIn()
    {

        myMat = GetComponent<RawImage>().material;
        myMat.SetFloat("Vector1_8CA26D50", 1.5f);
        for (float i = 0; i < timeToTransition; i += 0.01f)
        {
            float lerp = Mathf.InverseLerp(0, timeToTransition, (i+ 0.5f)*-1);
            myMat.SetFloat("Vector1_8CA26D50", lerp*-1);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
}
