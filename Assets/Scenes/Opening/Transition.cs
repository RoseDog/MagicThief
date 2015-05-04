using System.Collections;
public class Transition : Actor
{
	public UnityEngine.Material cookShadersCover;
	private UnityEngine.GameObject cookShadersObject;
    Cocos2dAction fadeInAction;
    Cocos2dAction fadeOutAction;
    float duration = 0.8f;
	public override void Awake()
	{
        base.Awake();
        Globals.transition = this;
        cookShadersCover = Instantiate(cookShadersCover) as UnityEngine.Material;
	}

    UnityEngine.GameObject CreateCameraCoverPlane()
	{
        cookShadersObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
		cookShadersObject.renderer.material = cookShadersCover;        
		return cookShadersObject;
	}

	void Location()
	{
		cookShadersObject.transform.parent = UnityEngine.Camera.main.transform;
        cookShadersObject.transform.localPosition = UnityEngine.Vector3.zero;        
        cookShadersObject.transform.localRotation = UnityEngine.Quaternion.identity;        
        cookShadersObject.transform.localScale = new UnityEngine.Vector3(40, 40, 1);
	}

	public override void Visible(bool visibility)
	{
		cookShadersObject.renderer.enabled = visibility;
	}

	void DestroyCameraCoverPlane()
	{
        UnityEngine.Debug.Log("destroy plane");
		if (cookShadersObject)
		{
			DestroyImmediate(cookShadersObject);
		}

		cookShadersObject = null;
	}

    public void BlackOut(UnityEngine.Events.UnityAction action = null)
	{
        if (cookShadersObject == null)
        {
            CreateCameraCoverPlane();
        }
        Visible(true);

        if (fadeInAction != null)
        {
            RemoveAction(ref fadeInAction);
        }
        Location();
        fadeOutAction = new Sequence(new FadeTo(cookShadersObject.renderer, 0, 1, duration), new FunctionCall(action));
        AddAction(fadeOutAction);
	}

    
    public void BlackIn(UnityEngine.Events.UnityAction action = null)
	{
        if (cookShadersObject == null)
        {
            CreateCameraCoverPlane();
        }
        Visible(true);
        if (fadeOutAction != null)
        {
            RemoveAction(ref fadeOutAction);
        }
        Location();
        fadeInAction = new Sequence(new FadeTo(cookShadersObject.renderer, 1, 0, duration), new FunctionCall(action));
        AddAction(fadeInAction);
	}    
}