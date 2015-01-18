public class RealiseGemLost : GuardAction 
{
    System.Collections.Generic.List<Pathfinding.Path> tempPathes = new System.Collections.Generic.List<Pathfinding.Path>();    

    public override void Excute()
    {
        base.Excute();
        foreach (FOV2DEyes eye in guard.eyes)
        {
            eye.visionCone.status = FOV2DVisionCone.Status.Suspicious;
        }
        guard.anim.CrossFade("atkReady");
        guard.FaceTarget(guard.guardedGemHolder.transform);

        // 找到最近的宝石
        for (ushort idx = 0; idx < (Globals.LevelController as TutorialLevelController).unstolenGems.Count; ++idx)
        {
            UnityEngine.GameObject gem = (Globals.LevelController as TutorialLevelController).unstolenGems[idx];
            Pathfinding.Path p = Pathfinding.ABPath.Construct(transform.position, gem.transform.position, null);
            p.callback += OnPathToGemComplete;
            p.gem = gem;
            AstarPath.StartPath(p);
        }        
    }

    
    public void OnPathToGemComplete(Pathfinding.Path p)
    {
        tempPathes.Add(p);
        if (tempPathes.Count == (Globals.LevelController as TutorialLevelController).unstolenGems.Count)
        {
            float shortest = UnityEngine.Mathf.Infinity;
            UnityEngine.GameObject nearestGem = null;
            for (int idx = 0; idx < tempPathes.Count; ++idx)
            {
                Pathfinding.Path path = tempPathes[idx];
                float length = path.GetTotalLength();
                if (length < shortest)
                {
                    shortest = length;
                    nearestGem = path.gem;
                }
            }
            UnityEngine.Debug.Log("go to guard nearest gem");
            guard.guardedGemHolder = nearestGem;
            Pathfinding.Node birthNode = Globals.maze.pathFinder.GetSingleNode(nearestGem.transform.position, true);
            guard.birthNode = birthNode;
            guard.patrol.InitPatrolRoute();
            Invoke("BackToNewBirthNodePos", 1.5f);
            tempPathes.Clear();
        }        
    }

    public override void Stop()
    {
        CancelInvoke("BackToNewBirthNodePos");
        base.Stop();
    }

    void BackToNewBirthNodePos()
    {
        guard.backing.Excute();
    }
}
