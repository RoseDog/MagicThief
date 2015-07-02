using UnityEngine;
using System.Collections;

public delegate bool ButtonEvent(string value);

public class Button : MonoBehaviour
{
    public ButtonEvent Evt_BtnDown;
    public ButtonEvent Evt_BtnUp;
    public ButtonEvent Evt_BtnDoubleClick;
    public ButtonEvent Evt_BtnDoubleDown;
    public ButtonEvent Evt_BtnLongPush;
    public ButtonEvent Evt_BtnClick;
    public ButtonEvent Evt_BtnFling;

	public Finger touchingFinger;

	double longPushThreshold = 0.5f;

	protected Rect defaultRect;                          // Default position / extents of the joystick graphic

	public Vector2 halfGuiSize;
	protected Vector2 guiCenter;
	protected Color originalColor;

	double timeLastClick = UnityEngine.Mathf.NegativeInfinity;

	string atkPreFix;

    public Magician magician;


    public virtual void Prepare()
    {
        if (name.Contains("_"))
        {
            atkPreFix = name.Substring(0, name.IndexOf("_"));
        }

        // This is an offset for touch input to match with the top left
        // corner of the GUI
        halfGuiSize.x = GetComponent<GUITexture>().pixelInset.width * 0.5f;
        halfGuiSize.y = GetComponent<GUITexture>().pixelInset.height * 0.5f;

        originalColor = GetComponent<GUITexture>().color;

        for (int idx = 0; idx < 3; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down += OnFingerDown;
        }
    }

	public string GetAtkPrefix()
	{
		return atkPreFix;
	}


	public void Visible(bool visibility)
	{
		if (!visibility && touchingFinger != null)
		{
			OnTouchingFingerUp(null);
		}
		GetComponent<GUITexture>().enabled = visibility;
	}


	public bool IsVisible()
	{
		return GetComponent<GUITexture>().enabled;
	}

	public Vector2 GetCenter()
	{
		return new Vector2(GetComponent<GUITexture>().pixelInset.x + halfGuiSize.x, GetComponent<GUITexture>().pixelInset.y + halfGuiSize.y);
	}

	public virtual bool OnFingerDown(object sender)
	{
		if (touchingFinger == null && IsVisible())
		{
			Finger finger = (Finger)sender;
			if (GetComponent<GUITexture>().HitTest(new Vector3(finger.nowPosition.x, finger.nowPosition.y)))
			{
				touchingFinger = finger;
				GetComponent<GUITexture>().color = Color.gray;

				if (Evt_BtnDown != null)
					Evt_BtnDown(GetAtkPrefix());

				if (Time.time - timeLastClick < 0.2f)
				{
					if (Evt_BtnDoubleDown != null)
                        Evt_BtnDoubleDown(GetAtkPrefix());
				}

				touchingFinger.Evt_Moving += OnTouchingFingerMoving;
				touchingFinger.Evt_Up += OnTouchingFingerUp;
				return true;
			}
		}

		return false;
	}

	public virtual bool OnTouchingFingerMoving(object sender)
	{
		if (IsVisible())
		{
			if (Evt_BtnFling != null)
			{
                Evt_BtnFling(GetAtkPrefix());
			}
		}

		return true;
	}

	public virtual bool OnTouchingFingerUp(object sender)
	{
		if (!IsVisible())
		{
			return true;
		}

		if (Evt_BtnUp != null)
            Evt_BtnUp(GetAtkPrefix());

		if (touchingFinger.timeSinceTouchBegin > longPushThreshold)
		{
			if (Evt_BtnLongPush != null)
                Evt_BtnLongPush(GetAtkPrefix());
		}
		else
		{
			if (Time.time - timeLastClick < 0.2f)
			{
				if (Evt_BtnDoubleClick != null)
                    Evt_BtnDoubleClick(GetAtkPrefix());
			}
			else if (Evt_BtnClick != null)
                Evt_BtnClick(GetAtkPrefix());
			timeLastClick = Time.time;
		}

		touchingFinger.Evt_Moving -= OnTouchingFingerMoving;
		touchingFinger.Evt_Up -= OnTouchingFingerUp;

		ResetGUI();

		return true;
	}

	public virtual void ResetGUI()
	{
		touchingFinger = null;
		GetComponent<GUITexture>().color = originalColor;
	}
}