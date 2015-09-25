public class AsyncLoad : UnityEngine.MonoBehaviour 
{
    public System.String Next;
    public System.String LastLevelName;
    private UnityEngine.AsyncOperation loadingHangarOperation;
    float progress = 0;
    
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
        UnityEngine.Application.LoadLevel("loading");
    }

    System.Collections.IEnumerator _LoadSceneAsync()
    {
        // 先做个假的，让人看到加载过程。以后有需求了用loadingHangarOperation.progress以及文件下载器做真的
        while (progress < 100 || progress == 100)
        {
            float val = progress / 100.0f;
            
            Globals.loadingLevelController.progress.fillAmount = val;
            UnityEngine.Vector3 pos = Globals.loadingLevelController.rosa.GetComponent<UnityEngine.RectTransform>().anchoredPosition;
            Globals.loadingLevelController.rosa.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector3(val * UnityEngine.Screen.width, pos.y);
            
            
            progress += 2.0f;
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
