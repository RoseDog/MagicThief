using System;
using System.Collections;
using System.Collections.Generic;

public class Patrol : GuardAction 
{
    int currentTargetIdx;
    public List<UnityEngine.Vector3> targetPoses = new List<UnityEngine.Vector3>();
    List<UnityEngine.GameObject> patrolNodes = new List<UnityEngine.GameObject>();
    public override void Awake()
    {
        base.Awake();
    }

	// Use this for initialization
	public void InitPatrolRoute() 
    {
        DestroyRouteNodes();
        targetPoses.Clear();

        if (guard.birthNode.walkable)
        {
            //Call Start in base script (AIPath)
            // 找到四个方向上最远可以到达的走廊
            AddTargetPosInDirection(Globals.EAST, 4);
            AddTargetPosInDirection(Globals.SOUTH, 6);
            AddTargetPosInDirection(Globals.WEST, 4);
            AddTargetPosInDirection(Globals.NORTH, 6);
        }
        

        System.Diagnostics.Debug.Assert(targetPoses.Count == 4);
	}

    void AddTargetPosInDirection(String direction, int patrolCellsCount)
    {
        Pathfinding.Node node = Globals.pathFinder.GetSingleNode(transform.position,true);
        float nodeSize = Globals.pathFinder.graph.nodeSize;
        for (int i = 0; i < patrolCellsCount; ++i)
        {
            // 生成表示行走区域的方块
            UnityEngine.GameObject cube = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
            cube.transform.localScale = new UnityEngine.Vector3(nodeSize, 0.5f, nodeSize);
            cube.transform.position = Globals.GetPathNodePos(node) + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f);
            cube.transform.parent = transform;

            patrolNodes.Add(cube);

            UnityEngine.MeshRenderer meshRenderer = cube.GetComponentInChildren<UnityEngine.MeshRenderer>();
            meshRenderer.material.SetColor("_Color", UnityEngine.Color.green);

            UnityEngine.Vector3 nextNodePos = cube.transform.position;
           
            if (direction == Globals.EAST)
            {
                nextNodePos += new UnityEngine.Vector3(nodeSize, 0.0f, 0.0f);
            }
            else if (direction == Globals.SOUTH)
            {
                nextNodePos += new UnityEngine.Vector3(0.0f, 0.0f, -nodeSize);
            }
            else if (direction == Globals.WEST)
            {
                nextNodePos += new UnityEngine.Vector3(-nodeSize, 0.0f, 0.0f);
            }
            else if (direction == Globals.NORTH)
            {
                nextNodePos += new UnityEngine.Vector3(0.0f, 0.0f, nodeSize);
            }

            // 如果没有可以行走的node了
            Pathfinding.Node nextNode = Globals.pathFinder.GetSingleNode(nextNodePos, true);
            if (nextNode == null)
            {
                targetPoses.Add(Globals.GetPathNodePos(node));
                return;
            }

            node = nextNode;
        }

        targetPoses.Add(Globals.GetPathNodePos(node));
    }

    public void RouteConfirmed()
    {
        foreach (UnityEngine.GameObject node in patrolNodes)
        {
            node.transform.parent = null;
        }        
    }

    public void SetRouteNodesVisible(bool visible)
    {
        foreach (UnityEngine.GameObject node in patrolNodes)
        {
            node.SetActive(visible);
        }
    }

    public void DestroyRouteNodes()
    {
        foreach (UnityEngine.GameObject node in patrolNodes)
        {
            DestroyImmediate(node);
        }
        patrolNodes.Clear();
    }

    public void RouteRemoving()
    {
        foreach (UnityEngine.GameObject node in patrolNodes)
        {
            node.transform.parent = transform;
        }
    }

    public override void Excute()
    {
        base.Excute();
        guard.moving.canMove = true;
        guard.moving.canSearch = false;
        _beginPatrol();
        UnityEngine.Debug.Log("patrol");
    }

    public void NextPatrol()
    {
        currentTargetIdx = (currentTargetIdx + 1) % 4;
        this.Invoke("_beginPatrol", 2.0f);
    }

    void _beginPatrol()
    {
        guard.moving.GetSeeker().StartPath(guard.moving.GetFeetPosition(), targetPoses[currentTargetIdx]);
    }

    public override void Stop()
    {
        base.Stop();
        guard.moving.canMove = false;
        UnityEngine.Debug.Log("stop Patrol");
    }
}
