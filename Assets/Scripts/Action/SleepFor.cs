using UnityEngine;
using System.Collections;

public class SleepFor : Cocos2dAction
{

	// duration
	private float _duration;
	// start time
	private float _start_time;

	// Constructor
	public SleepFor(float duration = 1f)
	{
		// define duration
		_duration = duration;
	}
	
	// Init
	public override void Init () {
		// get start time
		_start_time = Time.time;
	
		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// Reached target duration
			if((Time.time - _start_time) >= _duration) EndAction();
		}
		
	}

}
