using UnityEngine;
using System.Collections;

public class ScaleTo : Cocos2dAction
{
	// duration
	private int _duration;
	// start time
	private int _start_frame;
	// start scale
	private Vector3 _start;
	// end scale
	private Vector3 _end;
	// parent transformer
	private Transform _transform;
	
	// Constructor
	public ScaleTo(Transform target, Vector3 size, int duration = 100)
	{
        _transform = target;
		// define destination scale
		_end = size;
		// define scale duration
		_duration = duration;
	}
	
	// Init
	public override void Init () {
        
		// get start time
		_start_frame = Time.frameCount;
		// get starting scale
		_start = _transform.localScale;
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Update scale
            _transform.localScale = Vector3.Lerp(_start, _end, (Time.frameCount - _start_frame) / (float)_duration);

            if (Time.frameCount - _start_frame >= _duration)
            {
                EndAction();
                _transform.localScale = _end;
            }
		}
		
	}

}
