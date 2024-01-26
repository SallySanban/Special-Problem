using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowText : MonoBehaviour
{
    public static ShowText instance;

    [SerializeField] TextMeshProUGUI text;

    public bool textOnScreen = false;

    private void Awake()
    {
        instance = this;

        ShowHideText(false);
    }

    public void ShowHideText(bool show)
    {
        if (show)
        {
            if (!textOnScreen)
            {
                text.gameObject.SetActive(true);
                text.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(text, true));
                textOnScreen = true;
            }
        }
        else
        {
            text.gameObject.SetActive(false);

            textOnScreen = false;
        }
    }

    public void ShowTextMethod(string textToShow)
    {
        text.text = textToShow;

        ShowHideText(true);

        Talk.instance.ShowHideTextbox(false);
        Talk.instance.ShowHideNamebox(false);
    }
}
