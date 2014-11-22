using System.Collections;

public class GuardAction : Action 
{
    protected Guard guard;
    public override void Awake()
    {
        guard = GetComponent<Guard>();
        base.Awake();
    }
}
