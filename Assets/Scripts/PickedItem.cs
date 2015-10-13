public class PickedItem : Actor 
{
    FlyToScreenNumber fly;
    int cash = 0;
    public UnityEngine.SpriteRenderer holder;
    public System.String item_id;
    public Cocos2dAction floatUpAction;
    public MultiLanguageUIText tip;
    
    public int GetCash()
    {
        return cash;
    }

    public void SetCash(int c)
    {
        cash = c;
    }
    
	public override void Awake () 
    {
        base.Awake();
        fly = GetComponent<FlyToScreenNumber>();
        tip.gameObject.SetActive(false);            
	}

    public override void Start()
    {
        base.Start();
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
        tip.gameObject.SetActive(true);
        if (cash > 0)
        {
            tip.text = "<color=yellow>+" + cash.ToString() + "</color>";
        }
        else
        {
            Globals.languageTable.SetText(tip, "item_picked", new System.String[] {gameObject.name, "1" });
        }        

        if (fly != null)
        {
            // 宝石，加钱，
            //Globals.Assert(Globals.stealingController.unstolenGems.Contains(transform.parent.gameObject));
            //Globals.stealingController.unstolenGems.Remove(transform.parent.gameObject);

            transform.parent = null;
            ClearAllActions();
            transform.localPosition = transform.localPosition - new UnityEngine.Vector3(0, 0, 0.6f);

            fly.numberDelta = cash;
            // 飞到屏幕的金钱图标那里去
            SleepThenCallFunction(floatDuration + 5, () => FlyOff());
            GetComponent<UnityEngine.AudioSource>().Play();

            // 落在地上的钱
            if (holder == null && Globals.stealingController)
            {
                Globals.stealingController.pickedCash.Add(item_id);
            }
        }
        else
        {
            if (Globals.stealingController)
            {
                Globals.stealingController.pickedItems.Add(item_id);
            }            
            
            SleepThenCallFunction(floatDuration + 5, () => Destroy());
        }
        cash = 0;
        gameObject.layer = 26;
        spriteRenderer.gameObject.layer = 26;
        if(shadow)
        {
            shadow.enabled = false;
        }
        floatUpAction = new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0.0f, 150.0f, 0.0f), floatDuration);
        AddAction(floatUpAction);
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

        if (floatUpAction == null && Globals.LevelController.fogTex != null)
        {           
            bool infog = Globals.LevelController.IsPointInFog(transform.position);
            // 如果有holder
            if (holder)
            {
                // 那么两个都在雾里的时候，才是在雾里的
                if (infog && Globals.LevelController.IsPointInFog(holder.transform.position))
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
}
