
public class MagicThiefCamera : Actor
{
    float dragCamSpeed = 0.02f;    
    float touchBorderMoveSpeed = 0.3f;
    [UnityEngine.HideInInspector]
    public float disScale = 1.0f;

    public UnityEngine.Vector2 restriction_x = new UnityEngine.Vector2(-5, 5);
    public UnityEngine.Vector2 restriction_y = new UnityEngine.Vector2(-5, 5);

    public UnityEngine.Transform target;
    public UnityEngine.GameObject MiniMapPlane;
    public UnityEngine.GameObject viewportFrame;
    float plane_width;
    float plane_height;
    UnityEngine.Rect MiniMapRect;
    float zCache;

    public override void Awake()
    {
        base.Awake();
        Globals.cameraFollowMagician = this;
        MiniMapPlane = Globals.getChildGameObject(gameObject, "MiniMapPlane");
        if (MiniMapPlane != null)
        {
            float view_port_height = GetComponent<UnityEngine.Camera>().orthographicSize;
            float view_port_width = GetComponent<UnityEngine.Camera>().orthographicSize * UnityEngine.Screen.width / UnityEngine.Screen.height;
            plane_width = 11.0f * MiniMapPlane.transform.localScale.x;
            plane_height = 11.0f * MiniMapPlane.transform.localScale.z;
            MiniMapPlane.transform.localPosition = new UnityEngine.Vector3(
                view_port_width - plane_width * 0.5f,
                view_port_height - plane_height * 0.5f,
                MiniMapPlane.transform.localPosition.z);
            MiniMapRect = new UnityEngine.Rect();
            UnityEngine.Vector3 plane_screen_pos = GetComponent<UnityEngine.Camera>().WorldToScreenPoint(MiniMapPlane.transform.localPosition);
            MiniMapRect.center = plane_screen_pos;

            float plane_screen_width = 2f * (UnityEngine.Screen.width - plane_screen_pos.x);
            float plane_screen_height = 2f * (UnityEngine.Screen.height - plane_screen_pos.y);

            MiniMapRect.xMin = plane_screen_pos.x - plane_screen_width * 0.5f;
            MiniMapRect.yMin = plane_screen_pos.y - plane_screen_height * 0.5f;
            MiniMapRect.xMax = plane_screen_pos.x + plane_screen_width * 0.5f;
            MiniMapRect.yMax = plane_screen_pos.y + plane_screen_height * 0.5f;
            MiniMapPlane.SetActive(false);
            viewportFrame = Globals.getChildGameObject(gameObject, "viewport-frame");
            viewportFrame.SetActive(false);
        }
        

        zCache = transform.localPosition.z;
    }

    public void OpenMinimap()
    {
        MiniMapPlane.SetActive(true);
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
        SetDragSpeed(6f);
        SetTouchBorderMoveSpeed(25.0f);
    }

    public void SetDragSpeed(float speed)
    {
        dragCamSpeed = speed;
    }

    public void SetTouchBorderMoveSpeed(float speed)
    {
        touchBorderMoveSpeed = speed;
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

    Finger fingerOnMiniMap;
    public bool CheckFingerDownOnMiniMap(Finger finger)
    {
        UnityEngine.Vector3 finger_pos = GetComponent<UnityEngine.Camera>().ScreenToWorldPoint(finger.nowPosition);
        if (MiniMapPlane.activeSelf && MiniMapRect.Contains(finger.nowPosition))
        {
            fingerOnMiniMap = finger;
            DragOnMiniMap(fingerOnMiniMap);
            return true;
        }
        return false;
    }

    public bool CheckFingerUpOnMiniMap(Finger finger)
    {
        if (fingerOnMiniMap != null)
        {
            fingerOnMiniMap = null;
            return true;
        }
        return false;
    }

    public void DragToMove(Finger finger)
    {      
        if(UnityEngine.Application.isMobilePlatform)
        {
            // 移动平台，拖动
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
    }

    public void TouchBorderToMove()
    {
        UnityEngine.Vector3 mousePos = UnityEngine.Input.mousePosition;
        UnityEngine.Vector3 movementDirection = UnityEngine.Vector3.zero;

        if (UnityEngine.Screen.width - mousePos.x < 10)
        {
            movementDirection += UnityEngine.Vector3.right;
        }
            // 上
        if (UnityEngine.Screen.height - mousePos.y < 10)
        {
            movementDirection += UnityEngine.Vector3.up;
        }
        if (mousePos.x < 10)
        {
            movementDirection += UnityEngine.Vector3.left;
        }
        if (mousePos.y < 10)
        {
            movementDirection += UnityEngine.Vector3.down;
        }
        movementDirection.Normalize();
        transform.position += movementDirection * touchBorderMoveSpeed;
        transform.position = RestrictPosition(transform.position);
    }

    public void DragOnMiniMap(Finger finger)
    {
        UnityEngine.Vector3 finger_pos = GetComponent<UnityEngine.Camera>().ScreenToWorldPoint(finger.nowPosition);
        if (fingerOnMiniMap != null)
        {
            if (MiniMapRect.Contains(finger.nowPosition))
            {
                float x_ratio = (finger.nowPosition.x - MiniMapRect.xMin) / MiniMapRect.width;
                float y_ratio = (finger.nowPosition.y - MiniMapRect.yMin) / MiniMapRect.height;
                UnityEngine.Vector2 xyPos = Globals.maze.GetMiniMapProjectPosition(x_ratio, y_ratio);
                transform.position = new UnityEngine.Vector3(xyPos.x, xyPos.y, zCache);
            }
        }
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        if (bStaring)
        {
            //transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));           
            transform.position = Globals.stealingController.magician.transform.position + new UnityEngine.Vector3(0, 0, zCache);
        }
        else if (target != null)
        {
            transform.position = target.position;
            transform.position = RestrictPosition(transform.position);
        }

        TouchBorderToMove();
    }

    public UnityEngine.Vector3 RestrictPosition(UnityEngine.Vector3 pos)
    {
        pos.x = UnityEngine.Mathf.Clamp(pos.x, restriction_x.x, restriction_x.y);
        pos.y = UnityEngine.Mathf.Clamp(pos.y, restriction_y.x, restriction_y.y);
        pos.z = zCache;
        return pos;
    }
}
