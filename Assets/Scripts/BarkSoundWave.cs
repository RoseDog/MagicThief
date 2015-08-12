public class BarkSoundWave : Actor
{
    public Guard owner;
    public override void TouchBegin(Actor other)    
    {
        base.TouchBegin(other);
        Guard guard = other as Guard;
        if (owner != guard && guard.heardAlert != null && /*guard.heardAlert.alertTeammate == null &&*/ guard.spot.target == null)
        {
            guard.heardAlert.HeardSound(transform.position);
//             if (owner == null)
//             {
//                 guard.heardAlert.HeardSound(transform.position);
//             }
//             else
//             {
//                 guard.heardAlert.Heard(owner);
//             }            
        }
    }

    public override void TouchStay(Actor other)
    {
        base.TouchStay(other);        
        Guard guard = other as Guard;
        if (owner != guard && guard.heardAlert != null && /*guard.heardAlert.alertTeammate == null &&*/ guard.spot.target == null)
        {
            guard.heardAlert.HeardSound(transform.position);

//             if (owner == null)
//             {
//                 guard.heardAlert.HeardSound(transform.position);
//             }
//             else
//             {
//                 guard.heardAlert.Heard(owner);
//             }
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {

    }
}
