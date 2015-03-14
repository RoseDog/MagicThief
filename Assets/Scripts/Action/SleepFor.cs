public class SleepFor : Cocos2dAction
{

	// duration
    public int _frameDuration;
	// start time
    public int _start_frame;

	// Constructor
	public SleepFor(int duration = 100)
	{
		// define duration
		_frameDuration = duration;
	}
	
	// Init
	public override void Init () {
		// get start time
		_start_frame = UnityEngine.Time.frameCount;
	
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Reached target duration
            if ((UnityEngine.Time.frameCount - _start_frame) >= _frameDuration) EndAction();
		}
		
	}

}
