using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{

    UserInput userInput;

    private void Awake()
    {
        userInput = FindObjectOfType<UserInput>();
    }

    public void OnReload()
    {
        StartCoroutine(FadeAndReload());
    }

    IEnumerator FadeAndReload()
    {
        Fader.instance.FadeOut();
        yield return new WaitForSeconds(.4f);
        GameManager.instance.RestartLevel();
    }

    public void OnReloadEnter()
    {
        if (!GameManager.instance.IsReloading())
        {
            userInput.allowClick = false;
        }        
    }

    public void OnReloadExit()
    {
        userInput.allowClick = true;
    }

    public void ToggleMute()
    {
        GameManager.instance.ToggleAudio();
    }
}
