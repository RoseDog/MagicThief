using System.Collections;

public class GuardWeapon : UnityEngine.MonoBehaviour 
{
    public Actor owner;
    void Awake()
    {
        owner = GetComponentInParent<Actor>();
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().hitted.Excute();
    }
}
