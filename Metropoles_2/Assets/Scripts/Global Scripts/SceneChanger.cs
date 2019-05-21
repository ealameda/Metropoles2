using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Texture to show between scenes
    public Texture2D inbetween; 
    //Amount of time to fade scenes in and out
    public float fadeTime;
    // Are we currently changing scenes?
    bool changing = false;
    // fadingOut and fadingIn could both be false, but not both true.
    // If we are changing scenes, is this a fade out?
    bool fadingOut = false;

    // If we are changing scenes, is this a fade in? 
    bool fadingIn = false;
    float fadeT = 0.0f;
    
    // Mark all objects in the same scene as this indestructable. 
    void Start()
    {
        foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
            Object.DontDestroyOnLoad(rootObj);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
            StartChange();
        UpdateChange();
    }

    public void StartChange()
    {
        if (changing)
            return;
        changing = true;
        fadingOut = true;
        fadingIn = false;
        fadeT = 0;
    }

    void UpdateChange()
    {
        if (!changing)
            return;
        // Update the fade percentage
        if (fadingOut)
            fadeT += Time.deltaTime / fadeTime;
        else if (fadingIn)
            fadeT -= Time.deltaTime / fadeTime;
        Debug.Log("Fade time is " + fadeT);
        // If we've faded out past 1, time to change the scene. 
        if (fadeT > 1 && fadingOut)
        {
            fadeT = 1.0f;
            fadingOut = false;
            ChangeScene();
        }
        // If we've faded in past 0, we're done with the change. 
        if (fadeT < 0 && fadingIn)
            changing = false;
        
        
    }

    private void OnGUI() 
    {
        if (changing)
        {
            Color fadeColor = new Color (0,0,0,fadeT);
            GUI.color = fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), inbetween);

        }
    }

    void ChangeScene()
    {
        Debug.Log("Current scene is index " + SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Total number scenes "+ SceneManager.sceneCountInBuildSettings);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // We want to change to the next scene, but skip scene 0, which is our loading scene. 
        // This is almost certainly unnecessary, since we don't want to return to any scenes,
        // but might eventually be useful?
        nextSceneIndex = nextSceneIndex % (SceneManager.sceneCountInBuildSettings -1) + 1;
        Debug.Log("WHOOSH, next scene with index " + nextSceneIndex);
        SceneManager.sceneLoaded += StartFadeIn;
        Scene nextScene = SceneManager.GetSceneByBuildIndex(nextSceneIndex);
        SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Single);
        
    }

    void StartFadeIn(Scene scene, LoadSceneMode mode)
    {
        fadingIn = true;
    }


}
