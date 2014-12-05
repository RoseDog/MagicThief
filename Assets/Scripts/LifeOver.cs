public class LifeOver : Action
{
    public override void Excute()
    {
        base.Excute();
        (actor as Magician).isMoving = false;
        (actor as Magician).UnRegistEvent();
        actor.anim.Play("A_Die_1");
        UnityEngine.Debug.Log("magician life over");
    }
}
