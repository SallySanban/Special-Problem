using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AskName : MonoBehaviour
{
    public static AskName instance;

    [SerializeField] Image inputField;

    public bool inputFieldOnScreen;

    private void Awake()
    {
        instance = this;

        ShowHideInputField(false);
    }

    public void ShowHideInputField(bool show)
    {
        if (show)
        {
            if(!inputFieldOnScreen)
            {
                inputField.gameObject.SetActive(true);
                inputField.color = new Color(1, 1, 1, 0);

                StartCoroutine(Fade.FadeMethod(inputField, true));
                inputFieldOnScreen = true;
            }
        }
        else
        {
            inputField.gameObject.SetActive(false);

            inputFieldOnScreen = false;
        }
    }

    public void AskNameMethod()
    {
        ShowHideInputField(true);
    }
}
