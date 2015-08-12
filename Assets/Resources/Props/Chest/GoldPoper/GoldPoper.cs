using UnityEngine;
using System.Collections;
using System;

public class GoldPoper : Actor 
{    
    public Chest chest;
    ParticleSystem[] systems;

	public void InitParticleTex(int gold)
    {
        int digit_count = 5;
		if (systems == null)
		{            
            systems = new ParticleSystem[digit_count];
			ParticleSystem[] temp = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem sys in temp)
			{
				if (sys.gameObject.name.Equals("1"))
				{
					systems[0] = sys;
				}
				else if (sys.gameObject.name.Equals("10"))
				{
					systems[1] = sys;
				}
				else if (sys.gameObject.name.Equals("100"))
				{
					systems[2] = sys;
				}
                else if (sys.gameObject.name.Equals("1000"))
                {
                    systems[3] = sys;
                }
                else if (sys.gameObject.name.Equals("10000"))
                {
                    systems[4] = sys;
                }
			}
		}

        for (int i = 0; i < digit_count; ++i)
		{
			systems[i].gameObject.SetActive(false);
		}

		string value = gold.ToString();
        for (int i = 0; i < value.Length; ++i)
        {
            string s = value[i].ToString();
            int digit = 0;
            int.TryParse(s, out digit);
            systems[i].gameObject.SetActive(true);
            systems[i].GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Props/Chest/GoldPoper/" + digit, typeof(Texture2D));
        }
	}
    Cocos2dAction PopAction;
    public void Pop()
    {
        _Pop();
        PopAction = RepeatingCallFunction(40, ()=>_Pop());
    }

    public void _Pop()
    {
        if(chest.goldLast > 1)
        {
            foreach (ParticleSystem sys in systems)
            {
                sys.Emit(1);
            }

            chest.LostGold();        
        }        
    }

    public void StopPop()
    {
        if (PopAction != null)
        {
            RemoveAction(ref PopAction);
        }        
    }
}
