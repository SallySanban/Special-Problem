using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideProp : MonoBehaviour
{
    public static ShowHideProp instance;

    [SerializeField] Image prop;

    private void Awake()
    {
        instance = this;

        prop.gameObject.SetActive(false);
    }

    public void ShowHidePropMethod(string filename, bool show)
    {
        prop.sprite = Resources.Load<Sprite>("Art/" + filename);

        if (show)
        {
            prop.gameObject.SetActive(true);
            prop.color = new Color(1, 1, 1, 0);

            StartCoroutine(Fade.FadeMethod(prop, true));
        }
        else
        {
            StartCoroutine(Fade.FadeMethod(prop, false));
        }
    }
}
