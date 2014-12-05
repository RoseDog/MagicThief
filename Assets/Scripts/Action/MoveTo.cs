using UnityEngine;
using System.Collections;

public class MoveTo : Cocos2dAction
{

	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start position
	private Vector3 _start;
	// end position
	private Vector3 _end;
	// parent transformer
	private Transform _transform;

	// Constructor
	public MoveTo(Vector3 position, float duration = 1f)
	{
		// define destination point
		_end = position;
		// define movement duration
		_duration = duration;
	}
	
	// Init
	public override void Init () {
		// get transformer instance
		_transform = parent.transform;
		// get start time
		_start_time = Time.time;
		// get starting position
        _start = _transform.localPosition;
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			/// Update position
			_transform.localPosition = Vector3.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			
			// Reached target position
            if (_transform.localPosition == _end) EndAction();
		}
		
	}

}
