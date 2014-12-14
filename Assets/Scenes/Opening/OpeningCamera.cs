public class OpeningCamera : UnityEngine.MonoBehaviour 
{
    UnityEngine.Animator anim;
    public UnityEngine.GameObject canvasForLogin;
    void Awake()
    {
        UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
        UnityEngine.GameObject mgrs = UnityEngine.GameObject.Instantiate(mgrs_prefab) as UnityEngine.GameObject;
    }
	// Use this for initialization
	void Start () 
    {
        canvasForLogin = UnityEngine.GameObject.Find("CanvasForLogin");
        canvasForLogin.SetActive(false);
        Globals.EnableAllInput(false);
        anim = GetComponent<UnityEngine.Animator>();
        anim.Play("CamMovingUp");        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BlackIn()
    {
        Globals.transition.BlackIn();
    }

    public void BlackOut()
    {
        Globals.transition.BlackOut();
    }

    public void MoveEnd()
    {
        Invoke("ShowUI", 2.5f);        
    }

    void ShowUI()
    {
        canvasForLogin.SetActive(true);
        Globals.EnableAllInput(true);
    }

    public void GameBegin()
    {
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
}
