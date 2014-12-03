using UnityEngine;
using System.Collections;

public delegate bool FingerEvent(object sender);

public class Finger
{
    static public string FINGER_STATE_NONE = "None";
    static public string FINGER_STATE_DOWN = "FD";
    static public string FINGER_STATE_HOLDING = "H";
    static public string FINGER_STATE_UP = "FU";

    static public string FINGER_STATE_FAST_MOVING = "FM";
    static public string FINGER_STATE_SLOW_MOVING = "SM";

    static public string FINGER_STATE_HOVERING = "HOVER";


    static public string GESTURE_NONE = "None";
    static public string GESTURE_CLICK = "C";
    static public string GESTURE_DOUBLE_CLICK = "DC";
    static public string GESTURE_R = "R";
    static public string GESTURE_L = "L";
    static public string GESTURE_D = "D";
    static public string GESTURE_U = "U";
    static public string GESTURE_TWO_FIGURE_CLICK = "2C";

    static public float HALF_PI = Mathf.PI / 2.0f;
    static public float UP_GESTURE_RADIAN = 60.0f / 90.0f;


    public Finger(int fingerId, InputMgr mgr)
    {
        inputMgr = mgr;
        touchBegin = touchStaying = touchMoving = touchEnd = false;
        id = fingerId;
    }
    public InputMgr inputMgr;
    public bool enabled;
    private bool touchBegin;
    private bool touchStaying;
    private bool touchMoving;
    private bool touchEnd;
    private bool hovering;

    public Vector2 beginPosition;
    public Vector2 nowPosition;
    public Vector2 lastPosition;
    public Vector2 holdingPosition;

    public int id;


    // 从手指按下开始累计的时间
    public float timeSinceTouchBegin;

    // 从移动时开始累计的时间
    public float timeSinceMoving;

    // 从移动时开始累计的向量
    private float movingLength;

    public string currentState = FINGER_STATE_NONE;
    public string currentGesture = Finger.GESTURE_NONE;

    public float timeLastClick = UnityEngine.Mathf.NegativeInfinity;

    public FingerEvent Evt_Down;
    public FingerEvent Evt_Staying;
    public FingerEvent Evt_Moving;
    public FingerEvent Evt_Up;
    public FingerEvent Evt_Hovering;
    public FingerEvent Evt_FastMoving;
    public FingerEvent Evt_SlowMoving;


    public void Down(Vector2 pos)
    {
        if (IsPositionOnCanvas(pos))
        {
            return;
        }

        enabled = true;
        touchBegin = touchStaying = touchMoving = touchEnd = hovering = false;

        touchBegin = true;

        holdingPosition = beginPosition = nowPosition = pos;

        timeSinceTouchBegin = 0.0f;
        timeSinceMoving = UnityEngine.Mathf.Infinity;

        if (Evt_Down != null)
        {
            Evt_Down(this);
        }
    }

    public void Staying(Vector2 pos)
    {
        touchBegin = touchStaying = touchMoving = touchEnd = hovering = false;

        touchStaying = true;

        holdingPosition = lastPosition = nowPosition;

        timeSinceTouchBegin += Time.deltaTime;
        if (enabled)
        {
            if (Evt_Staying != null)
            {
                Evt_Staying(this);
            }
        }
    }

    public bool IsPositionOnCanvas(Vector3 position)
    {
        UnityEngine.Canvas[] canvases = GameObject.FindObjectsOfType<UnityEngine.Canvas>();
        foreach (UnityEngine.Canvas canvas in canvases)
        {
            UnityEngine.EventSystems.PointerEventData event_data = new UnityEngine.EventSystems.PointerEventData(GameObject.FindObjectOfType<UnityEngine.Canvas>().GetComponent<UnityEngine.EventSystems.EventSystem>());
            event_data.position = position;
            System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>().Raycast(event_data, results);
            if (results.Count != 0)
            {
                return true;
            }
        }
        return false;
    }

    public void Moving(Vector2 pos)
    {
        if (IsPositionOnCanvas(pos))
        {
            if (enabled)
            {
                Up(pos);
            }
            return;
        }
        else
        {
            if (!enabled)
            {
                Down(pos);
            }
        }

        touchBegin = touchStaying = touchMoving = touchEnd = hovering = false;

        touchMoving = true;

        lastPosition = nowPosition;
        nowPosition = pos;

        timeSinceMoving += Time.deltaTime;
        movingLength += (nowPosition - lastPosition).magnitude;
        if (enabled)
        {
            if (Evt_Moving != null)
            {
                Evt_Moving(this);
            }
        }
    }

    public Vector2 MovmentDelta()
    {
        return nowPosition - lastPosition;
    }

    public void Up(Vector2 pos)
    {

        touchBegin = touchStaying = touchMoving = touchEnd = hovering = false;

        touchEnd = true;

        lastPosition = nowPosition;
        nowPosition = pos;
        if (enabled)
        {
            if (Evt_Up != null)
            {
                Evt_Up(this);
            }
        }


        enabled = false;
    }

    public void Hovering()
    {
        touchBegin = touchStaying = touchMoving = touchEnd = hovering = false;

        hovering = true;

        if (Evt_Hovering != null)
        {
            Evt_Hovering(this);
        }
    }

    void _SetCurrentState(string state)
    {
        currentState = state;
    }

    public string ResolveState()
    {
        if (enabled)
        {
            if (touchBegin)
            {
                _SetCurrentState(FINGER_STATE_DOWN);
            }
            else if (touchEnd)
            {
                _SetCurrentState(FINGER_STATE_UP);
            }
            else if (touchStaying)
            {
                _SetCurrentState(FINGER_STATE_HOLDING);
            }
            else if (touchMoving)
            {
                if (currentState == FINGER_STATE_HOLDING || currentState == FINGER_STATE_DOWN)
                {
                    timeSinceMoving = UnityEngine.Mathf.Epsilon;
                    movingLength = (nowPosition - lastPosition).magnitude;
                }
                float speed = movingLength / timeSinceMoving;
                if (speed > inputMgr.movingSpeedThreshold)
                {
                    _SetCurrentState(FINGER_STATE_FAST_MOVING);

                    if (Evt_FastMoving != null)
                    {
                        Evt_FastMoving(this);
                    }
                }
                else if (speed < inputMgr.movingSpeedThreshold)
                {
                    _SetCurrentState(FINGER_STATE_SLOW_MOVING);

                    if (Evt_SlowMoving != null)
                    {
                        Evt_SlowMoving(this);
                    }
                }
            }
            else if (hovering)
            {
                _SetCurrentState(FINGER_STATE_HOVERING);
            }
        }

        return currentState;
    }

    public bool IsHovering()
    {
        return currentState == FINGER_STATE_HOVERING;
    }

    public bool IsDown()
    {
        return currentState == FINGER_STATE_DOWN;
    }

    public bool IsUp()
    {
        return currentState == FINGER_STATE_UP;
    }

    public bool IsFastMoving()
    {
        return currentState == FINGER_STATE_FAST_MOVING;
    }

    public bool IsSlowMoving()
    {
        return currentState == FINGER_STATE_SLOW_MOVING;
    }

    public bool IsMoving()
    {
        return IsSlowMoving() || IsFastMoving();
    }

    public string ResolveSingleFingerGesture()
    {
        currentGesture = Finger.GESTURE_NONE;
        if (currentState == Finger.FINGER_STATE_FAST_MOVING)
        {
            Vector2 line = holdingPosition - nowPosition;
            line.Normalize();
            float rad = Mathf.Abs(line.y / line.x);
            if (rad > Finger.UP_GESTURE_RADIAN)
            {
                if (line.y > 0)
                {
                    currentGesture = Finger.GESTURE_D;

                }
                else
                {
                    currentGesture = Finger.GESTURE_U;
                }
            }
            else
            {
                if (line.x < 0)
                {
                    currentGesture = Finger.GESTURE_R;
                }
                else
                {
                    currentGesture = Finger.GESTURE_L;
                }
            }
        }

        return currentGesture;
    }
}

public delegate bool KeyEvent(string key);
public delegate bool MouseEvent(string value);

public class InputMgr : MonoBehaviour
{
    public ArrayList fingers = new ArrayList();

    bool bBlock = false;

    // 阀值
    public float movingSpeedThreshold = 600.0f;

    UnityEngine.Touch m_touch;

    public KeyEvent Evt_WASD;
    public KeyEvent Evt_WASD_Up;
    public KeyEvent Evt_SpaceDown;
    public KeyEvent Evt_KeyAlpha0Up;
    public KeyEvent Evt_KeyAlpha1Up;
    public MouseEvent Evt_MouseLeftUp;
    public MouseEvent Evt_MouseRightUp;

    public float cameraPinchZoomSpeed = 0.05f;

    void Awake()
    {
        DontDestroyOnLoad(this);
        Globals.input = this;
        Prepare();
    }

    void OnLevelWasLoaded(int scene_id)
    {
        foreach (Finger f in fingers)
        {
            f.Evt_Down = null;
            f.Evt_FastMoving = null;
            f.Evt_Hovering = null;
            f.Evt_SlowMoving = null;
            f.Evt_Moving = null;
            f.Evt_Staying = null;
            f.Evt_Up = null;
        }
    }

    bool GetTouchByID(int id)
    {
        for (int idx = 0; idx < Input.touches.Length; ++idx)
        {
            if (Input.touches[idx].fingerId == id)
            {
                m_touch = Input.touches[idx];
                return true;
            }
        }

        return false;
    }

    public Finger GetFingerByID(int idx)
    {
        return (Finger)fingers[idx];
    }

    public void Block()
    {
        bBlock = true;
    }

    public void Rewind()
    {
        bBlock = false;
    }

    // Update is called once per frame
    public void Update()
    {
        //UnityEngine.Physics2D.Raycast();

        // 分别针对iphone和PC查询输入
        if (!bBlock)
        {
#if UNITY_IPHONE
			for (int idx = 0; idx < 3; ++idx)
			{
				Finger finger = GetFingerByID(idx);

				if (GetTouchByID(idx))
				{
					if (m_touch.phase == TouchPhase.Began)
					{
						finger.Down(m_touch.position);
					}
					else if (m_touch.phase == TouchPhase.Canceled || m_touch.phase == TouchPhase.Ended)
					{
						finger.Up(m_touch.position);
					}
					else
					{
						Vector2 pos = new Vector2(m_touch.position.x, m_touch.position.y);
						if (pos == finger.nowPosition)
						{
							finger.Staying(m_touch.position);
						}
						else
						{
							finger.Moving(m_touch.position);
						}
					}
				}
				else
				{
					finger.Hovering();
				}

				finger.ResolveState();
			}
#else
            for (int idx = 0; idx < 3; ++idx)
            {

                Finger finger = GetFingerByID(idx);

                // 下面的代码检测手指是否放在了UI上，如果是的话，就不发送finger消息了
                // 现在还有个问题。如果手指按下的时候在UI上，稍后移动手指，移出了ui范围，还是会继续发送FingerMoving这样的消息
                // 解决方法：手指在没接触到屏幕的时候应该删除，接收到down的消息才创建，这样，落到Ui上的touch就不会创建finger，就没有后面的消息了
                Vector2 mousePos = Input.mousePosition;


                string fingerName = "Finger" + idx.ToString();
                if (Input.GetButtonDown(fingerName) || Input.GetButtonUp(fingerName) || Input.GetButton(fingerName))
                {
                    if (Input.GetButtonDown(fingerName))
                    {
                        finger.Down(mousePos);
                    }
                    else if (Input.GetButtonUp(fingerName))
                    {
                        finger.Up(mousePos);
                    }
                    else if (Input.GetButton(fingerName))
                    {
                        Vector2 pos = new Vector2(mousePos.x, mousePos.y);
                        if (pos == finger.nowPosition)
                        {
                            finger.Staying(mousePos);
                        }
                        else
                        {
                            finger.Moving(mousePos);
                        }
                    }
                }
                else
                {
                    finger.Hovering();
                }

                finger.ResolveState();
            }


            // 键盘
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                if (Evt_KeyAlpha0Up != null)
                {
                    Evt_KeyAlpha0Up("0");
                }
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                if (Evt_KeyAlpha1Up != null)
                {
                    Evt_KeyAlpha1Up("1");
                }
            }

            //             if (Input.GetButtonDown("Space"))
            //             {
            //                 if (Evt_SpaceDown != null)
            //                 {
            //                     Evt_SpaceDown("");
            //                 }
            //             }

            if (Input.GetButtonDown("Horizontal") || Input.GetButton("Horizontal"))
            {
                if (Evt_WASD != null)
                {
                    Evt_WASD("WASD");
                }
            }

            if (Input.GetButtonDown("Vertical") || Input.GetButton("Vertical"))
            {
                if (Evt_WASD != null)
                {
                    Evt_WASD("");
                }
            }

            if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
            {
                if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
                {
                    if (Evt_WASD_Up != null)
                    {
                        Evt_WASD_Up("");
                    }
                }
            }

            // 鼠标
            if (Input.GetMouseButtonUp(0))
            {
                if (Evt_MouseLeftUp != null)
                {
                    Evt_MouseLeftUp("A");
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (Evt_MouseRightUp != null)
                {
                    Evt_MouseRightUp("B");
                }
            }
#endif
			Finger finger0 = GetFingerByID(0);
			Finger finger1 = GetFingerByID(1);
			if (finger0.enabled && finger1.enabled && (finger0.IsMoving()||finger1.IsMoving()))
			{
				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (finger0.lastPosition - finger1.lastPosition).magnitude;
				float touchDeltaMag = (finger0.nowPosition - finger1.nowPosition).magnitude;
				
				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
				MagicThiefCamera camera_now = null;
				if (Globals.cameraFollowMagician != null)
				{
					camera_now = Globals.cameraFollowMagician;
				}
				if (Globals.cameraForDefender != null)
				{
					camera_now = Globals.cameraForDefender;
					UnityEngine.Debug.Log(deltaMagnitudeDiff.ToString("f4"));
				}
				camera_now.SetDisScale(-deltaMagnitudeDiff * cameraPinchZoomSpeed);
			}
		}
	}
	
	public void Prepare()
	{
		for (int idx = 0; idx < 3; ++idx)
		{
			Finger finger = new Finger(idx, this);
			fingers.Add(finger);
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		// Send data to server
        if (stream.isWriting)
        {
            Vector3 pos = rigidbody.position;
            Quaternion rot = rigidbody.rotation;
            Vector3 velocity = rigidbody.velocity;
            Vector3 angularVelocity = rigidbody.angularVelocity;

            stream.Serialize(ref pos);
            stream.Serialize(ref velocity);
            stream.Serialize(ref rot);
            stream.Serialize(ref angularVelocity);
        }
        // Read data from remote client
        else
        {
            Vector3 pos = Vector3.zero;
            Vector3 velocity = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            Vector3 angularVelocity = Vector3.zero;
            stream.Serialize(ref pos);
            stream.Serialize(ref velocity);
            stream.Serialize(ref rot);
            stream.Serialize(ref angularVelocity);
        }
    }
}

