public class LightCone : UnityEngine.MonoBehaviour 
{
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesInLight = new System.Collections.Generic.List<UnityEngine.GameObject>();
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().inLight = true;
        enemiesInLight.Add(other.gameObject);
    }

    void OnTriggerStay(UnityEngine.Collider other)
    {
        foreach (UnityEngine.GameObject enemy in enemiesInLight)
        {
            foreach (Guard guard in Globals.maze.guards)
            {
                if (guard.spot != null && guard.spot.target == null && guard.IsSeenEnemy(enemy))
                {
                    guard.SpotEnemy(enemy);
                    guard.eye.enemiesInEye.Add(enemy);                    
                }
            }
        }
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().inLight = false;
        enemiesInLight.Remove(other.gameObject);        
    }    
}
