public class FunctionCall : Cocos2dAction 
{
    UnityEngine.MonoBehaviour callback;
    System.String method;
    // Constructor
    public FunctionCall(UnityEngine.MonoBehaviour cb, System.String methodName)
	{
        callback = cb;
        method = methodName;
	}
	
	// Init
	public override void Init () 
    {
        initialized = true;
        callback.Invoke(method, 0.0f);
		EndAction();        				
	}	
}
