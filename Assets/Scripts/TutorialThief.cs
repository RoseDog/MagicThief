public class TutorialThief : Actor 
{
    UnityEngine.GameObject ThiefTip;        
    public UnityEngine.RectTransform TipToShowMazeBtn;
    public UnityEngine.UI.Button BtnToShowMazeBtn;
    public UnityEngine.RectTransform TipRetry;
    public UnityEngine.UI.Button RetryBtn;
    public UnityEngine.RectTransform TipAnotherThief;
    public UnityEngine.UI.Button MorGuardBtn;
    public override void Awake()
    {
        ThiefTip = Globals.getChildGameObject(gameObject, "ThiefTip");
        TipToShowMazeBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "TipToShowMazeBtn");
        BtnToShowMazeBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "BtnToShowMazeBtn");                
        TipRetry = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "TipRetry");        
        RetryBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "RetryBtn");
        TipAnotherThief = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "TipAnotherThief");
        MorGuardBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(gameObject, "MorGuardBtn");
        HideTip();
        base.Awake();

        animation["repel"].speed = 0.2f;
    }

    public void ShowTipToShowMazeBtn()
    {
        TipToShowMazeBtn.gameObject.SetActive(true);        
        TipRetry.gameObject.SetActive(false);
        TipAnotherThief.gameObject.SetActive(false);
    }

    public void ShowTipRetry()
    {
        TipRetry.gameObject.SetActive(true);        
        TipToShowMazeBtn.gameObject.SetActive(false);
        TipAnotherThief.gameObject.SetActive(false);
    }

    public void ShowTipAnotherThief()
    {
        TipAnotherThief.gameObject.SetActive(true);
        TipRetry.gameObject.SetActive(false);
        TipToShowMazeBtn.gameObject.SetActive(false);        
    }

    void _ShowTip()
    {
        ThiefTip.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new ScaleTo(ThiefTip.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 0.3f));
    }

    public void HideTip()
    {
        TipRetry.gameObject.SetActive(false);
        TipToShowMazeBtn.gameObject.SetActive(false);
        TipAnotherThief.gameObject.SetActive(false);
    }

    public Chest targetChest;
    System.Collections.Generic.List<UnityEngine.GameObject> cubes = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public void AimAtTargetChest(Chest chest)
    {
        ClearRouteCubes();
        targetChest = chest;
        
        moving.GetSeeker().StartPath(moving.GetFeetPosition(), targetChest.transform.position);

        moving.canMove = false;
        System.Collections.Generic.List<UnityEngine.Vector3> path = moving.GetSeeker().GetCurrentPath().vectorPath;
        float nodeSize = Globals.maze.pathFinder.graph.nodeSize;
        UnityEngine.Debug.Log(path.Count);
        foreach (UnityEngine.Vector3 pos in path)
        {
            // 生成表示行走区域的方块
            UnityEngine.GameObject cube = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
            cube.transform.localScale = new UnityEngine.Vector3(nodeSize, 0.5f, nodeSize);
            cube.transform.position = pos;

            UnityEngine.MeshRenderer meshRenderer = cube.GetComponentInChildren<UnityEngine.MeshRenderer>();
            meshRenderer.material.SetColor("_Color", UnityEngine.Color.red);

            cubes.Add(cube);
        }
    }

    void ClearRouteCubes()
    {
        foreach (UnityEngine.GameObject cube in cubes)
        {
            DestroyObject(cube);
        }
        cubes.Clear();
    }

    public override void InStealing()
    {
        base.InStealing();
        moving.canMove = true;
    }

    public override void OutStealing()
    {
        ClearRouteCubes();
        moving.canMove = false;
        (Globals.LevelController as MagicianHomeLevel).GuardTakeThiefDown();
        base.OutStealing();
    }
}
