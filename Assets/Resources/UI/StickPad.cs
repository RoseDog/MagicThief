using UnityEngine;
using System.Collections;
public class StickPad : Button
{
	public void Active(Rect rect, Color color)
	{
		guiTexture.color = color;
		guiTexture.pixelInset = rect;
	}

	public Vector2 LimitStickMove(Vector2 fingerPos)
	{
		Vector2 guiPos = fingerPos;
		if (Vector2.Distance(guiPos, GetCenter()) > guiTexture.pixelInset.width * 0.5f)
		{
			guiPos = GetCenter() + (guiPos - GetCenter()).normalized * guiTexture.pixelInset.width * 0.5f;
		}
		return guiPos;
	}
}
