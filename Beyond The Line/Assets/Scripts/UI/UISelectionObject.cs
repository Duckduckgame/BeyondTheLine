using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UISelectionObject : MonoBehaviour
{
    public enum SelectionType {Track, Vehicle };

    public SelectionType selectionType;
    Button button;
    GameObject trackPreview;

    
    public string stringToLoad;

    public GameObject objectToLoad;

    public Vector3 targetPos;

    float slerpSpeed = 4;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * slerpSpeed);
    }


}
