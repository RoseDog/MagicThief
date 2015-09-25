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
               
        spriteSheet.AddAnim("idle", 4);
        spriteSheet.AddAnim("moving", 6, 2.0f);
        spriteSheet.AddAnim("falling", 1, 0.2f);
        spriteSheet.AddAnim("landing", 1, 0.2f);
        spriteSheet.AddAnim("hitted", 1, 0.2f);
        spriteSheet.AddAnim("lifeOver", 6, 0.8f);
        spriteRenderer.color = UnityEngine.Color.black;
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
    public void AimAtTargetChest(Chest chest)
    {
        targetChest = chest;
        //ShowPathToPoint(targetChest.transform.position);
        GoTo(targetChest.GetWorldCenterPos());
        moving.canMove = false;
    }
    

    public override void InStealing()
    {
        base.InStealing();        
        moving.canMove = true;
    }

    public override void OutStealing()
    {
        HidePath();
        moving.canMove = false;        
        base.OutStealing();
    }

    public void Vanish()
    {
        Destroy(gameObject);
    }
}
