public class RealiseGemLost : GuardAction 
{
    System.Collections.Generic.List<Pathfinding.Path> tempPathes = new System.Collections.Generic.List<Pathfinding.Path>();
    Cocos2dAction call;
    public override void Excute()
    {
        base.Excute();
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Suspicious);
        guard.spriteSheet.Play("idle");

        // 找到最近的宝石
        for (ushort idx = 0; idx < (Globals.LevelController as TutorialLevelController).unstolenChests.Count; ++idx)
        {
            Chest chest = (Globals.LevelController as TutorialLevelController).unstolenChests[idx];
            Pathfinding.Path p = Pathfinding.ABPath.Construct(transform.position, chest.transform.position, null);
            p.callback += OnPathToGemComplete;
            p.chest = chest;
            AstarPath.StartPath(p);
        }        
    }

    
    public void OnPathToGemComplete(Pathfinding.Path p)
    {
        tempPathes.Add(p);
        if (tempPathes.Count == (Globals.LevelController as TutorialLevelController).unstolenChests.Count)
        {
            double shortest = UnityEngine.Mathf.Infinity;
            Chest nearestChest = null;
            for (int idx = 0; idx < tempPathes.Count; ++idx)
            {
                Pathfinding.Path path = tempPathes[idx];
                double length = path.GetTotalLength();
                if (length < shortest)
                {
                    shortest = length;
                    nearestChest = path.chest;
                }
            }
            UnityEngine.Debug.Log("go to guard nearest gem");
            guard.guardedChest = nearestChest;
            Pathfinding.Node birthNode = Globals.maze.pathFinder.GetSingleNode(nearestChest.transform.position, true);
            guard.birthNode = birthNode;
            guard.patrol.InitPatrolRoute();

            call = guard.SleepThenCallFunction(70, () => BackToNewBirthNodePos());
            tempPathes.Clear();
        }        
    }

    public override void Stop()
    {
        guard.RemoveAction(ref call);
        base.Stop();
    }

    void BackToNewBirthNodePos()
    {
        guard.backing.Excute();
    }
}
