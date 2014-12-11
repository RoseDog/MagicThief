using UnityEngine;
using System.Collections;

public class ScaleTo : Cocos2dAction
{
	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start scale
	private Vector3 _start;
	// end scale
	private Vector3 _end;
	// parent transformer
	private Transform _transform;
	
	// Constructor
	public ScaleTo(Transform target, Vector3 size, float duration = 1f)
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
		_start_time = Time.time;
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
			_transform.localScale = Vector3.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			
			// Reached target size
			if(_transform.localScale == _end) EndAction();

		}
		
	}

}
