using UnityEngine;
using System.Collections;

public class RotateTo : Cocos2dAction
{	
	// duration
	private int _duration;
	// start time
	private int _start_frame;
	// start rotation
    private Vector3 _start;
	// end rotation
    private Vector3 _end;
	// parent transformer
	private Transform _transform;

    private bool _loop = false;

	// Constructor
    public RotateTo(Vector3 from, Vector3 to, int duration = 30, bool loop = false)
	{
        _start = from;

        _end = to;

		_duration = duration;
        
        _loop = loop;
	}
	
	// Init
	public override void Init () {
		// get transformer instance
		_transform = parent.transform;
		// get start time
        _start_frame = Globals.LevelController.frameCount;
        _transform.rotation = Quaternion.Euler(_start);

		initialized = true;        
	}

	public override void Update () {
		
		// Not completed
		if(!completed)
		{
            _transform.rotation = Quaternion.Euler(Vector3.Lerp(_start, _end, (Globals.LevelController.frameCount - _start_frame) / (float)_duration));
			// Reached target position
            if (Globals.LevelController.frameCount - _start_frame >= _duration)
            {
                _transform.rotation = Quaternion.Euler(_end);
                if(!_loop)
                {
                    EndAction();
                }
                else
                {
                    _start_frame = Globals.LevelController.frameCount;                    
                    _transform.rotation = Quaternion.Euler(_start);
                }
            }
		}		
	}
}


public class RotateEye : Cocos2dAction
{
    // duration
    private int _duration;
    // start time
    private int _start_frame;
    // start rotation
    private Vector3 _start_dir;
    // end rotation
    private Vector3 _end;
    // parent transformer
    private FOV2DEyes fovEye;

    // Constructor
    public RotateEye(FOV2DEyes eye, Vector3 to, int duration = 30)
    {
        fovEye = eye;        

        _end = to;

        _duration = duration;
    }

    // Init
    public override void Init()
    {
        _start_dir = fovEye.dirCache;
        // get start time
        _start_frame = Globals.LevelController.frameCount;
        
        initialized = true;
    }

    public override void Update()
    {
        // Not completed
        if (!completed)
        {
            Vector3 euler = Vector3.Lerp(Vector3.zero, _end, (float)(Globals.LevelController.frameCount - _start_frame) / (float)_duration);

            fovEye.dirCache =
                Quaternion.Euler(euler) * _start_dir;
            // Reached target position
            if (Globals.LevelController.frameCount - _start_frame >= _duration)
            {
                EndAction();
            }
        }
    }
}
