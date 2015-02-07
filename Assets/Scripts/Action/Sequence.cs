using UnityEngine;
using System.Collections.Generic;

public class Sequence : Cocos2dAction
{
	
	// Action list
    private List<Cocos2dAction> actions = new List<Cocos2dAction>();
	
	// Constructor
    public Sequence(params Cocos2dAction[] action_list)
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
			
			// Run actions
			if(actions.Count>0)
			{
				// Get current action instance
                Cocos2dAction action = actions[0];
				
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
				if(action.IsCompleted()) actions.Remove(action);

			} else {
				
				Debug.Log("Sequence completed");
				
				// No more actions
				EndAction();
			}
		}
		
	}

}

public class RepeatForever : Cocos2dAction
{

    // Action list
    private List<Cocos2dAction> actions = new List<Cocos2dAction>();

    int currentActionIdx;

    // Constructor
    public RepeatForever(params Cocos2dAction[] action_list)
    {
        currentActionIdx = 0;
        // add actions to list
        for (int i = 0; i < action_list.Length; i++) actions.Add(action_list[i]);
    }

    // Init
    public override void Init()
    {

        initialized = true;
    }

    // Update
    public override void Update()
    {
        // Get current action instance
        Cocos2dAction action = actions[currentActionIdx];

        // Initialize action
        if (!action.IsInitialized())
        {
            // Assing parent
            action.parent = parent;
            // Initialize action
            action.Init();
        }

        // Update action
        action.Update();

        // Remove action when completed
        if (action.IsCompleted())
        {
            action.completed = false;
            action.initialized = false;
            ++currentActionIdx;
            currentActionIdx = currentActionIdx % actions.Count;
        }     
    }
}
