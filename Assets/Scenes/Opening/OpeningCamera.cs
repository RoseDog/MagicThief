public class OpeningCamera : UnityEngine.MonoBehaviour 
{
    UnityEngine.Animator anim;
	// Use this for initialization
	void Start () 
    {
        Globals.canvasForScreen.SetActive(false);
        Globals.input.enabled = false;
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
        Globals.canvasForScreen.SetActive(true);        
        Globals.input.enabled = true;
    }
}
