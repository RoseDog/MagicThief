using System;
using System.Collections;

public class SelectGuard : UnityEngine.MonoBehaviour 
{    
    [UnityEngine.HideInInspector]
    public GuardBtn[] btns;
    
    void Awake()
    {
        btns = GetComponentsInChildren<GuardBtn>();
        UnityEngine.GameObject selectedImage = getChildGameObject(gameObject, "guardSelectedImage");
        foreach (GuardBtn btn in btns)
        {
            UnityEngine.UI.Button temp = btn.GetComponent<UnityEngine.UI.Button>();
            temp.onClick.AddListener(() => btnClicked(temp));
            btn.guardSelectedImage = selectedImage;
            btn.guardSelectedImage.gameObject.SetActive(false);
        }
    }

    public UnityEngine.GameObject getChildGameObject(UnityEngine.GameObject fromGameObject, String withName)
    {
        //Author: Isaac Dart, June-13.
        UnityEngine.RectTransform[] ts = fromGameObject.GetComponentsInChildren<UnityEngine.RectTransform>();
        foreach (UnityEngine.RectTransform child in ts)
        {
            if (child.gameObject.name == withName)
                return child.gameObject;
        }

        return null;
    }
	// Use this for initialization
	void Start () 
    {
        
	}

    public void btnClicked(UnityEngine.UI.Button btn)
    {
        UnityEngine.Debug.Log(btn.name);
//         CancelNextGuard();        
//         
//         selectedGuardPrefabName = btn.name;
//         ShowNextGuard();
        

        // 选择南北方向最远10个格子，然后来回走        
        //guard.GetComponent<Patrol>().Excute();
    }

    public void ShowBtns()
    {
        gameObject.SetActive(true);
        StartCoroutine(_scaleCanvasOut());
    }

    public void HideBtns()
    {
        StopCoroutine(_scaleCanvasOut());
        transform.localScale = UnityEngine.Vector3.zero;
        gameObject.SetActive(false);
        foreach (GuardBtn btn in btns)
        {
            btn.dragging = false;
            btn.inside = false;
            btn.guardSelectedImage.gameObject.SetActive(false);
        }
    }

    float currentScaleTime = 1.0f;
    float scaleCanvasForCommandTime = 0.2f;
    IEnumerator _scaleCanvasOut()
    {
        float scale = 0.0f;
        currentScaleTime = 0.0f;
        while (scale < 1.0f)
        {
            currentScaleTime = currentScaleTime + UnityEngine.Time.deltaTime;
            scale = currentScaleTime / scaleCanvasForCommandTime;
            transform.localScale = new UnityEngine.Vector3(scale, scale, scale);

            yield return null;
        }        
        yield return null;
    }
}