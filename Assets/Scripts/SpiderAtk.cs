public class SpiderAtk : Attack 
{
    public Actor GetTarget()
    {
        if((guard as Spider).machineActiveArea.enemiesInArea.Count != 0)
        {
            return (guard as Spider).machineActiveArea.enemiesInArea[0].GetComponent<Actor>();
        }
        return null;
    }

    public override void AtkAnimation()
    {
        guard.FaceTarget(GetTarget().transform);
        guard.spriteSheet.Play("Atk");
    }

    public override void FireTheHit()
    {
        if ((guard as Spider).machineActiveArea.enemiesInArea.Count != 0)
        {            
            UnityEngine.GameObject netPrefab = UnityEngine.Resources.Load("Avatar/SpiderNet") as UnityEngine.GameObject;
            UnityEngine.GameObject net = UnityEngine.GameObject.Instantiate(netPrefab) as UnityEngine.GameObject;
            net.transform.position = transform.position;
            net.GetComponent<SpiderNet>().Fire(GetTarget());
            net.GetComponent<SpiderNet>().spider = guard as Spider;            
        }
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
