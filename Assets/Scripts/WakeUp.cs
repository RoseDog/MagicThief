public class WakeUp : GuardAction 
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        System.Collections.Generic.List<UnityEngine.Sprite> animation = new System.Collections.Generic.List<UnityEngine.Sprite>();
        SpriteAnim anim = new SpriteAnim();
        anim.spriteList = new System.Collections.Generic.List<UnityEngine.Sprite>();
        anim.events = new System.Collections.Generic.Dictionary<int, UnityEngine.Events.UnityAction>();
        anim.speed = 0.3f;
        anim.clampForever = false;
        SpriteAnim BeenHypnosised = guard.spriteSheet._animationList["BeenHypnosised"];
        for (int i = BeenHypnosised.spriteList.Count-1; i >=0 ; --i)
        {
            UnityEngine.Sprite sprite = BeenHypnosised.spriteList[i];
            anim.spriteList.Add(sprite);
        }
        Globals.Assert(anim.spriteList.Count != 0, "empty animation");
        guard.spriteSheet._animationList.Add("wakeUp", anim);

        guard.spriteSheet.AddAnimationEvent("wakeUp", -1, ()=>wakeUpEnd());
    }

    public override void Excute()
    {
        base.Excute();
        actor.spriteSheet.Play("wakeUp");
    }

    public void wakeUpEnd()
    {
        Stop();
        guard.EnableEyes(true);
        guard.wandering.Excute();
    }
}
