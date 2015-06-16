public struct SpriteAnim
{
    public System.Collections.Generic.List<UnityEngine.Sprite> spriteList;
    public System.Collections.Generic.Dictionary<int, UnityEngine.Events.UnityAction> events;
    public double speed;
    public bool clampForever;
}

public class SpriteSheet : UnityEngine.MonoBehaviour
{
    public System.Collections.Generic.Dictionary<System.String, SpriteAnim> _animationList = new System.Collections.Generic.Dictionary<string, SpriteAnim>();
    public UnityEngine.Sprite[] _sprites;
    public Actor _actor;
    public bool initialized = false;
    public void Awake()
    {
        
    }
    public void init()
    {
        initialized = true;
//         int rows = 1;
//         int cols = 4;
//         string filePath = gameObject.name;
//         UnityEngine.TextAsset text = UnityEngine.Resources.Load(filePath, typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
//         UnityEngine.Texture2D texture = new UnityEngine.Texture2D(0, 0, UnityEngine.TextureFormat.ETC2_RGBA1, false);
//         texture.LoadImage(text.bytes);
//         texture.anisoLevel = 0;
//         texture.filterMode = UnityEngine.FilterMode.Point;
// 
//         for (int j = 0; j < rows; j++)
//         {
//             System.Collections.Generic.List<UnityEngine.Sprite> animFrames = new System.Collections.Generic.List<UnityEngine.Sprite>();
//             for (int i = 0; i < cols; i++)
//             {
//                 UnityEngine.Rect spriteRect = new UnityEngine.Rect(texture.width / cols * i,
//                 texture.height / rows * j,
//                 texture.width / cols,
//                 texture.height / rows);
//                 UnityEngine.Sprite tempSprite = UnityEngine.Sprite.Create(texture, spriteRect, new UnityEngine.Vector2(0.5f, 0.5f));
//                 animFrames.Add(tempSprite);
//             }
//             _sprites.Add(animFrames);
//         }
        _actor = GetComponentInParent<Actor>();
        if (_actor != null)
        {
            _actor.gameObject.name = Globals.StripCloneString(_actor.gameObject.name);
            UnityEngine.Debug.Log(_actor.gameObject.name);
            _sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/" + _actor.gameObject.name + "_Sprite");
        }
        else
        {
            gameObject.name = Globals.StripCloneString(gameObject.name);
            _sprites = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Avatar/" + gameObject.name + "_Sprite");
        }        
    }

    int sprite_add_idx = 0;
    public void AddAnim(System.String name, int frame_count, double speed = 1.0f, bool clampForever = false)
    {
        if (!initialized)
        {
            init();
        }

        System.Collections.Generic.List<UnityEngine.Sprite> animation = new System.Collections.Generic.List<UnityEngine.Sprite>();
        SpriteAnim anim = new SpriteAnim();
        anim.spriteList = new System.Collections.Generic.List<UnityEngine.Sprite>();
        anim.events = new System.Collections.Generic.Dictionary<int, UnityEngine.Events.UnityAction>();
        anim.speed = speed;
        anim.clampForever = clampForever;

        for (int i = 0; i < frame_count;++i )
        {
            anim.spriteList.Add(_sprites[sprite_add_idx]);
            ++sprite_add_idx;
            if (sprite_add_idx >= _sprites.Length)
            {
                sprite_add_idx = sprite_add_idx % _sprites.Length;
            }            
        }

        _animationList.Add(name, anim);
    }



    public void CreateAnimationByName(System.String name, double speed = 1.0f, bool clampForever = false)
    {
        if (!initialized)
        {            
            init();            
        }
        System.Collections.Generic.List<UnityEngine.Sprite> animation = new System.Collections.Generic.List<UnityEngine.Sprite>();
        SpriteAnim anim = new SpriteAnim();
        anim.spriteList = new System.Collections.Generic.List<UnityEngine.Sprite>();
        anim.events = new System.Collections.Generic.Dictionary<int, UnityEngine.Events.UnityAction>();
        anim.speed = speed;
        anim.clampForever = clampForever;
        foreach (UnityEngine.Sprite sprite in _sprites)
        {
            int idx = sprite.name.LastIndexOf("_");
            if (idx != -1 && sprite.name.Substring(0, idx).Equals(name))
            {
                anim.spriteList.Add(sprite);
            }            
        }
        Globals.Assert(anim.spriteList.Count != 0, "empty animation");
        _animationList.Add(name, anim);
    }


    public void CreateAnimationBySprites(
        System.Collections.Generic.List<UnityEngine.Sprite> sprites, 
        System.String name, double speed = 1.0f, bool clampForever = false)
    {        
        System.Collections.Generic.List<UnityEngine.Sprite> animation = new System.Collections.Generic.List<UnityEngine.Sprite>();
        SpriteAnim anim = new SpriteAnim();
        anim.spriteList = new System.Collections.Generic.List<UnityEngine.Sprite>();
        anim.events = new System.Collections.Generic.Dictionary<int, UnityEngine.Events.UnityAction>();
        anim.speed = speed;
        anim.clampForever = clampForever;
        foreach (UnityEngine.Sprite sprite in sprites)
        {
            anim.spriteList.Add(sprite);
        }
        _animationList.Add(name, anim);
    }

    public bool HasAnimation(System.String name)
    {
        return _animationList.ContainsKey(name);
    }

    public int GetAnimationLength(System.String name)
    {
        return _animationList[name].spriteList.Count * spriteChangeFrequent;
    }

    public int GetAnimationLengthWithSpeed(System.String name)
    {
        return (int)(_animationList[name].spriteList.Count * spriteChangeFrequent
            * (1.0f / GetAnimationSpeed(name)));
    }

    public double GetAnimationSpeed(System.String name)
    {
        return _animationList[name].speed;
    }

    public void AddAnimationEvent(System.String name, int frame, UnityEngine.Events.UnityAction action)
    {
        if (frame == -1)
        {
            frame = _animationList[name].spriteList.Count - 1;
        }
        _animationList[name].events.Add(frame, action);
    }


    public void Play(System.String anim)
    {
        if (_currentAnim == anim)
        {
            return;
        }
        if (!initialized)
        {
            init();
        }
        _currentAnim = anim;
        frameIdx = 0;
        frameCount = 1000000;// 这样会立刻切换Frame
        Update();
    }
    System.String _currentAnim = "";
    int frameIdx = 0;
    int frameCount;
    int spriteChangeFrequent = 6;
    public int GetSpriteChangeFrequent()
    {
        return spriteChangeFrequent;
    }

    void Update()
    {
        ++frameCount;        
        if (_currentAnim != "")
        {
            SpriteAnim anim = _animationList[_currentAnim];
            double scaledFrame = frameCount * anim.speed;
            if (scaledFrame > 6)
            {
                if (anim.events.ContainsKey(frameIdx-1))
                {
                    anim.events[frameIdx-1].Invoke();
                }
                // 动画会被invoke改变
                if (_animationList[_currentAnim].spriteList != anim.spriteList)
                {
                    return;
                }

                if (!anim.clampForever)
                {
                    frameIdx = frameIdx % anim.spriteList.Count;
                }
                else
                {
                    frameIdx = UnityEngine.Mathf.Clamp(frameIdx, 0, anim.spriteList.Count-1);
                }
                
                System.Collections.Generic.List<UnityEngine.Sprite> spriteList = anim.spriteList;
                if (_actor)
                {
                    _actor.spriteRenderer.sprite = spriteList[frameIdx];
                }
                else
                {
                    UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
                    if (image)
                    {
                        image.sprite = spriteList[frameIdx];
                        image.rectTransform.sizeDelta = spriteList[frameIdx].rect.size;
                    }                               
                }
                
                frameIdx++;
                
                frameCount = 0;                                
            }
        }
    }
}