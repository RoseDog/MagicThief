public class MachineActiveArea : Actor
{
    public Machine machine;
       
    public override void TouchBegin(Actor other)
    {
        base.TouchBegin(other);
        machine.EnterActiveArea(other.gameObject);
    }
}
