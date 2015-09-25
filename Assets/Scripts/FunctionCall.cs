public class FunctionCall : Cocos2dAction 
{
    public UnityEngine.MonoBehaviour callback;
    public System.String method;
    UnityEngine.Events.UnityAction action;
    // Constructor
    public FunctionCall(UnityEngine.Events.UnityAction a)
	{
        action = a;
	}
	
	// Init
	public override void Init () 
    {
        initialized = true;
        if (action != null)
        {
            action.Invoke();
        }        
		EndAction();        				
	}	
}
