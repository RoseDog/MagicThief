
public class MagicThiefCamera : Actor
{
    float dragCamSpeed = 0.02f;    
    [UnityEngine.HideInInspector]
    public float disScale = 1.0f;

    public UnityEngine.Vector2 restriction_x = new UnityEngine.Vector2(-5, 5);
    public UnityEngine.Vector2 restriction_y = new UnityEngine.Vector2(-5, 5);

    public UnityEngine.Transform target;
    public UnityEngine.GameObject MiniMapPlane;
    public UnityEngine.GameObject viewportFrame;
    public virtual void Awake()
    {
        Globals.cameraFollowMagician = this;
        MiniMapPlane = Globals.getChildGameObject(gameObject, "MiniMapPlane");
        MiniMapPlane.SetActive(false);
        viewportFrame = Globals.getChildGameObject(gameObject, "viewport-frame");
        viewportFrame.SetActive(false);
        base.Awake();
    }

    public void OpenMinimap()
    {
        MiniMapPlane.SetActive(true);
        float view_port_height = camera.orthographicSize;
        float view_port_width = camera.orthographicSize * UnityEngine.Screen.width / UnityEngine.Screen.height;
        float plane_width = 11.0f * MiniMapPlane.transform.localScale.x;
        MiniMapPlane.transform.localPosition = new UnityEngine.Vector3(
            view_port_width - plane_width * 0.5f,
            view_port_height - plane_width * 0.5f,
            MiniMapPlane.transform.localPosition.z);
        viewportFrame.SetActive(true);
    }

    public void CloseMinimap()
    {
        MiniMapPlane.SetActive(false);
        viewportFrame.SetActive(false);
    }

    public void Reset()
    {
        enabled = true;
        SetDragSpeed(0.06f);
    }

    public void SetDragSpeed(float speed)
    {
        dragCamSpeed = speed;
    }
    
    public UnityEngine.Vector3 GetHorForward()
    {
        UnityEngine.Vector3 cameraHorForward = transform.forward;
        cameraHorForward.y = 0.0f;
        return cameraHorForward.normalized;
    }

    public UnityEngine.Vector3 GetHorRight()
    {
        UnityEngine.Vector3 cameraHorRight = transform.right;
        cameraHorRight.y = 0.0f;
        return cameraHorRight.normalized;
    }

    public void SetDisScale(float scale)
    {
		disScale = UnityEngine.Mathf.Clamp(disScale + scale, 0.3f, 2.0f);
		//dragCamSpeed = 0.05f * disScale;
    }    
    
    public void MoveToPoint(UnityEngine.Vector3 destination, int duration)
    {
        enabled = true;
        destination = RestrictPosition(destination);

        AddAction(new MoveTo(transform, destination, duration, false));
    }

    bool bStaring = false;
    public void StaringMagician(float duration)
    {
        bStaring = true;
        Invoke("EndStaring", duration);
    }

    void EndStaring()
    {
        UnityEngine.Debug.Log("EndStaring");
        bStaring = false;
        enabled = false;
    }

    public void DragToMove(Finger finger)
    {
		// two fingers touch , drag camaera not allowed
		Finger finger0 = Globals.input.GetFingerByID(0);
		Finger finger1 = Globals.input.GetFingerByID(1);
		if (!(finger0.enabled && finger1.enabled))
		{
			UnityEngine.Vector2 finger_move_delta = finger.MovmentDelta();
            UnityEngine.Vector3 movementDirection = -finger_move_delta;
			transform.position += movementDirection * dragCamSpeed;
            transform.position = RestrictPosition(transform.position);
		}
    }

    public override void Update()
    {
        base.Update();
        if (bStaring)
        {
            //transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));           
            transform.position = Globals.magician.transform.position + new UnityEngine.Vector3(0,0, -1.0f);
        }
        else if (target != null)
        {
            transform.position = target.position;
            transform.position = RestrictPosition(transform.position);
        }        
    }

    public UnityEngine.Vector3 RestrictPosition(UnityEngine.Vector3 pos)
    {
        pos.x = UnityEngine.Mathf.Clamp(pos.x, restriction_x.x, restriction_x.y);
        pos.y = UnityEngine.Mathf.Clamp(pos.y, restriction_y.x, restriction_y.y);
        pos.z = -1.0f;
        return pos;
    }
}
