public class CanvasForLogin : UnityEngine.MonoBehaviour 
{
	// Use this for initialization
	void Awake () 
    {
        Globals.canvasForLogin = this;
        UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
        UnityEngine.GameObject mgrs = UnityEngine.GameObject.Instantiate(mgrs_prefab) as UnityEngine.GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
