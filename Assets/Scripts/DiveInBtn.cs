public class DiveInBtn : UnityEngine.MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => btnClicked());	
	}
    public void btnClicked()
    {
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Level_0");
    }	
}