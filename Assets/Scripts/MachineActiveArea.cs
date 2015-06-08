public class MachineActiveArea : UnityEngine.MonoBehaviour
{
    public Machine machine;
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesInArea = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public virtual void OnTriggerEnter(UnityEngine.Collider other)
    {        
        enemiesInArea.Add(other.gameObject);
        machine.EnterActiveArea(other.gameObject);
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().inLight = false;
        enemiesInArea.Remove(other.gameObject);
    }
}
