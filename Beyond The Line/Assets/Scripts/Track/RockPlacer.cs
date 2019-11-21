using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPlacer : MonoBehaviour
{
    Transform[] rockPositions;
    [SerializeField]
    GameObject[] rockGO;
    GameObject rockParent;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GO = new GameObject();
        rockParent = Instantiate(GO, Vector3.zero, Quaternion.identity);
        rockParent.name = "rockParent";
        rockPositions = gameObject.GetComponentsInChildren<Transform>();
        


        for (int i = 0; i < rockPositions.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(rockPositions[i].position, Vector3.down, out hit))
            {

                if (hit.collider.gameObject.GetComponent<TerrainCollider>() != null)
                {
                    Vector3 rotation = new Vector3(-90, Random.Range(0,180), 0);
                    Vector3 depth = Vector3.up * 5;
                    GameObject rockObject = Instantiate(rockGO[Random.Range(0, rockGO.Length)], hit.point - depth, Quaternion.Euler(rotation) * Quaternion.Euler(hit.normal), rockParent.transform);
                    int rockScale = Random.Range(40, 80);
                    rockObject.transform.localScale = new Vector3(rockScale, rockScale, rockScale);
                   
                }
            }
        }
    }
}
