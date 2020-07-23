using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using TMPro;

public class StartGame : MonoBehaviour
{

    PlayerActions player;
    public Animator menuAnimator;
    public TextMeshProUGUI dialogueText;
    bool cutscene = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && cutscene)
        {
            StopAllCoroutines();
            GameManager.instance.PlayTrack(1);
            GameManager.instance.AdvanceLevel();
        }
    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerActions>();
    }

    public void Begin()
    {
        StartCoroutine(LoadCutscene());
    }

    IEnumerator LoadCutscene()
    {
        cutscene = true;
        menuAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        player.PreformAction(Direction.East, 5);
        player.gameObject.GetComponent<Animator>().SetBool("Breathing", true);        
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.StopAudio(2f);
        yield return new WaitForSeconds(.5f);
        dialogueText.text = "Go raid dungeons, they said";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "There'll be treasure, they said";
        yield return new WaitForSeconds(3f);
        dialogueText.text = "Nobody said anything about any bloody monsters";
        yield return new WaitForSeconds(3.5f);
        dialogueText.text = "Or evil curses for that matter";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "I've had quite enough of that for today, thankyou";
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = "";
        yield return new WaitForSeconds(1.5f);
        player.gameObject.GetComponent<Animator>().SetBool("Breathing", false);
        player.PreformAction(Direction.East, 17);
        yield return new WaitForSeconds(2);
        GameManager.instance.PlayTrack(1);
        GameManager.instance.AdvanceLevel();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
