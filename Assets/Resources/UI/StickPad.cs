using UnityEngine;
using System.Collections;
public class StickPad : Button
{
    public override void Prepare()
    {
        float scale_factor = UnityEngine.Screen.width / 640.0f;
        GetComponent<GUITexture>().pixelInset = new UnityEngine.Rect(0.0f, 0.0f, 200.0f * scale_factor,
            200.0f * scale_factor);
        base.Prepare();
    }

	public void Active(Rect rect, Color color)
	{
		GetComponent<GUITexture>().color = color;
		GetComponent<GUITexture>().pixelInset = rect;
	}

	public Vector2 LimitStickMove(Vector2 fingerPos)
	{
		Vector2 guiPos = fingerPos;
		if (Vector2.Distance(guiPos, GetCenter()) > GetComponent<GUITexture>().pixelInset.width * 0.5f)
		{
			guiPos = GetCenter() + (guiPos - GetCenter()).normalized * GetComponent<GUITexture>().pixelInset.width * 0.5f;
		}
		return guiPos;
	}
}
