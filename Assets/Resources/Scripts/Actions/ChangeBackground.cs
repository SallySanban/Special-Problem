using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackground : MonoBehaviour
{
    public static ChangeBackground instance;

    [SerializeField] Image background;

    private void Awake()
    {
        instance = this;

        background.gameObject.SetActive(true);
    }

    public void ChangeBackgroundMethod(string filename)
    {
        if (filename == "Blackout") //surely there's a background on the scene to be able to blackout (and then deletes background)
        {
            StartCoroutine(Fade.FadeMethod(background, false));
        }
        else
        {
            if (background.gameObject.activeSelf)   //if the background is on the scene (switching from one bg to another)
            {
                StartCoroutine(SwitchBackground(filename));
            }
            else //if background is not on scene (going from blackout - where bg doesnt exist - to existing bg)
            {
                background.gameObject.SetActive(true);
                background.color = new Color(1, 1, 1, 0);

                background.sprite = Resources.Load<Sprite>("Art/" + filename);

                StartCoroutine(Fade.FadeMethod(background, true));
            }
        }
    }

    IEnumerator SwitchBackground(string filename)
    {
        yield return StartCoroutine(Fade.FadeMethod(background, false)); //deletes background

        background.gameObject.SetActive(true);
        background.color = new Color(1, 1, 1, 0);

        background.sprite = Resources.Load<Sprite>("Art/" + filename);

        StartCoroutine(Fade.FadeMethod(background, true));
    }
}
