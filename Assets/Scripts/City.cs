public class City : UnityEngine.MonoBehaviour 
{
    Finger fingerDownOnMap;
    TargetBuilding target;
    void Awake()
    {
        if (Globals.input == null)
        {
            UnityEngine.GameObject mgrs_prefab = UnityEngine.Resources.Load("GlobalMgrs") as UnityEngine.GameObject;
            UnityEngine.GameObject mgrs = UnityEngine.GameObject.Instantiate(mgrs_prefab) as UnityEngine.GameObject;
        }
        Globals.input.enabled = true;
    }
	// Use this for initialization
	void Start () 
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(0);
            finger.Evt_Down += OnDragFingerDown;
            finger.Evt_Moving += OnDragFingerMoving;
            finger.Evt_Up += OnDragFingerUp;
        }
	}


    public bool OnDragFingerDown(object sender)
    {
        fingerDownOnMap = sender as Finger;
 
        return true;
    }


    public bool OnDragFingerMoving(object sender)
    {
        Globals.cameraFollowMagician.DragToMove(fingerDownOnMap);

        return true;
    }

    public bool OnDragFingerUp(object sender)
    {
        if (fingerDownOnMap.timeSinceTouchBegin < 0.5f &&
            UnityEngine.Vector2.Distance(fingerDownOnMap.beginPosition, fingerDownOnMap.nowPosition) < 10.0f)
        {
            TargetBuilding building = Globals.FingerRayToObj<TargetBuilding>(
                Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), 16, fingerDownOnMap);

            if (building != null)
            {
                if (target != null && building != target)
                {
                    target.Unchoose();
                }

                building.Choosen();
                target = building;
            }                        
        }

        return true;
    }    
}
