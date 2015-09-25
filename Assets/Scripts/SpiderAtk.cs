public class SpiderAtk : Attack 
{
    Actor target;
    public Actor GetTarget()
    {
        if((guard as Spider).machineActiveArea.actorsInTouch.Count != 0)
        {
            return (guard as Spider).machineActiveArea.actorsInTouch[0];
        }
        return null;
    }

    public override void AtkAnimation()
    {
        target = GetTarget();
        guard.FaceTarget(GetTarget().transform);
        guard.spriteSheet.Play("Atk");
    }

    public override void FireTheHit()
    {
        UnityEngine.GameObject netPrefab = UnityEngine.Resources.Load("Avatar/SpiderNet") as UnityEngine.GameObject;
        UnityEngine.GameObject net = UnityEngine.GameObject.Instantiate(netPrefab) as UnityEngine.GameObject;
        net.transform.position = transform.position;
        net.GetComponent<SpiderNet>().Fire(target);
        net.GetComponent<SpiderNet>().spider = guard as Spider;
    }

    public override bool checkTargetStillAlive()
    {
        if (GetTarget() == null || GetTarget().IsLifeOver())
        {
            Stop();
            guard.spriteSheet.Play("idle");
            return false;
        }
        return true;
    }

    public override bool checkTargetStillClose()
    {
        if (GetTarget() == null)
        {
            Stop();
            guard.spriteSheet.Play("idle");
            return false;
        }
        return true;
    }
}
