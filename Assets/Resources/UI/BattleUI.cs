using UnityEngine;
using System.Collections;

public class BattleUI : MonoBehaviour
{
	Button[] buttons;
	public Joystick joystick;
    public Magician magician;

    public void Prepare()
    {
        buttons = FindObjectsOfType(typeof(Button)) as Button[];
        foreach(Button btn in buttons)
        {
            btn.magician = magician;
            btn.Prepare();
        }

        joystick = transform.Find("R_Joystick").GetComponent<Joystick>();

        UnityEngine.Rect inset = joystick.guiTexture.pixelInset;
        joystick.guiTexture.pixelInset = new UnityEngine.Rect(UnityEngine.Screen.width * 0.5f - joystick.halfGuiSize.x, 80, inset.width, inset.height);
        joystick.pad.guiTexture.pixelInset = joystick.guiTexture.pixelInset;
    }

	public void Hide()
	{
		foreach (Button button in buttons)
		{
			button.Visible(false);
		}
	}

	public void Show()
	{
		foreach (Button button in buttons)
		{
			button.Visible(true);
		}
	}

	public bool BackToMenu(object sender)
	{
		Application.LoadLevel("StartUI");
		return true;
	}
}
