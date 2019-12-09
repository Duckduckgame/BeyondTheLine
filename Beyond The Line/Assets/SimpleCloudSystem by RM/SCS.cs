using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCS : MonoBehaviour {

	public Transform Player ;
	public  float CloudsSpeed;




	
	// Update is called once per frame
	void Update ()
    {

        if (Player != null)
        {
            Vector3 pos = Player.transform.position;
            gameObject.transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        }
		transform.Rotate(0,Time.deltaTime*CloudsSpeed ,0); 
	}




}
