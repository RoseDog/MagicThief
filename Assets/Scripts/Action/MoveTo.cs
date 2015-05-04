using UnityEngine;
using System.Collections;

public class MoveTo : Cocos2dAction
{

	// duration
	private int _duration;
	// start time
	private int _start_frame;
	// start position
	private Vector3 _start;
	// end position
	private Vector3 _end;
	// parent transformer
	private Transform _transform;

    private bool _isUI = false;

	// Constructor
    public MoveTo(Transform target, Vector3 to, int duration = 100, bool isUI = false)
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
		_start_frame = Time.frameCount;
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

            tempResult = Vector3.Lerp(_start, _end, (Time.frameCount - _start_frame) / (float)_duration);
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


public class MoveToWithSpeed : Cocos2dAction
{
    // duration
    private float _speed;
    // start time
    private int _start_frame;
    // start position
    private Vector3 _start;
    // end position
    private Vector3 _end;

    private Vector3 _dir;
    // parent transformer
    private Transform _transform;

    private bool _isUI = false;

    // Constructor
    public MoveToWithSpeed(Transform target, Vector3 to, float speed = 0.1f)
    {
        _transform = target;
        // define destination point
        _end = to;
        // define movement duration
        _speed = speed;
    }

    // Init
    public override void Init()
    {
        // get start time
        _start_frame = Time.frameCount;
        // get starting position
        _start = _transform.localPosition;

        _dir = (_end - _start).normalized;

        initialized = true;
    }

    // Update
    public override void Update()
    {

        // Not completed
        if (!completed)
        {
            UnityEngine.Vector3 tempResult = UnityEngine.Vector3.zero;

            _transform.localPosition = _start + (Time.frameCount - _start_frame) * _dir * _speed;                        
            
            // Reached target position
            if (Globals.Vector3AlmostEqual(_transform.localPosition, _end, 0.1f)) EndAction();
        }

    }

}


public class EaseOut : Cocos2dAction
{

	// duration
	private int _duration;
	// start time
	private int _start_frame;
	// start position
	private Vector3 _start;
	// end position
	private Vector3 _end;
	// parent transformer
	private Transform _transform;

    bool _isUI;
	// Constructor
    public EaseOut(Transform target, Vector3 to, int duration = 100, bool isUI = true)
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
		_start_frame = Time.frameCount;
		// get starting position
        if (_isUI)
        {
            _start = (_transform as UnityEngine.RectTransform).anchoredPosition;
        }
        else
        {
            _start = _transform.localPosition;
        }
        
        
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
            UnityEngine.Vector3 tempResult = UnityEngine.Vector3.zero;
            float time = (Time.frameCount - _start_frame) / (float)_duration;
            
            // exponential 
            //float temp = time == 1 ? 1 : (-UnityEngine.Mathf.Pow(2, -10 * time / 1) + 1);

            // quint
            time -= 1;
            float temp = time * time * time * time * time + 1;
            tempResult = Vector3.Lerp(_start, _end, temp);
			/// Update position
            if (_isUI)
            {
                (_transform as UnityEngine.RectTransform).anchoredPosition = tempResult;
            }
            else
            {
                _transform.localPosition = tempResult;
            }
            

            if (Time.frameCount - _start_frame >= _duration)
            {
                EndAction();
                (_transform as UnityEngine.RectTransform).anchoredPosition = _end;
            }			
		}
		
	}

}


public class JumpTo : Cocos2dAction
{
	// duration
	private int _duration;
	// start time
	private int _start_frame;
	// start position
	private Vector3 _start;
	// end position
	private Vector3 _end;
    float _height;
	// parent transformer
	private Transform _transform;

	// Constructor
    public JumpTo(Transform target, Vector3 to, float height = 1.5f, int duration = 100)
	{
        _transform = target;
		// define destination point
		_end = to;
        _height = height;
		// define movement duration
		_duration = duration;
	}
	
	// Init
	public override void Init () 
    {		
		// get start time
		_start_frame = Time.frameCount;
        _start = _transform.localPosition;       
		
		initialized = true;
	}

	// Update
    public float jumpHeight;
	public override void Update () {
		
		// Not completed
		if(!completed)
		{            
            UnityEngine.Vector3 delta = _end - _start;

            float frac = (Time.frameCount - _start_frame) / (float)_duration;
            jumpHeight = _height * 4 * frac * (1 - frac);
            jumpHeight += delta.y * frac;
            float x = delta.x * frac;

            _transform.localPosition = _start + new UnityEngine.Vector3(x, jumpHeight);
			
			// Reached target position
            if (_transform.localPosition == _end) EndAction();
		}
		
	}

}