public class LightCone : MachineActiveArea
{
    public override void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.GetComponent<Actor>().inLight = true;
        base.OnTriggerEnter(other);                
    }

    void OnTriggerStay(UnityEngine.Collider other)
    {
        foreach (UnityEngine.GameObject enemy in enemiesInArea)
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
}
