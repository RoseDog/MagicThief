public class CameraFollow : MagicThiefCamera
{
    UnityEngine.Transform target;        
    public bool pauseFollowing = false;

    public override void Awake()
    {
        base.Awake();
        Globals.cameraFollowMagician = this;
    }

    public void Reset()
    {
        enabled = true;
        lookAt = lookAtCache;
        disOffset = disOffsetCache;
    }

    public void beginFollow(UnityEngine.Transform tar)
	{
        enabled = true;
        pauseFollowing = false;
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
                lookAt = RestrictPosition(lookAt);
            }            
        }
        base.Update();
	}
}
