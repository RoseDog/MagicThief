public class BillBoard : UnityEngine.MonoBehaviour 
{
	void Update() 
	{
        transform.LookAt(transform.position + UnityEngine.Camera.main.transform.rotation * UnityEngine.Vector3.back,
            UnityEngine.Camera.main.transform.rotation * UnityEngine.Vector3.up);
	}
}
