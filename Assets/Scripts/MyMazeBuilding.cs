public class MyMazeBuilding : Building
{
    UnityEngine.GameObject BackHome;
    public override void Awake()
    {
        BackHome = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "BackHome").gameObject;
        base.Awake();
    }

    public override void Choosen()
    {
        BackHome.gameObject.SetActive(true);
        BackHome.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new ScaleTo(BackHome.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 0.3f));
        base.Choosen();
    }    

    public void IntoHomeMaze()
    {
        Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");        
    }
}