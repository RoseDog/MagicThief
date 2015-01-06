public class Incant : Action 
{
    bool bReadyToReleaseDove = false;
    UnityEngine.GameObject dovePrefab;
    public Dove dove;
    public override void Awake()
    {
        base.Awake();
        dovePrefab = UnityEngine.Resources.Load("Avatar/Dove") as UnityEngine.GameObject;
        if (!Globals.AvatarAnimationEventNameCache.Contains(actor.name + "-A_Attack_2"))
        {
            UnityEngine.AnimationEvent evt = new UnityEngine.AnimationEvent();
            evt.functionName = "IncantOver";
            evt.time = actor.anim["A_Attack_2"].length;
            actor.anim["A_Attack_2"].clip.AddEvent(evt);

            UnityEngine.AnimationEvent doveEvt = new UnityEngine.AnimationEvent();
            doveEvt.functionName = "ReleaseDove";
            doveEvt.time = actor.anim["A_Attack_2"].length*0.33f;
            actor.anim["A_Attack_2"].clip.AddEvent(doveEvt);

            Globals.AvatarAnimationEventNameCache.Add(actor.name + "-A_Attack_2");
        }        
    }

    public override void Excute()
    {        
        base.Excute();
        actor.anim["A_Attack_2"].time = 0.0f;
        actor.anim["A_Attack_2"].speed = 0;
        actor.anim.Play("A_Attack_2");

        dove = (UnityEngine.GameObject.Instantiate(dovePrefab) as UnityEngine.GameObject).GetComponent<Dove>();
        dove.transform.position = new UnityEngine.Vector3(actor.transform.position.x, 
            dove.transform.position.y,
            actor.transform.position.z);
    }

    public override void Stop()
    {
        base.Stop();
    }

    public void FingerDown(Finger f)
    {        
        Excute();
    }

    public void FingerMoving(Finger f)
    {        
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 9;
        UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(f.nowPosition);
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {
            // 面朝拖动方向
            UnityEngine.Vector3 horDir = actor.transform.position - hitInfo.point;            
            horDir.y = 0;
            
            float magnitude = horDir.magnitude;
            // 拉开一定距离的时候才起作用
            if (magnitude > 1.0f)
            {
                float rate = UnityEngine.Mathf.Clamp01(magnitude / 3.0f);
                bReadyToReleaseDove = true;
                actor.transform.forward = horDir;
                actor.anim["A_Attack_2"].time = 0.2f * rate;
                float flyDistance = 15.0f * rate;
                dove.ShowFlyPath(horDir, flyDistance);
            }            
            else
            {
                dove.HideFlyPath();
                bReadyToReleaseDove = false;
            }
        }
        else
        {
            Globals.Assert(false);
        }                       
    }

    public void FingerUp(Finger finger)
    {        
        if (bReadyToReleaseDove)
        {
            actor.anim["A_Attack_2"].speed = 1;            
        }
        else
        {
            Destroy(dove.gameObject);
            IncantOver();
        }
        bReadyToReleaseDove = false;        
    }

    public void ReleaseDove()
    {        
        dove.ReleaseToFly();
        dove.HideFlyPath();
    }

    public void IncantOver()
    {
        Stop();
    }
}
