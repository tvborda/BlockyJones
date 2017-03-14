using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Uween;

namespace VoxelStudy
{
	public class ScreenFader : MonoBehaviour {
	
	    public Image fadeImage;
	    [Range(0.0f, 10.0f)]
	    public float fadeInSpeed = 3.0f;
	    [Range(0.0f, 10.0f)]
	    public float fadeOutSpeed = 3.0f;
	    public string loadSceneName = "";
	    public bool startWithFade = true;
	    public bool loadOnFadeOut = false;
	
	    void Start()
	    {
	        if (!fadeImage)
	            Debug.LogError("Fade Image: not specified", this);

            if (loadSceneName == "")
                Debug.LogError("Load Scene Name: not specified", this);

            if (startWithFade)
	            FadeIn();
	    }
	
	    private void FadeIn()
	    {
	        TweenCA.Add(fadeImage.gameObject, fadeInSpeed, Color.clear).Then(FadeOut);
	    }
	
	    private void FadeOut()
	    {
	        TweenCA tween = TweenCA.Add(fadeImage.gameObject, fadeOutSpeed, Color.black);
	        if (loadOnFadeOut)
	            tween.Then(FadeOutCallback);
	    }
	
	    private void FadeOutCallback() { 
	        SceneManager.LoadScene(loadSceneName);
	    }
	}
}
