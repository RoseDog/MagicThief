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
            Actor tar = GetTarget();
            UnityEngine.Vector3 magicianDir = tar.transform.position - guard.transform.position;

            UnityEngine.Vector3 faceDir = UnityEngine.Vector3.left;
            if (transform.localEulerAngles.y > 179)
            {
                faceDir = UnityEngine.Vector3.right;
            }
            faceDir.z = 0;
            double angle = UnityEngine.Vector3.Angle(magicianDir, faceDir);
            UnityEngine.GameObject netPrefab = UnityEngine.Resources.Load("Avatar/SpiderNet") as UnityEngine.GameObject;
            UnityEngine.GameObject net = UnityEngine.GameObject.Instantiate(netPrefab) as UnityEngine.GameObject;
            net.transform.position = transform.position;
            net.GetComponent<SpiderNet>().Fire(tar);
            net.GetComponent<SpiderNet>().spider = guard as Spider;

            UnityEngine.GameObject soundPrefab = UnityEngine.Resources.Load("Misc/GunSound") as UnityEngine.GameObject;
            GuardAlertSound sound = (UnityEngine.GameObject.Instantiate(soundPrefab) as UnityEngine.GameObject).GetComponent<GuardAlertSound>();
            sound.transform.position = transform.position;
            sound.SetRadius(15);
            sound.StartAlert();
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
