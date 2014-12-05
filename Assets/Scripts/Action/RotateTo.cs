using UnityEngine;
using System.Collections;

public class RotateTo : Cocos2dAction
{
	
	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start rotation
	private Quaternion _start;
	// end rotation
	private Quaternion _end;
	// parent transformer
	private Transform _transform;

	// Constructor
	public RotateTo(Vector3 rotation, float duration = 1f)
	{
		// define destination rotation
		_end = Quaternion.Euler(rotation.x,rotation.y,rotation.z);
		// define rotation duration
		_duration = duration;
	}
	
	// Init
	public override void Init () {
		// get transformer instance
		_transform = parent.transform;
		// get start time
		_start_time = Time.time;
		// get starting rotation
		_start = _transform.rotation;
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Update rotation
			_transform.rotation = Quaternion.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			
			// Reached target position
			if(_transform.rotation == _end) EndAction();

		}
		
	}

}
