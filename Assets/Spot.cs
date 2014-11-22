using UnityEngine;
using System.Collections;

public class Spot : GuardAction 
{
    public Transform target;
    public void SpotMagician(GameObject magician)
    {
        if (target == null)
        {
            target = magician.transform;
            Excute();
        }        
    }

    public override void Excute()
    {
        base.Excute();        
        guard.anim.CrossFade("atkReady");
        guard.FaceTarget(target);
        Debug.Log("spot");
        Invoke("BeginChase", 1.0f);
    }

    void BeginChase()
    {
        guard.chase.Excute();        
    }
}
