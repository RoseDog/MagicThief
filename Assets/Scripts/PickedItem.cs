public class PickedItem : Actor 
{
    FlyToScreenNumber fly;
    int cash = 300;
    public UnityEngine.SpriteRenderer holder;
    public int GetCash()
    {
        return cash;
    }
    
	public override void Awake () 
    {
        base.Awake();
        fly = GetComponent<FlyToScreenNumber>();        
        spriteRenderer = GetComponent<UnityEngine.SpriteRenderer>();
        if (fly != null)
        {
            int duration = UnityEngine.Random.Range(30, 50);
            UnityEngine.Vector3 pos_cache = transform.localPosition;
            AddAction(new RepeatForever(
                new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0, 0.1f, 0), duration / 2),
                new MoveTo(transform, pos_cache, duration / 2)));
        }
        
	}

    public override void TouchBegin(Actor other)
    {
        base.TouchBegin(other);
        Picked();
    }

    public void Picked()
    {
        int floatDuration = 40;

        if (fly != null)
        {
            // 宝石，加钱，
            Globals.Assert(Globals.stealingController.unstolenGems.Contains(transform.parent.gameObject));
            Globals.stealingController.unstolenGems.Remove(transform.parent.gameObject);

            transform.parent = null;
            ClearAllActions();
            transform.localPosition = transform.localPosition - new UnityEngine.Vector3(0, 0, 0.6f);

            fly.numberDelta = cash;
            // 飞到屏幕的金钱图标那里去
            SleepThenCallFunction(floatDuration + 5, () => FlyOff());
            GetComponent<UnityEngine.AudioSource>().Play();
        }
        else
        {
            if (Globals.stealingController)
            {
                Globals.stealingController.pickedItems.Add(gameObject.name);
            }            
            
            SleepThenCallFunction(floatDuration + 5, () => Destroy());
        }
        cash = 0;
        gameObject.layer = 26;
        AddAction(new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0.0f, 150.0f, 0.0f), floatDuration));     
    }

    void FlyOff()
    {
        fly.ToCashNumber(false);
    }

    public void Destroy()
    {
        DestroyObject(gameObject);
    }

    public override void FrameFunc()
    {
        base.FrameFunc();

        if (Globals.LevelController.fogTex != null)
        {
            bool infog = isInFog(transform.position);
            // 如果有holder
            if (holder)
            {
                // 那么两个都在雾里的时候，才是在雾里的
                if (infog && isInFog(holder.transform.position))
                {
                    infog = true;
                }
                else
                {
                    infog = false;
                }
            }            

            spriteRenderer.enabled = !infog;
            if (holder)
            {
                holder.enabled = !infog;
            }
        }        
    }


    bool isInFog(UnityEngine.Vector3 pos)
    {
        UnityEngine.Vector3 view_pos = Globals.LevelController.fogCam.WorldToViewportPoint(pos);
        int x = (int)((view_pos.x) * 256.0f);
        int y = (int)((view_pos.y) * 256.0f);
        UnityEngine.Color32 color = Globals.LevelController.fogTex.GetPixel(x, y);
        bool infog = true;
        if (color.a > 50)
        {
            infog = false;
        }
        else
        {
            infog = true;
        }
        return infog;
    }
}
