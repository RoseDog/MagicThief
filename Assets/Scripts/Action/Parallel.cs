using UnityEngine;
using System.Collections.Generic;

public class Cocos2dParallel : Cocos2dAction
{
	
	// Action list
    private List<Cocos2dAction> actions = new List<Cocos2dAction>();
	
	// Constructor
    public Cocos2dParallel(params Cocos2dAction[] action_list)
	{
		// add actions to list
		for (int i = 0; i < action_list.Length; i++) actions.Add(action_list[i]);
	}
	
	// Init
	public override void Init () {

		initialized = true;
	}

	// Update
	public override void Update () {
		
		// Not completed
		if(!completed)
		{
			// removal list
            List<Cocos2dAction> to_remove = new List<Cocos2dAction>();
			
			// Run all actions in parallel
            foreach (Cocos2dAction action in actions)
			{
				
				// Initialize action
				if(!action.IsInitialized()) {
					// Assing parent
					action.parent = parent;
					// Initialize action
					action.Init();
				}

				// Update action
				action.Update();
				
				// Remove action when completed
				if(action.IsCompleted()) to_remove.Add(action);				
				
			}
			
			// Remove finished actions
            foreach (Cocos2dAction action in to_remove) actions.Remove(action);
			
			// No more actions
			if(actions.Count == 0) {
				
				Debug.Log("Parallel completed");
				EndAction();
			}
		
		}
		
	}

}
