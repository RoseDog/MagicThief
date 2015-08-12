public class AsyncLoad : UnityEngine.MonoBehaviour 
{
    UnityEngine.GameObject canvasForLoading;
    public System.String Next;
    public System.String LastLevelName;
    private UnityEngine.AsyncOperation loadingHangarOperation;
    int progress = 0;
    GUIBarScript gui_bar_script;
    UnityEngine.UI.Text ui_text;
    public System.Collections.Generic.List<BuildingData> updateRoseTimeBuildings = new System.Collections.Generic.List<BuildingData>();
    public System.Collections.Generic.List<BuildingData> updateNewTargetTimeBuildings = new System.Collections.Generic.List<BuildingData>();
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

            //loadingHangarOperation = UnityEngine.Application.LoadLevelAsync(Next);
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
        if (Globals.magician != null)
        {
            Globals.magician.gameObject.SetActive(false);
        }
        
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
            progress += 4;
            yield return new UnityEngine.WaitForSeconds(0.0f);
        }
        progress = 0;
        yield return new UnityEngine.WaitForSeconds(0.3f);
        if (Next != "")
        {
            while(true)
            {
                yield return new UnityEngine.WaitForSeconds(0.1f);
                if(Globals.socket.IsReady())
                {
                    break;
                }
            }
            loadingHangarOperation = UnityEngine.Application.LoadLevelAsync(Next);            
        }        
    }

    void Update()
    {
        if (loadingHangarOperation != null)
        {
//             if(Globals.socket.IsReady())
//             {
//                 loadingHangarOperation.allowSceneActivation = true;
//             }
//             else
//             {
//                 loadingHangarOperation.allowSceneActivation = false;
//             }
//             
//             if (loadingHangarOperation.isDone)
//             {
// 
//             }
            
            // 这一行无效。。为什么呢？
            //Globals.input.enabled = true;

            //gui_bar_script.Value = loadingHangarOperation.progress;
            //Globals.languageTable.SetText(ui_text, "level_loading_progress", new System.String[] { ((int)(loadingHangarOperation.progress*100)).ToString() });
        }
        else
        {
            loadingHangarOperation = null;
        }
        foreach (BuildingData data in updateRoseTimeBuildings)
        {
            data.roseGrowLastDuration -= UnityEngine.Time.deltaTime;
        }

        foreach (BuildingData data in updateNewTargetTimeBuildings)
        {
            data.bornNewTargetLastDuration -= UnityEngine.Time.deltaTime;
        }        
    }

    public void AddBuildingRoseTimeUpdate(BuildingData data)
    {
        if (!updateRoseTimeBuildings.Contains(data))
        {
            updateRoseTimeBuildings.Add(data);
        }
    }

    public void RemoveBuildingRoseTimeUpdate(BuildingData data)
    {
        updateRoseTimeBuildings.Remove(data);
    }

    public void AddBuildingNewTargetTimeUpdate(BuildingData data)
    {
        if (!updateNewTargetTimeBuildings.Contains(data))
        {
            updateNewTargetTimeBuildings.Add(data);
        }        
    }    

    public void RemoveBuildingNewTargetTimeUpdate(BuildingData data)
    {
        updateNewTargetTimeBuildings.Remove(data);
    }
}
