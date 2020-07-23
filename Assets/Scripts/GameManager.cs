using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public int currentLevel = 0;
    public int finalLevel = 4;

    public AudioMixerSnapshot fadeOut;
    public AudioMixerSnapshot fadeIn;
    public AudioClip[] musicTracks;
    [Range(0, 1)] public float audioVolume = 0.5f;

    bool isReloading = false;
    bool levelComplete = false;
    bool audioOn = true;

    public void Awake()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
            DontDestroyOnLoad(this);
        }
        else if (GameManager.instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            StartCoroutine(LoadFirstLevel(0));
        }
    }

    public void RestartLevel()
    {
        if (isReloading)
        {
            return;
        }

        SceneManager.LoadScene(currentLevel);

        Fader.instance.FadeIn();

        //StartCoroutine(ReloadLevel());
    }

    IEnumerator ReloadLevel()
    {
        isReloading = true;

        AsyncOperation unload = SceneManager.UnloadSceneAsync(currentLevel);

        while (!unload.isDone)
        {
            yield return 0;
        }

        SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);

        //Fader.instance.FadeIn();

        isReloading = false;
    }

    public void AdvanceLevel()
    {
        if (levelComplete)
        {
            return;
        }

        levelComplete = true;

        StartCoroutine(LoadNextLevel());

    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(1f);

        Fader.instance.FadeOut();

        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene(currentLevel + 1);

        currentLevel++;

        Fader.instance.FadeIn();

        levelComplete = false;

        if (currentLevel+1 == finalLevel)
        {
            StartCoroutine(LoadFirstLevel());
        }
    }

    IEnumerator LoadFirstLevel(float delay = 5f)
    {
        yield return new WaitForSeconds(delay);

        Fader.instance.FadeOut();
        PlayTrack(0);

        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene(0);

        currentLevel = 0;

        Fader.instance.FadeIn();

        levelComplete = false;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public void PlayTrack(int trackNumber)
    {
        fadeIn.TransitionTo(.1f);
        AudioSource source = GetComponent<AudioSource>();
        source.clip = musicTracks[trackNumber];
        source.Play();
    }
    public void StopAudio(float duration)
    {        
        fadeOut.TransitionTo(duration);
    }

    public void ToggleAudio()
    {
        audioOn = !audioOn;
        AudioSource source = GetComponent<AudioSource>();
        source.volume = audioOn ? audioVolume : 0;
    }
}
