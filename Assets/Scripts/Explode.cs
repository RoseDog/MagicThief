public class Explode : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        UnityEngine.GameObject flashPrefab = UnityEngine.Resources.Load("Avatar/Flash") as UnityEngine.GameObject;
        UnityEngine.GameObject flash = UnityEngine.GameObject.Instantiate(flashPrefab) as UnityEngine.GameObject;
        flash.transform.position = guard.transform.position;
        Actor actor = guard.spot.target.GetComponent<Actor>();
        actor.ChangeLife(-guard.data.attackValue);
        actor.hitted.Excute();        

        Globals.DestroyGuard(guard);
    }
}
