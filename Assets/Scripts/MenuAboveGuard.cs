using System.Collections;

public class MenuAboveGuard : UnityEngine.MonoBehaviour
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Globals.cameraForDefender.transform.position);
	}
}
