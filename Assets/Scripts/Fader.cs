using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;
    public GraphicRaycaster canvasBlock;

    public void Awake()
    {
        if (Fader.instance == null)
        {
            Fader.instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Fader.instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public Animator animator;

    public void FadeOut()
    {
        canvasBlock.enabled = true;
        animator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        canvasBlock.enabled = false;
        StartCoroutine(FadeInCo());
    }

    IEnumerator FadeInCo()
    {
        animator.SetBool("FadeIn", true);

        yield return new WaitForSeconds(.6f);

        animator.SetBool("FadeIn", false);
    }
}
