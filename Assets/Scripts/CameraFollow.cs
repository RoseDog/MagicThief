public class CameraFollow : MagicThiefCamera
{
    UnityEngine.Transform target;        
    public bool pauseFollowing = false;

    public void Awake()
    {
        Globals.cameraFollowMagician = this;
    }

    public void beginFollow(UnityEngine.Transform tar)
	{
        target = tar;
	}

	// Update is called once per frame
	public override void Update()
	{        
        if (target != null)
        {
            if (!pauseFollowing)
            {
                lookAt = target.position;
            }            
        }
        base.Update();
	}
}
