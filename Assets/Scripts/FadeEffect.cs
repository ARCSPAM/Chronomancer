using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a fade effect for when the day ends
/// </summary>
public class FadeEffect : MonoBehaviour
{
    public float fadeTime = 1f;
    public Image fadePanel;
    public UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        fadePanel.color = new Color (0f, 0f, 0f, 0f);
    }

    /// <summary>
    /// Starts fade event
    /// </summary>
    public void Fade()
    {
        uiManager.HideEvent();
        StartCoroutine(FadeCoroutine());
    }
    /// <summary>
    /// Fade coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeCoroutine()
    {
        //darken screen over 1 second
        float elapsedTime = 0f;
        while(elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float a = Mathf.Clamp01(elapsedTime / fadeTime);
            fadePanel.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        //hold dark screen for half a second and
        yield return new WaitForSeconds(0.5f);
        elapsedTime = 0f;
        //bring original screen back over 1 second
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float a = Mathf.Clamp01(1f - (elapsedTime / fadeTime));
            fadePanel.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }
}
