public class Tip : UnityEngine.MonoBehaviour 
{
    public UnityEngine.UI.Text uiText;
    public int fadeAwaySpeed = 3;
    public float currentFadeTime = 1;
    public float completeFadeTime = 2.5f;
    void Awake()
    {
        uiText = GetComponent<UnityEngine.UI.Text>();
        StartCoroutine("_FadeAway");
    }

    System.Collections.IEnumerator _FadeAway()
    {
        yield return null;
        while (uiText.color.a > 0.0f)
        {
            currentFadeTime = currentFadeTime - UnityEngine.Time.deltaTime;
            uiText.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, currentFadeTime / completeFadeTime);
            yield return null;
        }
        
        StopCoroutine("_FadeAway");        
        DestroyObject(transform.parent.gameObject);
    }

    public void OnDestroy()
    {
        Globals.tipDisplay.msgList.RemoveAt(Globals.tipDisplay.msgList.Count - 1);
    }
}