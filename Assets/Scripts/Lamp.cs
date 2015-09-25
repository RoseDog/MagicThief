public class Lamp : Machine 
{
    Cocos2dAction moveAction;
    
    public override void Awake()
    {
        base.Awake();
        spriteSheet.AddAnim("idle", 4,1.5f);
        fixTimer_offset = new UnityEngine.Vector3(-6.9f,-84.3f,0);
    }

    public override void Start()
    {
        base.Start();
        if ((Globals.LevelController as StealingLevelController) != null)
        {
            int duration = UnityEngine.Random.Range(30, 50);
            UnityEngine.Vector3 pos_cache = transform.localPosition;
            moveAction = new RepeatForever(
                new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0, 0.08f, 0), duration / 2),
                new MoveTo(transform, pos_cache, duration / 2));
            //AddAction(moveAction);
        }        
    }

    public override void Broken(int fixDuration)
    {
        base.Broken(fixDuration);                
        //moveAction.paused = true;        
    }

    public override void FixingComplete()
    {
        base.FixingComplete();
        //moveAction.paused = false;        
    }
}
