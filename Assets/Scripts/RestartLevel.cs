using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RestartLevel : MonoBehaviour
{

    public static RestartLevel instance;
    public GameObject restartCanvas;
    public TextMeshProUGUI gameOverText;

    public void Awake()
    {
        if (RestartLevel.instance == null)
        {
            RestartLevel.instance = this;
            DontDestroyOnLoad(this);
        }
        else if (RestartLevel.instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        restartCanvas.SetActive(false);
    }

    public void ShowRestartCanvas(string displayText = "")
    {
        gameOverText.text = displayText;
        StartCoroutine(TriggerRestartCanvas());
    }

    IEnumerator TriggerRestartCanvas()
    {
        Fader.instance.FadeOut();
        yield return new WaitForSeconds(.6f);
        restartCanvas.SetActive(true);
    }

    public void OnRestartClicked()
    {
        GameManager.instance.RestartLevel();
        restartCanvas.SetActive(false);
    }
}
