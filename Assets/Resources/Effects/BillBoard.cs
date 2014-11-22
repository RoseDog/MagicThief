using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour 
{
	void Update() 
	{
        if (Globals.cameraFollowMagician != null)
        {
            transform.LookAt(transform.position + Globals.cameraFollowMagician.transform.rotation * Vector3.back,
            Globals.cameraFollowMagician.transform.rotation * Vector3.up);
        }        
	}
}
