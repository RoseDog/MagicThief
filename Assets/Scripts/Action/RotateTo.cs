using UnityEngine;
using System.Collections;

public class RotateTo : Cocos2dAction
{	
	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start rotation
    private Vector3 _start;
	// end rotation
    private Vector3 _end;
	// parent transformer
	private Transform _transform;

    private bool _loop = false;

	// Constructor
    public RotateTo(Vector3 from, Vector3 to, float duration = 1f, bool loop = false)
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
		_start_time = Time.time;
        _transform.rotation = Quaternion.Euler(_start);

		initialized = true;        
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Update rotation
            _transform.rotation = Quaternion.Euler(Vector3.Lerp(_start, _end, (Time.time - _start_time) / _duration));
            Vector3 euler = _transform.rotation.eulerAngles;
			// Reached target position
            if (Time.time - _start_time >= _duration)
            {
                _transform.rotation = Quaternion.Euler(_end);
                if(!_loop)
                {
                    EndAction();
                }
                else
                {
                    _start_time = Time.time;                    
                    _transform.rotation = Quaternion.Euler(_start);
                }
            }

		}
		
	}

}
