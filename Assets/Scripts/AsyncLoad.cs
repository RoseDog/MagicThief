public class AsyncLoad : UnityEngine.MonoBehaviour 
{
    UnityEngine.GameObject canvasForLoading;
    public System.String Next;
    public System.String LastLevelName;
    private UnityEngine.AsyncOperation loadingHangarOperation;
    int progress = 0;
    GUIBarScript gui_bar_script;
    UnityEngine.UI.Text ui_text;
    public void Awake()
    {
        Globals.asyncLoad = this;
        OnLevelWasLoaded(-1);
    }

    void OnLevelWasLoaded(int scene_id)
    {
        if (UnityEngine.Application.loadedLevelName == "loading")
        {
            canvasForLoading = UnityEngine.GameObject.Find("CanvasForLoading");
            gui_bar_script = canvasForLoading.GetComponentInChildren<GUIBarScript>();
            ui_text = canvasForLoading.GetComponentInChildren<UnityEngine.UI.Text>();
            FromLoadingSceneToNextScene();            
        }
    }

    public void FromLoadingSceneToNextScene()
    {
        StartCoroutine("_LoadSceneAsync");
    }

    public void ToLoadSceneAsync(System.String nextLevelName)
    {        
        Globals.EnableAllInput(false);
        Globals.transition.BlackOut(() => _ToLoadingScene(nextLevelName));                
    }

    public void _ToLoadingScene(System.String nextLevelName)
    {
        Next = nextLevelName;
        LastLevelName = UnityEngine.Application.loadedLevelName;
//        Globals.magician.gameObject.SetActive(false);
        UnityEngine.Application.LoadLevel("loading");
    }

    System.Collections.IEnumerator _LoadSceneAsync()
    {
        // 先做个假的，让人看到加载过程。以后有需求了用loadingHangarOperation.progress以及文件下载器做真的
        while (progress < 100 || progress == 100)
        {
            float val = (float)progress / 100.0f;
            gui_bar_script.Value = val;
            Globals.languageTable.SetText(ui_text, "level_loading_progress", new System.String[] { progress.ToString() });
            progress += 2;
            yield return new UnityEngine.WaitForSeconds(0.0f);
        }
        progress = 0;
        yield return new UnityEngine.WaitForSeconds(0.3f);
        if (Next != "")
        {
            while(true)
            {
                yield return new UnityEngine.WaitForSeconds(0.1f);
                if(Globals.socket.IsReady)
                {
                    break;
                }
            }
            loadingHangarOperation = UnityEngine.Application.LoadLevelAsync(Next);
            Globals.AvatarAnimationEventNameCache.Clear();
            
        }        
    }

    void Update()
    {
        if (loadingHangarOperation != null && loadingHangarOperation.isDone)
        {
            // 这一行无效。。为什么呢？
            //Globals.input.enabled = true;
        }
        else
        {
            loadingHangarOperation = null;
        }
    }
}
