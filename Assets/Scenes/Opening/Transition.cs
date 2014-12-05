using System.Collections;
public class Transition : UnityEngine.MonoBehaviour
{
	public UnityEngine.Material cookShadersCover;
	public float fadeSpeed = 0.5f;
	public UnityEngine.Color fadeColor = new UnityEngine.Color(0.0f, 0.0f, 0.0f, 1.0f);
	private UnityEngine.GameObject cookShadersObject;

	float fadingTime = 1.5f;
	float current;

    UnityEngine.MonoBehaviour callBackObj;
    System.String method;

	void Awake()
	{
        Globals.transition = this;
        cookShadersCover = Instantiate(cookShadersCover) as UnityEngine.Material;
		cookShadersCover.SetColor("_TintColor", fadeColor);        
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
        cookShadersObject.transform.localPosition = new UnityEngine.Vector3(cookShadersObject.transform.localPosition.x, cookShadersObject.transform.localPosition.y, cookShadersObject.transform.localPosition.z + 1.55f);
        cookShadersObject.transform.localRotation = UnityEngine.Quaternion.identity;
        cookShadersObject.transform.localEulerAngles = new UnityEngine.Vector3(cookShadersObject.transform.localPosition.x, cookShadersObject.transform.localPosition.y, cookShadersObject.transform.localPosition.z + 180.0f);
        cookShadersObject.transform.localScale = new UnityEngine.Vector3(3.6f * 1.5f, 1.5f, 1.5f);
	}

	public void Visible(bool visibility)
	{
		cookShadersObject.renderer.enabled = visibility;
	}

	IEnumerator FadeOut()
	{
		Location();
        fadeColor.a = 0.0f;
		while (fadeColor.a < 1.0f)
		{
			current = current + UnityEngine.Time.deltaTime;
			fadeColor.a = current / fadingTime;
			cookShadersCover.SetColor("_TintColor", fadeColor);
			yield return null;
		}

        if (callBackObj != null)
        {
            callBackObj.Invoke(method, 0.0f);
            callBackObj = null;
        }        
	}

	IEnumerator FadeIn()
	{
		Location();
        fadeColor.a = 1.0f;
		while (fadeColor.a > 0.0f)
		{
            current = current - UnityEngine.Time.deltaTime;
			fadeColor.a = current / fadingTime;
			cookShadersCover.SetColor("_TintColor", fadeColor);
			yield return null;
		}
        Visible(false);
        if (callBackObj != null)
        {
            callBackObj.Invoke(method, 0.0f);
            callBackObj = null;
        }
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

    public void BlackOut(UnityEngine.MonoBehaviour callback = null, System.String methodName = "")
	{
        if (cookShadersObject == null)
        {
            CreateCameraCoverPlane();
        }
        Visible(true);
        current = 0.0f;
        StopCoroutine("FadeIn");
        StartCoroutine(FadeOut());
        callBackObj = callback;
        method = methodName;
	}

    public void BlackIn(UnityEngine.MonoBehaviour callback = null, System.String methodName = "")
	{
        if (cookShadersObject == null)
        {
            CreateCameraCoverPlane();
        }
        Visible(true);
		current = fadingTime;
		StopCoroutine("FadeOut");
		StartCoroutine(FadeIn());
        callBackObj = callback;
        method = methodName;
	}

    public bool IsBackOutFinished()
    {
        return fadeColor.a >= 1.0f;
    }
}