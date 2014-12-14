public class BillBoard : UnityEngine.MonoBehaviour 
{
	void Update() 
	{
        if (UnityEngine.Camera.main != null)
        {
            transform.LookAt(transform.position + UnityEngine.Camera.main.transform.rotation * UnityEngine.Vector3.back,
            UnityEngine.Camera.main.transform.rotation * UnityEngine.Vector3.up);
        }        
	}
}
