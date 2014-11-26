using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class Globals
{
    public static readonly String EAST = "E";
    public static readonly String SOUTH = "S";
    public static readonly String WEST = "W";
    public static readonly String NORTH = "N";
    public static readonly String[] DIRECTIONS = { EAST, SOUTH, WEST, NORTH };
    public static bool SHOW_MACE_GENERATING_PROCESS = false;
    public static bool SHOW_ROOMS = false;
    public static float CREATE_MAZE_TIME_STEP = 0.1f;
    public static PathFinder pathFinder;
    public static InputMgr input;
    public static CameraFollow cameraFollowMagician;
    public static CameraForDefender cameraForDefender;
    public static MapGenerate map;
    public static SelectGuard selectGuardUI;
    public static Guard choosenGuard;
    public static Joystick joystick;
    public static AsyncLoad asyncLoad;
    public static void ReadGlobalInfo()
    {
//         string path = Path.Combine(Application.persistentDataPath, "GlobalInfo.txt");
//         StreamReader sr = new StreamReader(path, false);
//         playerName = sr.ReadLine();
//         unlockedLevels = Convert.ToInt32(sr.ReadLine());
//         Globals.money = System.Convert.ToSingle(sr.ReadLine());
//         Globals.skillPoints = System.Convert.ToInt32(sr.ReadLine());
//         sr.Close();
    }

    public static void SaveGlobalInfo()
    {
//         string path = Path.Combine(Application.persistentDataPath, "GlobalInfo.txt");
//         StreamWriter sw = new StreamWriter(path, false);
//         sw.WriteLine(playerName);
//         sw.WriteLine(unlockedLevels.ToString());
//         sw.WriteLine(money.ToString());
//         sw.WriteLine(skillPoints.ToString());
//         sw.Flush();
//         sw.Close();
    }

    static public void DestroyGuard(Guard guard)
    {
        guard.patrol.DestroyRouteNodes();
        UnityEngine.Object.DestroyImmediate(guard.canvasForCommandBtns.gameObject);
        UnityEngine.Object.DestroyImmediate(guard.gameObject);
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, String withName)
    {
        Transform ts = fromGameObject.transform.FindChild(withName);
        if (ts)
        {
            return ts.gameObject;
        }

        return null;
    }

    static public UnityEngine.Vector3 GetPathNodePos(Pathfinding.Node node)
    {
        return new UnityEngine.Vector3(node.position.x / 1000.0f,
            node.position.y / 1000.0f, node.position.z / 1000.0f);
    }
}
