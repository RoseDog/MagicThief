public class MagicianTrickAction : Action 
{
    public TrickData data;
    protected Magician mage;
    protected System.String animationName;
    protected System.String castTrickFuncName;    
    UnityEngine.Transform spineBone;
    RotateUpperBody rotateBody;
    public override void Awake()
    {
        mage = GetComponent<Magician>();
        base.Awake();

        rotateBody = GetComponent<RotateUpperBody>();

        if (actor.anim[animationName] != null && !Globals.AvatarAnimationEventNameCache.Contains(actor.name + "-" + animationName))
        {
            actor.anim[animationName].layer = 3;

            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "TrickActionEnd";
            evt.time = actor.anim[animationName].length;
            actor.anim[animationName].clip.AddEvent(evt);

            spineBone = Globals.getChildGameObject<UnityEngine.Transform>(gameObject, "Bip01 Spine");
            actor.anim[animationName].AddMixingTransform(spineBone);

            UnityEngine.AnimationEvent shotEvt = new UnityEngine.AnimationEvent();
            shotEvt.functionName = castTrickFuncName;
            shotEvt.time = actor.anim[animationName].length * 0.7f;
            actor.anim[animationName].clip.AddEvent(shotEvt);

            Globals.AvatarAnimationEventNameCache.Add(actor.name + "-" + animationName);
        }
    }

    protected void AimTarget(UnityEngine.Vector3 targetPos)
    {
        rotateBody.enabled = true;
        rotateBody.target = targetPos;
    }

    public virtual void CastTrick()
    {

    }

    public virtual void TrickActionEnd()
    {
        base.Stop();
        rotateBody.enabled = false;
    }

    public override void Excute()
    {
        base.Excute();
        if (actor.anim[animationName] != null)
        {
            actor.anim.Play(animationName);
            actor.anim[animationName].weight = 1.0f;
        }        
    }
}
