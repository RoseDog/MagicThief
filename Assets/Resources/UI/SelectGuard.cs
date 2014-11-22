using System;
using System.Collections;

public class SelectGuard : UnityEngine.MonoBehaviour 
{
    UnityEngine.GameObject guardSelectedImage;
    [UnityEngine.HideInInspector]
    public UnityEngine.UI.Button[] btns;
    [UnityEngine.HideInInspector]
    public String selectedGuardPrefabName;
    [UnityEngine.HideInInspector]
    public Guard nextGuard;
    [UnityEngine.HideInInspector]
    public Cell birthCell;
    void Awake()
    {
        Globals.selectGuardUI = this;
        btns = GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (UnityEngine.UI.Button btn in btns)
        {
            UnityEngine.UI.Button temp = btn;
            btn.onClick.AddListener(() => btnClicked(temp));
        }
        guardSelectedImage = getChildGameObject(gameObject, "guardSelectedImage");
        gameObject.SetActive(false);        
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
        CancelNextGuard();        
        
        selectedGuardPrefabName = btn.name;
        ShowNextGuard();
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().parent = btn.GetComponent<UnityEngine.RectTransform>();
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector2.zero;

        // 选择南北方向最远10个格子，然后来回走        
        //guard.GetComponent<Patrol>().Excute();
    }

    public void ShowNextGuard()
    {
        UnityEngine.Debug.Log("ShowNextGuard");
        guardSelectedImage.gameObject.SetActive(true);
        // 默认选择的守卫出现在地图上
        UnityEngine.GameObject guard_prefab = UnityEngine.Resources.Load("Avatar/" + Globals.selectGuardUI.selectedGuardPrefabName) as UnityEngine.GameObject;
        UnityEngine.GameObject guardObject = UnityEngine.GameObject.Instantiate(guard_prefab) as UnityEngine.GameObject;

        Guard guard = guardObject.GetComponent<Guard>();
        guardObject.transform.position = birthCell.GetFloorPos();
        guard.birthCell = birthCell;
        guard.Choosen();
        guard.patrol.InitPatrolRoute();
        nextGuard = guard;
    }

    public void CancelNextGuard()
    {
        if (nextGuard != null)
        {
            Globals.DestroyGuard(nextGuard);            
            guardSelectedImage.gameObject.SetActive(false);
            nextGuard = null;            
        }
    }

    public void ShowBtns()
    {
        StartCoroutine(_scaleCanvasOut());
    }

    public void HideBtns()
    {
        StopCoroutine(_scaleCanvasOut());
        transform.localScale = UnityEngine.Vector3.zero;
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