using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenToggler : MonoBehaviour
{
    public int targetFPS = 60;
    public bool IsFullScreen, SetAtStart;
    public int Height, Width;
    private void Awake()
    {
        if (SetAtStart)
        {
            IsFullScreen = true;
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        }
    }
    private void Start()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
#if UNITY_WEBGL


        //Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        //return;
#endif
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
            IsFullScreen = true;
        else
            IsFullScreen = false;
       // Width = 1920;
        //Height = 1080;
        Width = Screen.currentResolution.width;
        Height = Screen.currentResolution.height;
    }
    private void Update()
    {
#if UNITY_WEBGL
        //return;
#endif
        if (Input.GetKeyDown(KeyCode.F11))
        {
            IsFullScreen = !IsFullScreen;
            if (!IsFullScreen)
            {

                Screen.SetResolution(Width, Height, false);
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            else
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            Debug.Log(IsFullScreen);
        }
    }
    public void FullScreen(bool IsFullScreen)
    {
        Screen.fullScreen = IsFullScreen;
    }
    public void SetFullScreen()
    {
        IsFullScreen = !IsFullScreen;
        if (!IsFullScreen)
        {
            Width = Screen.currentResolution.width;
            Height = Screen.currentResolution.height;

            Screen.SetResolution(Width, Height, false);
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        Debug.Log(IsFullScreen);
    }
}
