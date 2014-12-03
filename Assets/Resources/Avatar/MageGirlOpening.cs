﻿public class MageGirlOpening : UnityEngine.MonoBehaviour 
{
    System.Collections.ArrayList animationNamesArray = new System.Collections.ArrayList();
    int anim_idx = 0;
	// Use this for initialization
	void Start () 
    {
        for (int idx = 0; idx < 1; ++idx)
        {
            Finger finger = Globals.input.GetFingerByID(idx);
            finger.Evt_Down += OnFingerDown;
        }

        animationNamesArray.Add("A_Greeting_1");
        animationNamesArray.Add("idle");
        animationNamesArray.Add("A_Shake_Hand_1");        
	}

    public bool OnFingerDown(object sender)
    {
        Finger finger = sender as Finger;
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 11;
        UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(finger.nowPosition);
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
        {
            animation.CrossFade(animationNamesArray[anim_idx] as System.String, 1.0f);
            ++anim_idx;
            anim_idx = anim_idx%3;
        }
        return true;
    }

    public void GreetingEnd()
    {
        animation.CrossFade("A_Stand", 0.2f);    
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}