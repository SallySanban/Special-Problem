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

        background.gameObject.SetActive(false);
    }

    public void ChangeBackgroundMethod(string filename)
    {
        background.gameObject.SetActive(true);
        background.color = new Color(1, 1, 1, 0);

        if (filename == "Blackout")
        {
            StartCoroutine(Fade.FadeMethod(background, false));
        }
        else
        {
            background.sprite = Resources.Load<Sprite>("Art/" + filename);

            StartCoroutine(Fade.FadeMethod(background, true));
        }
    }
}
