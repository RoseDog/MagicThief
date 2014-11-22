using System.Collections;

public class BeginPatrolBtn : UnityEngine.MonoBehaviour 
{
    public Guard guard;
    public Patrol patrol;
    UnityEngine.Transform canvasTrans;
    void Awake()
    {
        UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => btnClicked());

        canvasTrans = GetComponentInParent<UnityEngine.Canvas>().transform;
    }

    public void btnClicked()
    {
        // 开始巡逻了，允许收回
        if (guard == Globals.selectGuardUI.nextGuard)
        {
            guard.takeGuardBackBtn.gameObject.SetActive(true);
        }
        // 这是一个已经在巡逻的guard，再次显示出选择按钮
        else
        {
            Globals.selectGuardUI.ShowBtns();
        }
        
        guard.Unchoose();
        Globals.selectGuardUI.ShowNextGuard();    
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        canvasTrans.localEulerAngles = new UnityEngine.Vector3(0.0f, Globals.cameraForDefender.transform.localEulerAngles.y, 0.0f);
	}
}
