using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FullscreenHandler : MonoBehaviour
{
    [SerializeField] private GameObject fullScreenImage;
    [SerializeField] private GameObject revertImage;

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void Update()
    {
        revertImage.SetActive(Screen.fullScreen);
        fullScreenImage.SetActive(!Screen.fullScreen);
    }
}
