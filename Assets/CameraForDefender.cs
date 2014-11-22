using System.Collections;

public class CameraForDefender : MagicThiefCamera
{
    public Finger touchingFinger;
    public void Awake()
    {
        Globals.cameraForDefender = this;                
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	public override void Update () 
    {       
        base.Update();
	}
}
