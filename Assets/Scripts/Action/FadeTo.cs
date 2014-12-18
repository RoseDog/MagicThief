using UnityEngine;
using System.Collections;

public class FadeTo : Cocos2dAction
{
    Renderer _renderer;
	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start alpha
	private float _start;
	// end alpha
	private float _end;
	// parent color
	private Color _color;
	// parent material
	

	// Constructor
    public FadeTo(Renderer renderer, float start, float end, float duration = 1f)
	{
        _renderer = renderer;
        _start = start;
		// define destination alpha
        _end = end;
		// define fade duration
		_duration = duration;
	}
	
	// Init
	public override void Init () 
    {		       		
		_start_time = Time.time;	
        
		// get material color
        _color = _renderer.material.color;
        _color.a = _start;
        _renderer.material.color = _color;
		
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Change tmp color
			_color.a = Mathf.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			
			// Update material color
            _renderer.material.color = _color;
			
			// Reached target position
            if (_renderer.material.color.a == _end) EndAction();

		}		
	}
}


public class FadeUI : Cocos2dAction
{
    AlphaFadeUI _ui;
	// duration
	private float _duration = 1f;
	// start time
	private float _start_time = 0f;
	// start alpha
	private float _start;
	// end alpha
	private float _end;
	// parent color
	private Color _color;
	// parent material
	

	// Constructor
    public FadeUI(AlphaFadeUI tip, float start, float end, float duration = 1f)
	{
        _ui = tip;
        _start = start;
		// define destination alpha
        _end = end;
		// define fade duration
		_duration = duration;
	}
	
	// Init
	public override void Init () 
    {		       		
		_start_time = Time.time;
        _ui.UpdateAlpha(_start);
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Change tmp color
			float a = Mathf.Lerp(_start, _end, (Time.time - _start_time) / _duration);
			
			// Update material color
            _ui.UpdateAlpha(a);
			
			// Reached target position
            if (a == _end) EndAction();

		}		
	}
}
