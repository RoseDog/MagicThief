public class AsyncLoad : UnityEngine.MonoBehaviour 
{
    public string next;
    private UnityEngine.AsyncOperation loadingHangarOperation;
    int pointCount = 0;

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void Load( string nextLevelName )
    {
        if (Globals.asyncLoad == null)
        {
            UnityEngine.GameObject asycnLoadPrefab = UnityEngine.Resources.Load("UI/AsyncLoad") as UnityEngine.GameObject;
            Globals.asyncLoad = (Instantiate(asycnLoadPrefab) as UnityEngine.GameObject).GetComponent<AsyncLoad>();
        }

        Globals.asyncLoad.LoadSceneAsync(nextLevelName);
    }

    public void LoadSceneAsync(string nextLevelName)
    {
        next = nextLevelName;
        StartCoroutine("_LoadSceneAsync");
    }

    System.Collections.IEnumerator _LoadSceneAsync()
    {
        yield return new UnityEngine.WaitForSeconds(0.5f);
        loadingHangarOperation = UnityEngine.Application.LoadLevelAsync(next);
    }

    void FixedUpdate()
    {
        if ( loadingHangarOperation != null && !loadingHangarOperation.isDone)
        {
            pointCount++;
            if( pointCount > 4 )
            {
                pointCount = 0;
            }

            guiText.text = "Loading" + new System.String('.', pointCount);            
        }
        else
        {
            guiText.text = "";
        }
    }
}
