public class ShotLight : Action 
{    
    UnityEngine.Transform spineBone;
    public UnityEngine.GameObject targetLight;
    RotateUpperBody rotateBody;
    System.String animationName = "A_Skill_1";
    public override void Awake()
    {
        base.Awake();
        rotateBody = GetComponent<RotateUpperBody>();

        if (!Globals.AvatarAnimationEventNameCache.Contains(actor.name + "-" + animationName))
        {
            actor.anim[animationName].layer = 3;

            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "ShotOver";
            evt.time = actor.anim[animationName].length;
            actor.anim[animationName].clip.AddEvent(evt);

            spineBone = Globals.getChildGameObject<UnityEngine.Transform>(gameObject, "Bip01 Spine");
            UnityEngine.Debug.Log(spineBone);
            actor.anim[animationName].AddMixingTransform(spineBone);

            UnityEngine.AnimationEvent shotEvt = new UnityEngine.AnimationEvent();
            shotEvt.functionName = "LightBroken";
            shotEvt.time = actor.anim[animationName].length * 0.7f;
            actor.anim[animationName].clip.AddEvent(shotEvt);

            Globals.AvatarAnimationEventNameCache.Add(actor.name + "-" + animationName);
        }        
    }

    public void Shot(UnityEngine.GameObject light)
    {
        targetLight = light;
        rotateBody.enabled = true;        
        rotateBody.target = new UnityEngine.Vector3(light.transform.position.x, 4.0f, light.transform.position.z);
        Excute();
    }

    public override void Excute()
    {
        base.Excute();        
        actor.anim.Play(animationName);
        actor.anim[animationName].weight = 1.0f;
        
    }

    public void LightBroken()
    {
        targetLight.GetComponentInParent<Lamp>().BulbBroken();
    }
    public void ShotOver()
    {
        UnityEngine.Debug.Log("ShotOver");
        Stop();
        rotateBody.enabled = false;
        targetLight = null;
    }
}