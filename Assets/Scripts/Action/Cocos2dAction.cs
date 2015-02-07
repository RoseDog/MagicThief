using UnityEngine;
using System.Collections;

public class Cocos2dAction
{
	// parent object
	public MonoBehaviour parent = null;
	
	// completed flag
    public bool completed = false;
	
	// initialized flag
    public bool initialized = false;
	
	// Return true if action is completed
	public bool IsCompleted() { return completed; }
	
	// Return true if action is initialized
	public bool IsInitialized() { return initialized; }
	
	// Initialization
	public virtual void Init() {}
	
	// Update
	public virtual void Update() {}
	
	// Complete
	protected void EndAction()
	{
		completed = true;
	}
}

