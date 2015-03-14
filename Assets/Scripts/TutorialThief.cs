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

        LifeAmount = 100;
        LifeCurrent = LifeAmount;
        spriteSheet.CreateAnimationByName("idle");
        spriteSheet.CreateAnimationByName("moving");
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
        AddAction(new ScaleTo(ThiefTip.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 10));
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
        ShowPathToPoint(targetChest.transform.position);               
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
        HidePath();
        moving.canMove = true;
    }

    public override void OutStealing()
    {
        ClearRouteCubes();
        moving.canMove = false;        
        base.OutStealing();
    }
}
