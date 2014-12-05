public class AsyncLoad : UnityEngine.MonoBehaviour 
{
    public string next;
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
            gui_bar_script = Globals.canvasForLoading.GetComponentInChildren<GUIBarScript>();
            ui_text = Globals.canvasForLoading.GetComponentInChildren<UnityEngine.UI.Text>();
            FromLoadingSceneToNextScene();            
        }
    }   

    public void ToLoadSceneAsync(string nextLevelName)
    {
        Globals.EnableAllInput(false);        
        next = nextLevelName;
        Globals.transition.BlackOut(this, "_ToLoadingScene");                
    }

    public void _ToLoadingScene()
    {
        UnityEngine.Application.LoadLevel("loading");
    }

    public void FromLoadingSceneToNextScene()
    {
        StartCoroutine("_LoadSceneAsync");
    }

    System.Collections.IEnumerator _LoadSceneAsync()
    {
        // �������ٵģ����˿������ع��̡��Ժ�����������loadingHangarOperation.progress�Լ��ļ������������
        while (progress < 100 || progress == 100)
        {
            float val = (float)progress / 100.0f;
            gui_bar_script.Value = val;
            System.String text = "��������" + progress.ToString() + "%";
            ++progress;
            ui_text.text = text;
            yield return new UnityEngine.WaitForSeconds(0.01f);
        }
        progress = 0;
        yield return new UnityEngine.WaitForSeconds(1.0f);
        if (next != "")
        {
            loadingHangarOperation = UnityEngine.Application.LoadLevelAsync(next);            
        }        
    }

    void Update()
    {
        if (loadingHangarOperation != null && loadingHangarOperation.isDone)
        {
            // ��һ����Ч����Ϊʲô�أ�
            //Globals.input.enabled = true;
        }
        else
        {
            loadingHangarOperation = null;
        }
    }
}
