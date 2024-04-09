using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource[] audioSources = new AudioSource[2];

    private bool currentlyFading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < 2; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    //audioSource[0] is for music
    public void playSoundLoop(string sound)
    {
        audioSources[0].clip = Resources.Load<AudioClip>("Music/" + sound);
        audioSources[0].loop = true;

        StartCoroutine(FadeAudio(true));
    }

    //audioSource[1] is for sound effects
    public void playSoundEffect(string sound)
    {
        audioSources[1].clip = Resources.Load<AudioClip>("Music/" + sound);
        audioSources[1].loop = false;

        audioSources[1].Play();
    }

    public void stopSoundLoop()
    {
        StartCoroutine(FadeAudio(false));
    }

    IEnumerator FadeAudio(bool fadeIn)
    {
        while (currentlyFading)
        {
            yield return null;
        }

        float time = 0;
        float duration = 3f;
        float finalVolume = 0.3f;

        if (fadeIn)
        {
            currentlyFading = true;

            audioSources[0].volume = 0;

            audioSources[0].Play();

            while (time < duration)
            {
                audioSources[0].volume = Mathf.Lerp(0f, finalVolume, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            audioSources[0].volume = finalVolume;

            currentlyFading = false;
        }
        else
        {
            currentlyFading = true;

            while (time < duration)
            {
                audioSources[0].volume = Mathf.Lerp(finalVolume, 0f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            audioSources[0].volume = 0f;

            audioSources[0].Stop();

            currentlyFading = false;
        }
    }
}
