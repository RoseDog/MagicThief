public class GuardWeapon : UnityEngine.MonoBehaviour 
{
    public Guard owner;
    void Awake()
    {
        owner = GetComponentInParent<Actor>() as Guard;
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().hitted.ChangeLife(-owner.attackValue);
    }
}
