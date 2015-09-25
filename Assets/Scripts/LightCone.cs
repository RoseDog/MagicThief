public class LightCone : MachineActiveArea
{
    public override void TouchBegin(Actor other)
    {
        other.GetComponent<Actor>().inLight = true;
        base.TouchBegin(other);
    }
    
    public override void TouchOut(Actor other)
    {
        base.TouchOut(other);
        other.GetComponent<Actor>().inLight = false;
    }

    public override void TouchStay(Actor enemy)
    {
        base.TouchStay(enemy);
        foreach (Guard guard in Globals.maze.guards)
        {
            if (guard.spot != null && guard.spot.target == null && guard.IsSeenEnemy(enemy.gameObject))
            {
                guard.SpotEnemy(enemy.gameObject);
                guard.eye.enemiesInEye.Add(enemy.gameObject);
            }
        }
    }
}
