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

    private bool _isUI = false;

	// Constructor
    public MoveTo(Transform target, Vector3 to, float duration = 1f, bool isUI = false)
	{
        _transform = target;
		// define destination point
		_end = to;
		// define movement duration
		_duration = duration;

        _isUI = isUI;
	}
	
	// Init
	public override void Init () 
    {		
		// get start time
		_start_time = Time.time;
		// get starting position
        if (!_isUI)
        {
            _start = _transform.localPosition;
        }
        else
        {
            _start = (_transform as UnityEngine.RectTransform).anchoredPosition;
        }
        
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
            UnityEngine.Vector3 tempResult = UnityEngine.Vector3.zero;

            tempResult = Vector3.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			/// Update position
            if (!_isUI)
            {
                _transform.localPosition = tempResult;
            }
            else
            {
                (_transform as UnityEngine.RectTransform).anchoredPosition = tempResult;
            }			
			
			// Reached target position
            if (tempResult == _end) EndAction();
		}
		
	}

}


public class EaseOut : Cocos2dAction
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

    private bool _isUI = false;

	// Constructor
    public EaseOut(Transform target, Vector3 to, float duration = 1f, bool isUI = false)
	{
        _transform = target;
		// define destination point
		_end = to;
		// define movement duration
		_duration = duration;

        _isUI = isUI;
	}
	
	// Init
	public override void Init () 
    {		
		// get start time
		_start_time = Time.time;
		// get starting position
        if (!_isUI)
        {
            _start = _transform.localPosition;
        }
        else
        {
            _start = (_transform as UnityEngine.RectTransform).anchoredPosition;
        }
        
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
            UnityEngine.Vector3 tempResult = UnityEngine.Vector3.zero;
            float time = Time.time - _start_time;
            
            // exponential 
            //float temp = time == 1 ? 1 : (-UnityEngine.Mathf.Pow(2, -10 * time / 1) + 1);

            // quint
            time -= 1;
            float temp = time * time * time * time * time + 1;
            tempResult = Vector3.Lerp(_start, _end, temp);
			/// Update position
            if (!_isUI)
            {
                _transform.localPosition = tempResult;
            }
            else
            {
                (_transform as UnityEngine.RectTransform).anchoredPosition = tempResult;
            }
            if (Time.time - _start_time >= _duration)
            {
                EndAction();
            }			
		}
		
	}

}
