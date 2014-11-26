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
        Globals.selectGuardUI.ShowBtns();
        guard.Unchoose();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        canvasTrans.localEulerAngles = new UnityEngine.Vector3(0.0f, Globals.cameraForDefender.transform.localEulerAngles.y, 0.0f);
	}
}
