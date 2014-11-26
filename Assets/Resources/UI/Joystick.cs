using UnityEngine;
using System.Collections;
public class Joystick : Button
{
	public ButtonEvent Evt_Slided;
    public ButtonEvent Evt_Moving;
    public ButtonEvent Evt_FirstMove;

	public Vector2 position;                                // [-1, 1] in x,y

	public float slideDisThreshold = 80.0f;
	public float slideTimeThreshold = 0.5f;

    [HideInInspector]
	public StickPad pad;
	private Rect originalRect;
	private bool firstMove = false;

    public override void Prepare()
    {
        float scale_factor = UnityEngine.Screen.width / 640.0f;
        guiTexture.pixelInset = new UnityEngine.Rect(0.0f, 0.0f, 200.0f * scale_factor,
            200.0f * scale_factor);
        base.Prepare();        
        pad = GetComponentInChildren<StickPad>();
        originalRect = guiTexture.pixelInset;
        ResetGUI();
        Globals.joystick = this;
    }

	void MoveStick()
	{
		Rect tmprect = guiTexture.pixelInset;

		// 不能超过边缘
		Vector2 guiPos = pad.LimitStickMove(touchingFinger.nowPosition);

		tmprect.x = guiPos.x;
		tmprect.y = guiPos.y;

		// 移到中心
		tmprect.x = tmprect.x - halfGuiSize.x;
		tmprect.y = tmprect.y - halfGuiSize.y;

		guiTexture.pixelInset = tmprect;

		// Get a value between -1 and 1 based on the joystick graphic location
		position.x = (GetCenter().x - pad.GetCenter().x) / halfGuiSize.x;
		position.y = (GetCenter().y - pad.GetCenter().y) / halfGuiSize.y;
	}

	public override bool OnFingerDown(object sender)
	{
		if (base.OnFingerDown(sender))
		{
			originalRect = guiTexture.pixelInset;
			firstMove = true;
			pad.Active(guiTexture.pixelInset, guiTexture.color);
			MoveStick();
		}

		return false;
	}

	public override bool OnTouchingFingerMoving(object sender)
	{
		base.OnTouchingFingerMoving(sender);

		if (firstMove)
		{
			if (Evt_FirstMove != null)
			{
                Evt_FirstMove(GetAtkPrefix());
			}
			
			firstMove = false;
		}
		if (Evt_Moving != null)
		{
            Evt_Moving(GetAtkPrefix());
		}

		MoveStick();

		return true;
	}

	public override bool OnTouchingFingerUp(object sender)
	{
		if (Vector2.Distance(touchingFinger.holdingPosition, touchingFinger.nowPosition) > slideDisThreshold
					&& touchingFinger.timeSinceMoving < slideTimeThreshold)
		{
			if (Evt_Slided != null)
			{
                Evt_Slided(GetAtkPrefix());
			}
		}
		return base.OnTouchingFingerUp(sender);
	}

	public override void ResetGUI()
	{
		base.ResetGUI();
		position = Vector2.zero;
		guiTexture.pixelInset = originalRect;
	}
}