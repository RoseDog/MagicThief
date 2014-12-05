using UnityEngine;
using System.Collections;

public class FadeTo : Cocos2dAction
{

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
	private Material _material;

	// Constructor
	public FadeTo(float alpha, float duration = 1f)
	{
		// define destination alpha
		_end = alpha;
		// define fade duration
		_duration = duration;
	}
	
	// Init
	public override void Init () {
		// get parent material
		_material = parent.renderer.material;
		// get start time
		_start_time = Time.time;
		// get starting alpha
		_start = _material.color.a;
		// get material color
		_color = _material.color;
		
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
			_material.color = _color;
			
			// Reached target position
			if(_material.color.a == _end) EndAction();

		}
		
	}

}
