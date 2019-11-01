using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFog : MonoBehaviour
{
    Renderer renderer;
    [SerializeField]
    [Range(0,1)]
    float value;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }
    // Update is called once per frame
    void Update()
    {
            renderer.material.color = Color.Lerp(Color.blue, Color.red, value);
        renderer.sharedMaterial.SetColor("Emission Map", Color.Lerp(Color.blue, Color.red, value));
    }
}
