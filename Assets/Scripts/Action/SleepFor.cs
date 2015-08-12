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
        _start_frame = Globals.LevelController.frameCount;
	
		initialized = true;
	}

	public override void Update () {
		
		// Not completed
		if(!completed)
		{
            System.String content_test = parent.gameObject.name + " Sleep for " +
                Globals.LevelController.frameCount.ToString() + " " + _start_frame.ToString();
            Globals.record("testReplay", content_test);

			// Reached target duration
            if ((Globals.LevelController.frameCount - _start_frame) >= _frameDuration) EndAction();
		}
		
	}

}
