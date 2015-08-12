public class Cocos2dProgress : Cocos2dAction
{
    // duration
	private int _frameDuration;
	// start time
	private int _start_frame;
	// start position
	    
	// parent transformer
    private UnityEngine.UI.Image _image;

	// Constructor
    public Cocos2dProgress(UnityEngine.UI.Image image, int frameDuration = 100)
	{
        _image = image;
        _image.type = UnityEngine.UI.Image.Type.Filled;
        _image.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        _image.fillOrigin = (int)UnityEngine.UI.Image.OriginHorizontal.Left;
		
		// define movement duration
		_frameDuration = frameDuration;
	}
	
	// Init
	public override void Init () 
    {		
		// get start time
        _start_frame = Globals.LevelController.frameCount;
        _image.fillAmount = 1.0f;
		initialized = true;
	}

    public override void Update()
    {
        // Not completed
        if (!completed)
        {
            UnityEngine.Vector3 tempResult = UnityEngine.Vector3.zero;

            float progress = UnityEngine.Mathf.Lerp(1.0f, 0.0f, (Globals.LevelController.frameCount - _start_frame) / (float)_frameDuration);
            _image.fillAmount = progress;

            // Reached target position
            if (progress <= 0.0f) EndAction();
        }
    }		
 }
