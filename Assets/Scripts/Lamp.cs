public class Lamp : Guard 
{
    TrickTimer fixTimer;
    Cocos2dAction moveAction;
    UnityEngine.GameObject LightCone;
    public override void Awake()
    {
        base.Awake();
        walkable = false;
        fixTimer = GetComponentInChildren<TrickTimer>();
        fixTimer.gameObject.SetActive(false);
        LightCone = Globals.getChildGameObject(gameObject,"LightCone");
        spriteSheet.Play("idle");        
    }

    public override void Start()
    {
        base.Start();
        if ((Globals.LevelController as TutorialLevelController) != null)
        {
            int duration = UnityEngine.Random.Range(30, 50);
            UnityEngine.Vector3 pos_cache = transform.localPosition;
            moveAction = new RepeatForever(
                new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0, 0.08f, 0), duration / 2),
                new MoveTo(transform, pos_cache, duration / 2));
            AddAction(moveAction);
        }        
    }

    int fixingDuration = 500;
    public void BulbBroken()
    {
        fixTimer.BeginCountDown(this.gameObject, fixingDuration, UnityEngine.Vector3.zero);
        spriteSheet.enabled = false;
        moveAction.paused = true;
        LightCone.SetActive(false);
        fixTimer.gameObject.SetActive(true);
        SleepThenCallFunction(fixingDuration, () => FixingComplete());
    }

    public void FixingComplete()
    {
        fixTimer.gameObject.SetActive(false);
        spriteSheet.enabled = true;
        moveAction.paused = false;
        LightCone.SetActive(true);
    }
}
