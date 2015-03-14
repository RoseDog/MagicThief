public class BarkSoundWave : UnityEngine.MonoBehaviour
{
    public Guard owner;
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        Guard guard = other.GetComponent<Guard>();
        if (owner != guard && guard.heardAlert != null && guard.heardAlert.alertTeammate == null && guard.spot.target == null)
        {
            if (owner == null)
            {
                guard.heardAlert.HeardSound(transform.position);
            }
            else
            {
                guard.heardAlert.Heard(owner);
            }            
        }
    }

    void OnTriggerStay(UnityEngine.Collider other)
    {
        Guard guard = other.GetComponent<Guard>();
        if (owner != guard && guard.heardAlert != null && guard.heardAlert.alertTeammate == null && guard.spot.target == null)
        {
            if (owner == null)
            {
                guard.heardAlert.HeardSound(transform.position);
            }
            else
            {
                guard.heardAlert.Heard(owner);
            }
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {

    }
}
