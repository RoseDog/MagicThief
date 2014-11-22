using System.Collections;

public class Action : UnityEngine.MonoBehaviour 
{
    protected Actor actor;
    public virtual void Awake()
    {
        actor = GetComponent<Actor>();
    }

    public virtual void Excute()
    {
        if (actor.currentAction != null)
        {
            actor.currentAction.Stop();
        }
        actor.currentAction = this;
    }

    public virtual void Stop()
    {

    }
}
