using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHeart : MonoBehaviour
{
    public static ShowHeart instance;

    [SerializeField] Image heart;

    private void Awake()
    {
        instance = this;

        heart.gameObject.SetActive(false);
    }

    private void ShowHideHeart(bool show)
    {
        if (show)
        {
            heart.gameObject.SetActive(true);
            heart.color = new Color(1, 1, 1, 0);

            StartCoroutine(Fade.FadeMethod(heart, true));
        }
        else
        {
            StartCoroutine(Fade.FadeMethod(heart, false));
        }
    }

    public void ShowHeartMethod()
    {
        Player.choicesIncorrect--;

        Debug.Log("CHOICES CORRECT: " + Player.choicesIncorrect);

        ShowHideHeart(true);

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);

        ShowHideHeart(false);
    }
}
