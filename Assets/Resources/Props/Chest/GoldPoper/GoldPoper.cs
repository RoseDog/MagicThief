using UnityEngine;
using System.Collections;
using System;

public class GoldPoper : Actor 
{    
    public Chest chest;
    ParticleSystem[] systems;

	public void InitParticleTex(int gold)
	{
		if (systems == null)
		{
			systems = new ParticleSystem[3];
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
			}
		}

		for (int i = 0; i < 3; ++i )
		{
			systems[i].gameObject.SetActive(false);
		}

		string value = ((int)gold).ToString();
        for (int i = 0; i < value.Length; ++i)
        {
            string s = value[i].ToString();
            int digit = 0;
            int.TryParse(s, out digit);
            systems[i].gameObject.SetActive(true);
            systems[i].renderer.material.mainTexture = (Texture2D)Resources.Load("Props/Chest/GoldPoper/" + digit, typeof(Texture2D));
        }
	}

    public void Pop()
    {
        _Pop();
        InvokeRepeating("_Pop", 1.0f, 1.0f);
    }

    public void _Pop()
    {
        foreach (ParticleSystem sys in systems)
        {
            sys.Emit(1);
        }

        chest.LostGold();        
    }

    public void StopPop()
    {
        CancelInvoke("_Pop");
    }
}
