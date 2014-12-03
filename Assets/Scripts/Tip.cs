public class Tip : UnityEngine.MonoBehaviour 
{
    public UnityEngine.UI.Text uiText;
    public int fadeAwaySpeed = 3;
    public float currentFadeTime = 1;
    public float completeFadeTime = 2;
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


            //guiText.material.SetColor("_Color", tempTextColor);
            //guiTexture.color = tempTextureColor;
            yield return null;
        }
        
        StopCoroutine("_FadeAway");
        Globals.tipDisplay.msgList.RemoveAt(Globals.tipDisplay.msgList.Count-1);
        DestroyObject(gameObject);
    }
}